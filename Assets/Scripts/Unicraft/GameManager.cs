using UnityEngine;
using Unity.Mathematics;

namespace Unicraft.Core
{
    public class GameManager : MonoBehaviour, ISerializationCallbackReceiver
    {
        public static int3 ChunkSize { get; private set; }
        public static Material ChunkMaterial { get; private set; }
        public static World<DefaultChunkManager> World { get; private set; }

        public int3 chunkSize;
        public Material chunkMaterial;
        public int maxScheduledChunksPerFrame = 1;

        private void Awake()
        {
            if(World != null)
                World.Destroy();
            
            World = new World<DefaultChunkManager>(maxScheduledChunksPerFrame);
        }

        public void OnAfterDeserialize()
        {
            ChunkSize = chunkSize;
            ChunkMaterial = chunkMaterial;
        }

        public void OnBeforeSerialize()
        {
            chunkSize = ChunkSize;
            chunkMaterial = ChunkMaterial;
        }

        private void Update()
        {
            World.Update();
        }

        private void OnDestroy()
        {
            World.Destroy();
        }
    }
}