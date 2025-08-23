using BooterAndBigARM.World;

namespace BooterAndBigARM.World.Decor
{
    public interface IChunkDecorator
    {
        // Called once right after a chunk is created.
        void Decorate(Chunk chunk);
    }
}