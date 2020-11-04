using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;

namespace MinecraftLike
{
    [BurstCompile]
    public struct BlockBuilderJob : IJobParallelFor, IQuickJob
    {
        /// <summary>
        /// Active faces.
        /// </summary>
        [ReadOnly]
        public int activeFaces;

        /// <summary>
        /// Block size.
        /// </summary>
        [ReadOnly]
        public float3 size;

        /// <summary>
        /// Block position.
        /// </summary>
        [ReadOnly]
        public float3 position;

        /// <summary>
        /// Block vertices.
        /// </summary>
        [WriteOnly]
        public NativeList<Vertex>.ParallelWriter vertices;

        /// <summary>
        /// Block triangles
        /// </summary>
        [WriteOnly]
        public NativeList<Triangle>.ParallelWriter triangles;

        /// <summary>
        /// For triangle indices.
        /// </summary>
        private volatile int inc;

        public void Execute(int index)
        {
            var mask = 1 << index;

            if((activeFaces & mask) != mask)
                return;

            AddFaceVertices(index, position, size, ref vertices);
            AddFaceTriangles(inc, ref triangles);

            System.Threading.Interlocked.Increment(ref inc);
        }

        /// <summary>
        /// Add face vertex to a vertex NativeArray
        /// </summary>
        /// <param name="face">Face index (from 0 to 6)</param>
        /// <param name="faceVertexIndex">Index of the face vertex to add</param>
        /// <param name="pos">Block position</param>
        /// <param name="size">Block size</param>
        /// <param name="v">NativeList that store new vertex</param>
        public static void AddFaceVertices(in int face, in float3 pos, in float3 size, ref NativeList<Vertex>.ParallelWriter v)
        {
            var fIndex = face * 4;

            NativeArray<Vertex> vertices = new NativeArray<Vertex>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);


            vertices[0] = new Vertex 
            {
                pos  = BlockConstants.Vertices[BlockConstants.FacesIndices[fIndex    ]] * size + pos,
                norm = BlockConstants.FacesNormal[face],
                uv0  = BlockConstants.FaceUV[0],
            };

            vertices[1] = new Vertex 
            {
                pos  = BlockConstants.Vertices[BlockConstants.FacesIndices[fIndex + 1]] * size + pos,
                norm = BlockConstants.FacesNormal[face],
                uv0  = BlockConstants.FaceUV[1],
            };

            vertices[2] = new Vertex 
            {
                pos  = BlockConstants.Vertices[BlockConstants.FacesIndices[fIndex + 2]] * size + pos,
                norm = BlockConstants.FacesNormal[face],
                uv0  = BlockConstants.FaceUV[2],
            };

            vertices[3] = new Vertex 
            {
                pos  = BlockConstants.Vertices[BlockConstants.FacesIndices[fIndex + 3]] * size + pos,
                norm = BlockConstants.FacesNormal[face],
                uv0  = BlockConstants.FaceUV[3],
            };

            unsafe
            {
                var ptr = NativeArrayUnsafeUtility.GetUnsafePtr(vertices);
                v.AddRangeNoResize(ptr, 4);
            }
        }

        /// <summary>
        /// Add face triangles to a triangle NativeList.
        /// </summary>
        /// <param name="offset">Face Triangle index offset.</param>
        /// <param name="t">NativeList that store new triangles</param>
        public static void AddFaceTriangles(in int offset, ref NativeList<Triangle>.ParallelWriter t)
        {
            var triangles = new NativeArray<Triangle>(2, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            triangles[0] = new Triangle(0, 1, 2) + offset;
            triangles[1] = new Triangle(0, 2, 3) + offset;

            unsafe
            {
                var ptr = NativeArrayUnsafeUtility.GetUnsafePtr(triangles);
                t.AddRangeNoResize(ptr, 2);
            }
        }

        public JobHandle QuickSchedule(in JobHandle deps = default)
        {
            return new BlockBuilderJob
            {
                activeFaces = activeFaces,
                size        = size,
                position    = position,
                vertices    = vertices,
                triangles   = triangles,
            }.Schedule(6, 64, deps);
        }
    }
}