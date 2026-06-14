using System;

namespace VoxelGame.Core
{
    public enum BlockType : byte
    {
        Air = 0,
        Stone = 1,
        InteractiveCube = 2,
        Switch = 3,
        Player = 4
    }

    public enum GravityState : byte
    {
        Stable = 0,
        Floating = 1,
        Falling = 2
    }

    [Serializable]
    public struct GridBlock
    {
        public BlockType Type;
        public GravityState Gravity;
        public bool IsSolid;
    }
}
