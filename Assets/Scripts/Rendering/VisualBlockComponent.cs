using Unity.Entities;
using Unity.Mathematics;

namespace VoxelGame.Rendering
{
    public struct VisualBlockComponent : IComponentData
    {
        public float3 CurrentPosition;
        public float3 TargetPosition;
        public int LogicalIndex;
    }
}
