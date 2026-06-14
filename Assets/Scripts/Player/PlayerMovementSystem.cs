using Unity.Entities;
using Unity.Burst;
using VoxelGame.Core;
using Unity.Mathematics;
using VoxelGame.Rendering;

namespace VoxelGame.Player
{
    [BurstCompile]
    public partial struct PlayerMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldGridComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            var gridEntity = SystemAPI.GetSingletonEntity<WorldGridComponent>();
            var grid = SystemAPI.GetComponent<WorldGridComponent>(gridEntity);

            foreach (var (player, input, visual) in SystemAPI.Query<RefRW<PlayerComponent>, RefRO<PlayerInputComponent>, RefRW<VisualBlockComponent>>())
            {
                if (player.ValueRO.MoveTimer > 0)
                {
                    player.ValueRW.MoveTimer -= dt;
                    continue; 
                }

                if (input.ValueRO.IsCasting) continue;

                int dx = input.ValueRO.MoveX;
                int dz = input.ValueRO.MoveZ;

                if (dx != 0 || dz != 0)
                {
                    if (dx > 0) player.ValueRW.Facing = FacingDirection.East;
                    else if (dx < 0) player.ValueRW.Facing = FacingDirection.West;
                    else if (dz > 0) player.ValueRW.Facing = FacingDirection.North;
                    else if (dz < 0) player.ValueRW.Facing = FacingDirection.South;

                    int targetX = player.ValueRO.LogicalX + dx;
                    int targetY = player.ValueRO.LogicalY;
                    int targetZ = player.ValueRO.LogicalZ + dz;

                    if (targetX >= 0 && targetX < grid.Width && targetZ >= 0 && targetZ < grid.Depth)
                    {
                        int targetIndex = grid.GetIndex(targetX, targetY, targetZ);
                        var targetBlock = grid.Blocks[targetIndex];

                        if (targetBlock.Type == BlockType.Air)
                        {
                            bool isBeneathSolid = false;
                            if (targetY > 0)
                            {
                                int beneathIndex = grid.GetIndex(targetX, targetY - 1, targetZ);
                                var beneathBlock = grid.Blocks[beneathIndex];
                                isBeneathSolid = beneathBlock.IsSolid;
                            }

                            if (isBeneathSolid || targetY == 0) 
                            {
                                int currentIndex = grid.GetIndex(player.ValueRO.LogicalX, player.ValueRO.LogicalY, player.ValueRO.LogicalZ);
                                var currentBlock = grid.Blocks[currentIndex];

                                grid.Blocks[targetIndex] = currentBlock;
                                grid.Blocks[currentIndex] = targetBlock;

                                player.ValueRW.LogicalX = targetX;
                                player.ValueRW.LogicalY = targetY;
                                player.ValueRW.LogicalZ = targetZ;

                                player.ValueRW.MoveTimer = 0.2f;

                                visual.ValueRW.TargetPosition = new float3(targetX, targetY, targetZ);
                            }
                        }
                    }
                }
            }
        }
    }
}
