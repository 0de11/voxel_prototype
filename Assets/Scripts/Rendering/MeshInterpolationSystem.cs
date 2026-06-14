using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using VoxelGame.Core;

namespace VoxelGame.Rendering
{
    [BurstCompile]
    public partial struct MeshInterpolationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<VisualBlockComponent>();
            state.RequireForUpdate<WorldGridComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;

            foreach (var (visual, transform) in SystemAPI.Query<RefRW<VisualBlockComponent>, RefRW<LocalTransform>>())
            {
                visual.ValueRW.CurrentPosition = math.lerp(visual.ValueRO.CurrentPosition, visual.ValueRO.TargetPosition, dt * 5f);
                transform.ValueRW.Position = visual.ValueRO.CurrentPosition;
            }
        }
    }
}
