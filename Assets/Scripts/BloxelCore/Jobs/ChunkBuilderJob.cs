using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

using MinecraftLike.Extensions.Mathematics;

namespace MinecraftLike
{
    public struct ChunkBuilderJob : IJobParallelFor, IQuickJob
    {
        /// <summary>
        /// Chunk size.
        /// </summary>
        [ReadOnly]
        public int3 size;

        /// <summary>
        /// Blocks
        /// </summary>
        [ReadOnly]
        public NativeArray<RawBlock> blocks;

        /// <summary>
        /// Chunk vertices
        /// </summary>
        [WriteOnly]
        public NativeList<Vertex>.ParallelWriter vertices;

        /// <summary>
        /// Chunk triangles
        /// </summary>
        [WriteOnly]
        public NativeList<Triangle>.ParallelWriter triangles;

        public int3 RawSize => size + 2;

        public void Execute(int index)
        {
            var localPos = index.To3D(RawSize);
            
            var minEdges = localPos == 0;
            var maxEdges = localPos >= RawSize - 1;

            if(minEdges.x|| minEdges.y|| minEdges.z|| maxEdges.x|| maxEdges.y|| maxEdges.z)
                return;

            if(blocks[index].value <= 0)
                return;

            for(var faceIndex = 0; faceIndex < 6; faceIndex++)
            {
                var dir = BlockConstants.FaceDirections[faceIndex];

                if(blocks[(localPos + dir).To1D(RawSize)].value > 0)
                    continue;

                BlockBuilderJob.AddFaceVertices(faceIndex, localPos - 1, 1f, ref vertices);
                BlockBuilderJob.AddFaceTriangles(0, ref triangles);
            }
        }

        public JobHandle QuickSchedule(in JobHandle deps = default)
        {
            return new ChunkBuilderJob
            {
                size = size,
                blocks = blocks,
                vertices = vertices,
                triangles = triangles,
            }.Schedule(RawSize.Amount(), 1, deps);
        }
    }
}