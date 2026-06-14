using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using System.IO;

namespace VoxelGame.Core
{
    public class WorldManager : MonoBehaviour
    {
        public int width = 10;
        public int height = 10;
        public int depth = 10;

        private Entity gridEntity;
        private EntityManager entityManager;

        private void Start()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            gridEntity = entityManager.CreateEntity(typeof(WorldGridComponent));
            
            var gridComponent = new WorldGridComponent
            {
                Width = width,
                Height = height,
                Depth = depth,
                Blocks = new NativeArray<GridBlock>(width * height * depth, Allocator.Persistent)
            };

            entityManager.SetComponentData(gridEntity, gridComponent);
        }

        private void OnDestroy()
        {
            if (entityManager != default && entityManager.Exists(gridEntity))
            {
                var gridComponent = entityManager.GetComponentData<WorldGridComponent>(gridEntity);
                if (gridComponent.Blocks.IsCreated)
                {
                    gridComponent.Blocks.Dispose();
                }
            }
        }

        [System.Serializable]
        private class SerializableGrid
        {
            public int width;
            public int height;
            public int depth;
            public GridBlock[] blocks;
        }

        public void SaveToJson(string filePath)
        {
            var gridComponent = entityManager.GetComponentData<WorldGridComponent>(gridEntity);
            var managedArray = gridComponent.Blocks.ToArray();
            
            var serializableGrid = new SerializableGrid
            {
                width = gridComponent.Width,
                height = gridComponent.Height,
                depth = gridComponent.Depth,
                blocks = managedArray
            };

            string json = JsonUtility.ToJson(serializableGrid, true);
            File.WriteAllText(filePath, json);
        }

        public void LoadFromJson(string filePath)
        {
            if (!File.Exists(filePath)) return;

            string json = File.ReadAllText(filePath);
            var serializableGrid = JsonUtility.FromJson<SerializableGrid>(json);

            var gridComponent = entityManager.GetComponentData<WorldGridComponent>(gridEntity);
            
            if (gridComponent.Blocks.IsCreated)
            {
                gridComponent.Blocks.Dispose();
            }

            gridComponent.Width = serializableGrid.width;
            gridComponent.Height = serializableGrid.height;
            gridComponent.Depth = serializableGrid.depth;
            gridComponent.Blocks = new NativeArray<GridBlock>(serializableGrid.blocks, Allocator.Persistent);

            entityManager.SetComponentData(gridEntity, gridComponent);
        }
    }
}
