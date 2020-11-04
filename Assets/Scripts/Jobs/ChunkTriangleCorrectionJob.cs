using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

namespace MinecraftLike
{
    [BurstCompile]
    public struct ChunkTriangleCorrectionJob : IJobParallelFor, IQuickJob
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<Triangle> triangles;

        public void Execute(int index)
        {
            var tIndex = index * 2;

            triangles[tIndex    ] += index * 4;
            triangles[tIndex + 1] += index * 4;
        }

        public JobHandle QuickSchedule(in JobHandle deps = default)
        {
            return new ChunkTriangleCorrectionJob
            {
                triangles = triangles,
            }.Schedule(triangles.Length / 2, 64, deps);
        }
    }
}