using System;
using System.Collections.Generic;

using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

namespace MinecraftLike
{
    public class World
    {
        private Dictionary<int3, Chunk> _chunks;
        private Queue<int3> _chunksToConstruct;

        private List<(int3, JobHandle)> _scheduledChunks;
        public int MaxScheduledChunksPerFrames { get; private set; }

        public World(int maxScheduledChunksPerFrames)
        {
            _chunks = new Dictionary<int3, Chunk>();
            _chunksToConstruct = new Queue<int3>();
            _scheduledChunks = new List<(int3, JobHandle)>();
            MaxScheduledChunksPerFrames = maxScheduledChunksPerFrames;
        }
        
        public bool CreateChunk(int3 at)
        {
            if(_chunks.ContainsKey(at))
            {
                Debug.LogWarning($"Can't add chunk because another chunk already exist at this position ({at})...");
                return false;
            }

            var chunk = new GameObject($"Chunk {at}").AddComponent<Chunk>();
            chunk.transform.position = (float3)at;
            _chunks.Add(at, chunk);
            
            return true;
        }

        public void ConstructChunk(int3 at)
        {
            if(!_chunks.ContainsKey(at))
            {
                Debug.LogWarning($"Can't construct chunk at {at} because there is no chunk registered at this position");
                return;
            }

            _chunksToConstruct.Enqueue(at);
        }

        public void Update()
        {
            if(_chunksToConstruct.Count != 0)
            {
                var chunkPos = _chunksToConstruct.Dequeue();

                JobHandle handle = default;

                if(!_chunks[chunkPos].IsBlocksInitialized)
                    handle = _chunks[chunkPos].ScheduleInitializeBlocks();

                handle = _chunks[chunkPos].ScheduleConstructGeometry(handle);

                _scheduledChunks.Add((chunkPos, handle));
            }

            for(int i = 0, c = 0; i < _scheduledChunks.Count && c < MaxScheduledChunksPerFrames; i++, c++)
            {
                if(_scheduledChunks[i].Item2.IsCompleted)
                {
                    _scheduledChunks[i].Item2.Complete();
                    _chunks[_scheduledChunks[i].Item1].FinalizeConstructGeometry();
                    _scheduledChunks.RemoveAt(i);
                }
            }
        }

        public void Destroy()
        {
            _chunks.Clear();
            _chunksToConstruct.Clear();
            _scheduledChunks.Clear();
        }
    }
}