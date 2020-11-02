using UnityEngine;

using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

namespace MinecraftLike
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
    public class BlockDebugger : MonoBehaviour
    {
        [System.Flags] 
        public enum FaceDirection : int
        {
            Back  = 1,
            East  = 2,
            Front = 4,
            West  = 8,
            North = 16,
            South = 32,
        }

        private Mesh _m;
        private MeshFilter _mf;
        private MeshCollider _mc;

        private Material _material;

        public FaceDirection activeFaces;
        public float size = 1f;
        public float3 position = 0f;

        private void Awake()
        {
            _mf = GetComponent<MeshFilter>();
            _mc = GetComponent<MeshCollider>();

            _material = Resources.Load<Material>("BlockMaterial");
            _m = new Mesh();
            _mf.mesh = _m;
            _mc.sharedMesh = _m;
            
            GetComponent<MeshRenderer>().material = _material;
        }

        private void Update()
        {
            var vertices = new NativeList<Vertex>(24, Allocator.TempJob);
            var triangles = new NativeList<Triangle>(18, Allocator.TempJob);

            var job0 = new BlockBuilderJob
            {
                position = position,
                size = size,
                activeFaces = (int)activeFaces,

                vertices = vertices.AsParallelWriter(),
                triangles = triangles.AsParallelWriter(),
            }.Schedule(6, 64);

            job0.Complete();

            _m.Update(triangles.AsArray(), vertices.AsArray());

            vertices.Dispose();
            triangles.Dispose();
        }
    }
}