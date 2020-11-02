using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace MinecraftLike
{
    [BurstCompile]
    public struct BlockBuilderJob : IJobParallelFor
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
        private int inc;

        public void Execute(int index)
        {
            var mask = 1 << index;

            if((activeFaces & mask) != mask)
                return;

            var faceIndex = index * 4;

            for(var i = 0; i < 4; i++)
            {
                vertices.AddNoResize(new Vertex 
                {
                    pos  = BlockConstants.Vertices[BlockConstants.FacesIndices[faceIndex + i]] * size + position,
                    norm = BlockConstants.FacesNormal[index],
                    uv0  = BlockConstants.FaceUV[i],
                });
            }
            
            triangles.AddNoResize(new Triangle(0, 1, 2) + inc * 4);
            triangles.AddNoResize(new Triangle(0, 2, 3) + inc * 4);

            System.Threading.Interlocked.Increment(ref inc);
        }
    }
}