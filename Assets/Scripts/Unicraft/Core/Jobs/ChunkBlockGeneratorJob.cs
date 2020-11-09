using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;

using Unicraft.Extensions.Mathematics;

namespace Unicraft.Core.Jobs
{

    public struct ChunkBlockGeneratorJob : IJobParallelFor
    {
        /// <summary>
        /// NativeArray that contain generated blocks.
        /// </summary>
        [NativeDisableParallelForRestriction]
        public NativeArray<RawBlock> blocks;

        /// <summary>
        /// Chunk size
        /// </summary>
        [ReadOnly]
        public int3 size;

        /// <summary>
        /// Chunk world position
        /// </summary>
        [ReadOnly]
        public int3 position;

        private int3 RawSize => size + 2;

        public void Execute(int index)
        {
            var localPos = index.To3D(RawSize);
            var worldPos = localPos + position;

            var minEdges = localPos == 0;
            var maxEdges = localPos == RawSize - 1;

            var value = 0f;

            var pos = (float2)(localPos + position).xz;
            var n1 = ((1f + noise.snoise((float3)(localPos + position) * 0.025f)) / 2f) * 6f;
            var n = (1f + noise.snoise(pos * 0.01f)) * .5f * 32;
            value = n - worldPos.y - n1;

            // if(minEdges.x || minEdges.y || minEdges.z || maxEdges.x || maxEdges.y || maxEdges.z)
            //     value = -1;
            
            blocks[index] = new RawBlock
            {
                type = 0,
                value = value,
            };
        }

        public JobHandle QuickSchedule(in JobHandle deps = default)
        {
            return new ChunkBlockGeneratorJob
            {
                blocks          = blocks,
                size            = size,
                position        = position,
            }.Schedule(RawSize.Amount(), 32, deps);
        }
    }
}