using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using VoxelGame.Core;
using VoxelGame.Rendering;

namespace VoxelGame.Player
{
    [BurstCompile]
    public partial struct SpellCastingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldGridComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var gridEntity = SystemAPI.GetSingletonEntity<WorldGridComponent>();
            var grid = SystemAPI.GetComponent<WorldGridComponent>(gridEntity);

            foreach (var (playerRw, input) in SystemAPI.Query<RefRW<PlayerComponent>, RefRO<PlayerInputComponent>>())
            {
                ref var player = ref playerRw.ValueRW;

                if (input.ValueRO.IsCasting)
                {
                    if (!player.IsDecreasingGravity)
                    {
                        int targetX = player.LogicalX;
                        int targetZ = player.LogicalZ;

                        switch (player.Facing)
                        {
                            case FacingDirection.North: targetZ += 1; break;
                            case FacingDirection.South: targetZ -= 1; break;
                            case FacingDirection.East: targetX += 1; break;
                            case FacingDirection.West: targetX -= 1; break;
                        }

                        if (targetX >= 0 && targetX < grid.Width && targetZ >= 0 && targetZ < grid.Depth)
                        {
                            int floorIndex = grid.GetIndex(targetX, 0, targetZ);
                            int hoverIndex = grid.GetIndex(targetX, 1, targetZ);

                            var floorBlock = grid.Blocks[floorIndex];
                            var hoverBlock = grid.Blocks[hoverIndex];

                            if (floorBlock.Type != BlockType.Air && hoverBlock.Type == BlockType.Air)
                            {
                                floorBlock.Gravity = GravityState.Levitating;
                                
                                grid.Blocks[hoverIndex] = floorBlock;
                                grid.Blocks[floorIndex] = hoverBlock;

                                if (floorBlock.VisualEntity != Entity.Null && SystemAPI.HasComponent<VisualBlockComponent>(floorBlock.VisualEntity))
                                {
                                    var visual = SystemAPI.GetComponent<VisualBlockComponent>(floorBlock.VisualEntity);
                                    visual.TargetPosition = new float3(targetX, 1, targetZ);
                                    SystemAPI.SetComponent(floorBlock.VisualEntity, visual);
                                }

                                player.IsDecreasingGravity = true;
                                player.LiftedX = targetX;
                                player.LiftedZ = targetZ;
                            }
                        }
                    }
                }
                else
                {
                    if (player.IsDecreasingGravity)
                    {
                        int hoverIndex = grid.GetIndex(player.LiftedX, 1, player.LiftedZ);
                        var hoverBlock = grid.Blocks[hoverIndex];
                        
                        if (hoverBlock.Gravity == GravityState.Levitating)
                        {
                            hoverBlock.Gravity = GravityState.Stable;
                            grid.Blocks[hoverIndex] = hoverBlock;
                        }

                        player.IsDecreasingGravity = false;
                    }
                }
            }
        }
    }
}
