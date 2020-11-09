using Unity.Collections;
using Unity.Mathematics;

using Unicraft.Core.Jobs;
using Unicraft.Core.Geometry;

namespace Unicraft.Core.Debugger
{
    public class BlockDebugger : Debugger
    {
        [System.Flags] 
        public enum FaceDirection : int
        {
            Back  =  1,
            East  =  2,
            Front =  4,
            West  =  8,
            North = 16,
            South = 32,
        }

        public FaceDirection activeFaces;
        public float size = 1f;
        public float3 position = 0f;

        private void Update()
        {
            var vertices = new NativeList<Vertex>(24, Allocator.TempJob);
            var triangles = new NativeList<Triangle>(18, Allocator.TempJob);

            new BlockBuilderJob
            {
                position    = position,
                size        = size,
                activeFaces = (int)activeFaces,
                vertices    = vertices.AsParallelWriter(),
                triangles   = triangles.AsParallelWriter(),
            }
            .QuickSchedule()
            .Complete();

            _m.Update(triangles.AsArray(), vertices.AsArray());

            vertices.Dispose();
            triangles.Dispose();
        }
    }
}