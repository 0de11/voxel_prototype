using Unity.Entities;
using Unity.Burst;
using VoxelGame.Core;
using VoxelGame.Systems;

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

            foreach (var (player, input) in SystemAPI.Query<RefRO<PlayerComponent>, RefRO<PlayerInputComponent>>())
            {
                if (input.ValueRO.CastSpell)
                {
                    int targetX = player.ValueRO.LogicalX;
                    int targetY = player.ValueRO.LogicalY;
                    int targetZ = player.ValueRO.LogicalZ;

                    switch (player.ValueRO.Facing)
                    {
                        case FacingDirection.North: targetZ += 1; break;
                        case FacingDirection.South: targetZ -= 1; break;
                        case FacingDirection.East: targetX += 1; break;
                        case FacingDirection.West: targetX -= 1; break;
                    }

                    if (targetX >= 0 && targetX < grid.Width && targetZ >= 0 && targetZ < grid.Depth)
                    {
                        var commandEntity = state.EntityManager.CreateEntity();
                        state.EntityManager.AddComponentData(commandEntity, new GravitySpellCommand
                        {
                            TargetX = targetX,
                            TargetY = targetY,
                            TargetZ = targetZ
                        });
                    }
                }
            }
        }
    }
}
