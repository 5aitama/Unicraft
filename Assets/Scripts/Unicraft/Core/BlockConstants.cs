using Unity.Mathematics;

namespace Unicraft.Core
{
    /// <summary>
    /// Class that contains stuff to create a block.
    /// </summary>
    public static class BlockConstants
    {
        /// <summary>
        /// Array that contains vertices of a block with size of 1
        /// at (0, 0, 0).
        /// </summary>
        public static readonly float3[] Vertices = new float3[8]
        {
            // Vertices of the face at Z-
            new float3(-.5f, -.5f, -.5f),
            new float3(-.5f,  .5f, -.5f),
            new float3( .5f,  .5f, -.5f),
            new float3( .5f, -.5f, -.5f),

            // Vertices of the face at Z+
            new float3(-.5f, -.5f,  .5f),
            new float3(-.5f,  .5f,  .5f),
            new float3( .5f,  .5f,  .5f),
            new float3( .5f, -.5f,  .5f),
        };

        /// <summary>
        /// Indices for each faces of a block.
        /// </summary>
        public static readonly int[] FacesIndices = new int[24]
        {
            // Face at Z-
            0, 1, 2, 3,

            // Face at X+
            3, 2, 6, 7,

            // Face at Z+
            7, 6, 5, 4,

            // Face at X-
            4, 5, 1, 0,
            
            // Face at Y+
            1, 5, 6, 2,

            // Face at Y-
            4, 0, 3, 7,
        };

        /// <summary>
        /// Normal for each face of a block.
        /// </summary>
        public static readonly float3[] FacesNormal = new float3[]
        {
            // Face Z-
            new float3( 0,  0, -1),

            // Face X+
            new float3( 1,  0,  0),

            // Face Z+
            new float3( 0,  0,  1),

            // Face X-
            new float3(-1,  0,  0),

            // Face Y+
            new float3( 0,  1,  0),

            // Face Y-
            new float3( 0, -1,  0),
        };

        /// <summary>
        /// UV coordinate for a block face.
        /// </summary>
        /// <value></value>
        public static readonly float2[] FaceUV = new float2[] 
        {
            // Top bottom
            new float2(0, 0),

            // Top left
            new float2(0, 1),

            // Top right
            new float2(1, 1),
            
            // Bottom right
            new float2(1, 0),
        };

        /// <summary>
        /// Array that contains face direction in the right order.
        /// Correspond to normals but integers for cpu otpimization.
        /// </summary>
        public static readonly int3[] FaceDirections = new int3[]
        {
            new int3( 0,  0, -1),
            new int3( 1,  0,  0),
            new int3( 0,  0,  1),
            new int3(-1,  0,  0),
            new int3( 0,  1,  0),
            new int3( 0, -1,  0),
        };
    }
}