using Unity.Entities;
using Unity.Burst;
using VoxelGame.Core;

namespace VoxelGame.Systems
{
    public struct GravitySpellCommand : IComponentData
    {
        public int TargetX;
        public int TargetY;
        public int TargetZ;
    }

    [BurstCompile]
    public partial struct InteractionSystem : ISystem
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

            foreach (var (spell, entity) in SystemAPI.Query<RefRO<GravitySpellCommand>>().WithEntityAccess())
            {
                int index = grid.GetIndex(spell.ValueRO.TargetX, spell.ValueRO.TargetY, spell.ValueRO.TargetZ);
                if (index >= 0 && index < grid.Blocks.Length)
                {
                    var block = grid.Blocks[index];
                    block.Gravity = GravityState.Floating;
                    grid.Blocks[index] = block;
                }
                
                state.EntityManager.DestroyEntity(entity);
            }

            for (int z = 0; z < grid.Depth; z++)
            {
                for (int y = grid.Height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < grid.Width; x++)
                    {
                        int index = grid.GetIndex(x, y, z);
                        var block = grid.Blocks[index];

                        if (block.Gravity == GravityState.Floating)
                        {
                            if (y + 1 < grid.Height)
                            {
                                int aboveIndex = grid.GetIndex(x, y + 1, z);
                                var aboveBlock = grid.Blocks[aboveIndex];

                                if (aboveBlock.Type == BlockType.Air)
                                {
                                    grid.Blocks[aboveIndex] = block;
                                    grid.Blocks[index] = aboveBlock;
                                }
                                else
                                {
                                    block.Gravity = GravityState.Stable;
                                    grid.Blocks[index] = block;
                                }
                            }
                            else
                            {
                                block.Gravity = GravityState.Stable;
                                grid.Blocks[index] = block;
                            }
                        }
                    }
                }
            }
        }
    }
}
