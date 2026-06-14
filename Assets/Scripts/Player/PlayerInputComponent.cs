using Unity.Entities;

namespace VoxelGame.Player
{
    public struct PlayerInputComponent : IComponentData
    {
        public int MoveX;
        public int MoveZ;
        public bool CastSpell;
    }
}
