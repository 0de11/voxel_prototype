using System;
using Unity.Entities;

namespace VoxelGame.Core
{
    public enum BlockType : byte
    {
        Player = 0,
        Air = 1,
        Stone = 2
    }

    public enum GravityState : byte
    {
        Stable = 0,
        Levitating = 1
    }

    [Serializable]
    public struct GridBlock
    {
        public BlockType Type;
        public GravityState Gravity;
        public bool IsSolid;
        public Entity VisualEntity;
    }
}
