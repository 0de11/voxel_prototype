using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using VoxelGame.Core;

namespace VoxelGame.Rendering
{
    public partial class BlockSpawnerSystem : SystemBase
    {
        private bool hasSpawned = false;

        protected override void OnUpdate()
        {
            if (hasSpawned) return;

            if (!SystemAPI.TryGetSingletonEntity<WorldGridComponent>(out var gridEntity)) return;
            if (!SystemAPI.TryGetSingleton<GridRendererComponent>(out var renderer)) return;

            var grid = SystemAPI.GetComponent<WorldGridComponent>(gridEntity);
            var blocks = grid.Blocks;

            if (UnityEngine.Camera.main != null)
            {
                UnityEngine.Camera.main.transform.position = new float3(4.5f, 10f, -5f);
                UnityEngine.Camera.main.transform.rotation = quaternion.Euler(math.radians(45f), 0f, 0f);
            }

            for (int z = 0; z < grid.Depth; z++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    int index = grid.GetIndex(x, 0, z);
                    var block = blocks[index];
                    block.Type = BlockType.Stone;
                    block.IsSolid = true;
                    block.Gravity = GravityState.Stable;
                    blocks[index] = block;
                }
            }

            int playerIndex = grid.GetIndex(grid.Width / 2, 1, grid.Depth / 2);
            var pBlock = blocks[playerIndex];
            pBlock.Type = BlockType.Player;
            pBlock.IsSolid = true;
            blocks[playerIndex] = pBlock;

            for (int z = 0; z < grid.Depth; z++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    for (int x = 0; x < grid.Width; x++)
                    {
                        int index = grid.GetIndex(x, y, z);
                        var block = blocks[index];

                        if (block.Type != BlockType.Air)
                        {
                            Entity visualEntity;
                            if (block.Type == BlockType.Player)
                            {
                                visualEntity = EntityManager.Instantiate(renderer.PlayerPrefab);
                                EntityManager.AddComponentData(visualEntity, new VoxelGame.Player.PlayerComponent
                                {
                                    LogicalX = x,
                                    LogicalY = y,
                                    LogicalZ = z,
                                    Facing = VoxelGame.Player.FacingDirection.North
                                });
                                EntityManager.AddComponentData(visualEntity, new VoxelGame.Player.PlayerInputComponent());
                            }
                            else
                            {
                                visualEntity = EntityManager.Instantiate(renderer.BlockPrefab);
                            }

                            EntityManager.AddComponentData(visualEntity, new VisualBlockComponent
                            {
                                CurrentPosition = new float3(x, y, z),
                                TargetPosition = new float3(x, y, z),
                                LogicalIndex = index
                            });

                            EntityManager.SetComponentData(visualEntity, LocalTransform.FromPosition(new float3(x, y, z)));
                            
                            block.VisualEntity = visualEntity;
                            blocks[index] = block;
                        }
                    }
                }
            }

            hasSpawned = true;
        }
    }
}
