using Unity.Entities;
using UnityEngine;

namespace VoxelGame.Rendering
{
    public class GridRendererAuthoring : MonoBehaviour
    {
        public GameObject blockPrefab;
        public GameObject playerPrefab;

        class Baker : Baker<GridRendererAuthoring>
        {
            public override void Bake(GridRendererAuthoring authoring)
            {
                if (authoring.blockPrefab == null || authoring.playerPrefab == null)
                {
                    Debug.LogError("GridRendererAuthoring: Please assign BOTH the Block Prefab and Player Prefab in the Inspector!");
                    return;
                }

                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GridRendererComponent
                {
                    BlockPrefab = GetEntity(authoring.blockPrefab, TransformUsageFlags.Dynamic),
                    PlayerPrefab = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}
