using Unity.Mathematics;

namespace Unicraft.Core.Geometry
{
    /// <summary>
    /// Triangle representation.
    /// </summary>
    public struct Triangle
    {
        /// <summary>
        /// 3 indices that formed triangle.
        /// </summary>
        public int3 indices;

        /// <summary>
        /// Create new triangle
        /// </summary>
        /// <param name="indices">indices</param>
        public Triangle(int3 indices) => this.indices = indices;

        /// <summary>
        /// Create new triangle.
        /// </summary>
        /// <param name="a">1st index</param>
        /// <param name="b">2nd index</param>
        /// <param name="c">3rd index</param>
        public Triangle(int a, int b, int c) => this.indices = new int3(a, b, c);

        /// <summary>
        /// Create new triangle.
        /// </summary>
        /// <remarks>
        /// The first index is equal to <paramref name="s"/>
        /// and 2nd index is equal to <c>s + 1</c>
        /// and 3rd index is equal to <c>s + 2</c>
        /// </remarks>
        /// <param name="s">Value for 1st index</param>
        public Triangle(int s) => this.indices = new int3(s, s + 1, s + 2);

        /// <summary>
        /// Get triangle index.
        /// </summary>
        public int this[int index]
        {
            get => indices[index];
            set => indices[index] = value;
        }

        public static Triangle operator + (Triangle lhs, int rhs)
            => new Triangle(lhs.indices + rhs);

        public static Triangle operator + (Triangle lhs, int3 rhs)
            => new Triangle(lhs.indices + rhs);

        public static Triangle operator + (Triangle lhs, Triangle rhs)
            => new Triangle(lhs.indices + rhs.indices);

        public static Triangle operator - (Triangle lhs, int rhs)
            => new Triangle(lhs.indices - rhs);

        public static Triangle operator - (Triangle lhs, int3 rhs)
            => new Triangle(lhs.indices - rhs);

        public static Triangle operator - (Triangle lhs, Triangle rhs)
            => new Triangle(lhs.indices - rhs.indices);

        public static Triangle operator * (Triangle lhs, int rhs)
            => new Triangle(lhs.indices * rhs);

        public static Triangle operator * (Triangle lhs, int3 rhs)
            => new Triangle(lhs.indices * rhs);

        public static Triangle operator * (Triangle lhs, Triangle rhs)
            => new Triangle(lhs.indices * rhs.indices);

        public static Triangle operator / (Triangle lhs, int rhs)
            => new Triangle(lhs.indices / rhs);

        public static Triangle operator / (Triangle lhs, int3 rhs)
            => new Triangle(lhs.indices / rhs);
            
        public static Triangle operator / (Triangle lhs, Triangle rhs)
            => new Triangle(lhs.indices / rhs.indices);
    }
}