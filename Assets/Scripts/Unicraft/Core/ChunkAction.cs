using Unity.Mathematics;

namespace Unicraft.Core
{
    public enum ChunkCommand
    {
        Add,
        Remove,
        Update,
    } 

    public struct ChunkCommandBuffer
    {
        public int3 ChunkPosition { get; private set; }
        public ChunkCommand Command { get; private set; }

        public ChunkCommandBuffer(in int3 chunkPosition, in ChunkCommand command)
        {
            ChunkPosition = chunkPosition;
            Command = command;
        }
    }
}