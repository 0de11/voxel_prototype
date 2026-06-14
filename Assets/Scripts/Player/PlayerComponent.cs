using Unity.Entities;

namespace VoxelGame.Player
{
    public enum FacingDirection : byte
    {
        North = 0, // +Z
        East = 1,  // +X
        South = 2, // -Z
        West = 3   // -X
    }

    public struct PlayerComponent : IComponentData
    {
        public int LogicalX;
        public int LogicalY;
        public int LogicalZ;
        public FacingDirection Facing;
        public float MoveTimer; // To prevent moving every single frame
    }
}
