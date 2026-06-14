using Unity.Entities;
using Unity.Collections;

namespace VoxelGame.Core
{
    public struct WorldGridComponent : IComponentData
    {
        public int Width;
        public int Height;
        public int Depth;
        
        public NativeArray<GridBlock> Blocks;

        public int GetIndex(int x, int y, int z)
        {
            return x + (y * Width) + (z * Width * Height);
        }
    }
}
