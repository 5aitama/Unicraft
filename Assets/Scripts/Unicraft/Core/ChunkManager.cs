using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

using System;
using UnityEngine;

namespace Unicraft.Core
{
    public abstract class ChunkManager : IDisposable
    {
        /// <summary>
        /// Store all created chunks by their world position.
        /// </summary>
        public Dictionary<int3, Chunk> Chunks { get; protected set; } = new Dictionary<int3, Chunk>();

        /// <summary>
        /// Store all command buffers.
        /// </summary>
        protected Queue<ChunkCommandBuffer> ChunkCommandBuffers { get; set; } = new Queue<ChunkCommandBuffer>();

        /// <summary>
        /// HashSet that store world position of chunks that would be used in threads.
        /// </summary>
        protected NativeHashSet<int3> UsedChunks { get; set; } = new NativeHashSet<int3>(1, Allocator.Persistent);

        /// <summary>
        /// Store destroyed chunks.
        /// </summary>
        protected Stack<Chunk> DestroyedChunks { get; set; } = new Stack<Chunk>();

        /// <summary>
        /// Contains all scheduled chunks to be processing by a job.
        /// </summary>
        public List<(int3, JobHandle)> ScheduledChunks { get; protected set; } = new List<(int3, JobHandle)>();

        /// <summary>
        /// The max amount of chunks that can be contruct per frames.
        /// </summary>
        public int MaxChunksConstructPerFrames { get; set; } = 1;

        /// <summary>
        /// Called when a chunk need to be created.
        /// </summary>
        /// <param name="ccb">The command buffer</param>
        public virtual void OnChunkCreate(in ChunkCommandBuffer ccb)
        {
            if(HasChunkAt(ccb.ChunkPosition))
            {
                Debug.LogWarning($"Can't add chunk because another chunk already exist at this position : {ccb.ChunkPosition}");
                return;
            }

            var chunk = new GameObject($"Chunk {ccb.ChunkPosition}").AddComponent<Chunk>();
            chunk.transform.position = (float3)ccb.ChunkPosition;
            Chunks.Add(ccb.ChunkPosition, chunk);
        }

        /// <summary>
        /// Called when a chunk need to be destroyed.
        /// </summary>
        /// <param name="ccb">The command buffer</param>
        public virtual void OnChunkDestroy(in ChunkCommandBuffer ccb)
        {
            if(!HasChunkAt(ccb.ChunkPosition))
            {
                Debug.LogWarning($"Can't remove chunk because there is no chunk at this position : {ccb.ChunkPosition}");
                return;
            }

            if(UsedChunks.Contains(ccb.ChunkPosition))
            {
                ChunkCommandBuffers.Enqueue(ccb);
            }
            else
            {
                var chunk = GetChunkAt(ccb.ChunkPosition);
                chunk.gameObject.SetActive(false);
                DestroyedChunks.Push(chunk);
            }
        }

        /// <summary>
        /// Called when a chunk need to be updated.
        /// </summary>
        /// <param name="ccb">The command buffer</param>
        public virtual void OnChunkUpdate(in ChunkCommandBuffer ccb)
        {
            if(!HasChunkAt(ccb.ChunkPosition))
            {
                Debug.LogWarning($"Can't update chunk because there is no chunk at this position : {ccb.ChunkPosition}");
                return;
            }

            UsedChunks.Add(ccb.ChunkPosition);

            JobHandle handle = default;

            if(!Chunks[ccb.ChunkPosition].IsBlocksInitialized)
                handle = Chunks[ccb.ChunkPosition].ScheduleInitializeBlocks();

            handle = Chunks[ccb.ChunkPosition].ScheduleConstructGeometry(handle);

            ScheduledChunks.Add((ccb.ChunkPosition, handle));
        }

        public virtual void CheckAndCompleteScheduledChunks()
        {
            for(int i = 0, c = 0; i < ScheduledChunks.Count && c < MaxChunksConstructPerFrames; i++, c++)
            {
                if(ScheduledChunks[i].Item2.IsCompleted)
                {
                    ScheduledChunks[i].Item2.Complete();
                    Chunks[ScheduledChunks[i].Item1].FinalizeConstructGeometry();
                    UsedChunks.Remove(ScheduledChunks[i].Item1);
                    ScheduledChunks.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Enqueue a chunk command buffer.
        /// </summary>
        /// <param name="ccb">The chunk command buffer that would be enqeued</param>
        public virtual void EnqueueCommandBuffer(in ChunkCommandBuffer ccb) =>
            ChunkCommandBuffers.Enqueue(ccb);

        /// <summary>
        /// Check if there is a chunk at <paramref name="position"/>
        /// </summary>
        public virtual bool HasChunkAt(int3 position) =>
            Chunks.ContainsKey(position);
        
        /// <summary>
        /// Get a chunk by its <paramref name="position"/>
        /// </summary>
        public virtual Chunk GetChunkAt(int3 position) =>
            Chunks.ContainsKey(position) ? Chunks[position] : null;
        
        /// <summary>
        /// The amount of command buffers.
        /// </summary>
        public int AmountOfCommandBuffers => ChunkCommandBuffers.Count;

        /// <summary>
        /// The amount of scheduled chunks.
        /// </summary>
        public int AmountOfScheduledChunks => ScheduledChunks.Count;

        /// <summary>
        /// Return a command buffer.
        /// </summary>
        public ChunkCommandBuffer GetChunkCommandBuffer() => 
            ChunkCommandBuffers.Dequeue();

        public void Dispose()
        {
            UsedChunks.Dispose();
        }
    }
}