using Unity.Collections;
using Unity.Mathematics;

using Unicraft.Core.Jobs;
using Unicraft.Core.Geometry;
using Unicraft.Extensions.Mathematics;

namespace Unicraft.Core.Debugger
{
    public class ChunkDebugger : Debugger
    {
        /// <summary>
        /// Chunk size.
        /// </summary>
        public int3 size = new int3(16, 256, 16);

        void Update()
        {
            var blockAmount = (size + 2).Amount();

            var vertices  = new NativeList<Vertex>(blockAmount * 24, Allocator.TempJob);
            var triangles = new NativeList<Triangle>(blockAmount * 18, Allocator.TempJob);

            var blocks = new NativeArray<RawBlock>(blockAmount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            var job = new ChunkBlockGeneratorJob
            {
                position        = (int3)(float3)transform.position,
                size            = size,
                blocks          = blocks,
            }.QuickSchedule();

            job = new ChunkBuilderJob
            {
                size      = size,
                vertices  = vertices.AsParallelWriter(),
                triangles = triangles.AsParallelWriter(),
                blocks    = blocks,
            }
            .QuickSchedule(job);
            
            job.Complete();

            new ChunkTriangleCorrectionJob
            {
                triangles = triangles,
            }
            .QuickSchedule(job)
            .Complete();

            blocks.Dispose();

            _m.Update(triangles.AsArray(), vertices.AsArray());

            vertices.Dispose();
            triangles.Dispose();
        }
    }
}