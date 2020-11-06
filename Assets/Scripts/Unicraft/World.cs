using Unity.Mathematics;

namespace Unicraft.Core
{
    public class World<T> where T : ChunkManager, new()
    {
        public ChunkManager ChunkManager { get; private set; }

        public static World<T> Create(int maxScheduledChunksPerFrames) =>
            new World<T>(maxScheduledChunksPerFrames);

        public World(int maxScheduledChunksPerFrames)
        {
            ChunkManager = new T();
            ChunkManager.MaxChunksConstructPerFrames = maxScheduledChunksPerFrames;
        }

        public void CreateChunk(int3 at) =>
            ChunkManager.EnqueueCommandBuffer(new ChunkCommandBuffer(at, ChunkCommand.Add));
        
        public void DestroyChunk(int3 at) =>
            ChunkManager.EnqueueCommandBuffer(new ChunkCommandBuffer(at, ChunkCommand.Remove));
        
        public void UpdateChunk(int3 at) =>
            ChunkManager.EnqueueCommandBuffer(new ChunkCommandBuffer(at, ChunkCommand.Update));

        public void Update()
        {   
            ChunkManager.CheckAndCompleteScheduledChunks();

            while(ChunkManager.AmountOfCommandBuffers > 0)
            {
                var ccb = ChunkManager.GetChunkCommandBuffer();

                switch(ccb.Command)
                {
                    case ChunkCommand.Add:
                        ChunkManager.OnChunkCreate(ccb);
                        break;
                    case ChunkCommand.Remove:
                        ChunkManager.OnChunkDestroy(ccb);
                        break;
                    case ChunkCommand.Update:
                        ChunkManager.OnChunkUpdate(ccb);
                        break;
                    default: 
                        break;
                }

                ChunkManager.CheckAndCompleteScheduledChunks();
            }
        }

        public void Destroy()
        {
            ChunkManager.Dispose();
        }
    }
}