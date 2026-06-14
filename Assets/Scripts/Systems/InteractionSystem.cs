using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using VoxelGame.Core;
using VoxelGame.Rendering;

namespace VoxelGame.Systems
{
    // Keeping this for compatibility if anything references it, but we no longer use it
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

            // Downward Gravity Processing (Bottom-Up to allow cascading falls)
            for (int z = 0; z < grid.Depth; z++)
            {
                for (int y = 1; y < grid.Height; y++)
                {
                    for (int x = 0; x < grid.Width; x++)
                    {
                        int index = grid.GetIndex(x, y, z);
                        var block = grid.Blocks[index];

                        if (block.Type != BlockType.Air && block.Gravity == GravityState.Stable)
                        {
                            int belowIndex = grid.GetIndex(x, y - 1, z);
                            var belowBlock = grid.Blocks[belowIndex];

                            if (belowBlock.Type == BlockType.Air)
                            {
                                // Swap
                                grid.Blocks[belowIndex] = block;
                                grid.Blocks[index] = belowBlock; // Air

                                // Update Visuals
                                if (block.VisualEntity != Entity.Null && SystemAPI.HasComponent<VisualBlockComponent>(block.VisualEntity))
                                {
                                    var visual = SystemAPI.GetComponent<VisualBlockComponent>(block.VisualEntity);
                                    visual.TargetPosition = new float3(x, y - 1, z);
                                    SystemAPI.SetComponent(block.VisualEntity, visual);
                                }
                                
                                if (belowBlock.VisualEntity != Entity.Null && SystemAPI.HasComponent<VisualBlockComponent>(belowBlock.VisualEntity))
                                {
                                    var visual = SystemAPI.GetComponent<VisualBlockComponent>(belowBlock.VisualEntity);
                                    visual.TargetPosition = new float3(x, y, z);
                                    SystemAPI.SetComponent(belowBlock.VisualEntity, visual);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
