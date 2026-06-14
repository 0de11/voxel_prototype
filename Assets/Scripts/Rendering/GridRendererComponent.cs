using Unity.Entities;

namespace VoxelGame.Rendering
{
    public struct GridRendererComponent : IComponentData
    {
        public Entity BlockPrefab;
        public Entity PlayerPrefab;
    }
}
