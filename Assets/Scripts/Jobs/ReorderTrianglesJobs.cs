using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

namespace MinecraftLike
{
    [BurstCompile]
    public struct ReorderTrianglesJob : IJobParallelFor
    {
        [ReadOnly]
        public int stride;

        [ReadOnly]
        public int max;

        public NativeArray<Triangle> triangles;

        public void Execute(int index)
        {
            triangles[index] += (index / stride) * max;
        }
    }
}