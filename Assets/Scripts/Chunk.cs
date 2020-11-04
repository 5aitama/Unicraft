using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

using MinecraftLike.Extensions.Mathematics;
using Unity.Jobs;

namespace MinecraftLike
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer))]
    public class Chunk : MonoBehaviour 
    {
        private Mesh _mesh;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;

        private NativeList<Vertex> _vertices;
        private NativeList<Triangle> _triangles;
        private NativeArray<RawBlock> _blocks;

        public bool IsBlocksInitialized { get; set; }

        private void Awake()
        {
            _mesh = new Mesh();
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();

            _meshFilter.mesh = _mesh;
            _meshCollider.sharedMesh = _mesh;
            
            var blockAmount = (GameManager.ChunkSize + 2).Amount();
            _blocks = new NativeArray<RawBlock>(blockAmount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

            GetComponent<MeshRenderer>().material = GameManager.ChunkMaterial;
        }

        public JobHandle ScheduleInitializeBlocks(in JobHandle inputDeps = default)
        {
            return new ChunkBlockGeneratorJob
            {
                position        = (int3)(float3)transform.position,
                size            = GameManager.ChunkSize,
                blocks          = _blocks,
            }.QuickSchedule();
        }

        public JobHandle ScheduleConstructGeometry(in JobHandle inputDeps = default)
        {
            var blockAmount = (GameManager.ChunkSize + 2).Amount();

            _vertices  = new NativeList<Vertex>(blockAmount * 24, Allocator.TempJob);
            _triangles = new NativeList<Triangle>(blockAmount * 18, Allocator.TempJob);

            return new ChunkBuilderJob
            {
                size      = GameManager.ChunkSize,
                vertices  = _vertices.AsParallelWriter(),
                triangles = _triangles.AsParallelWriter(),
                blocks    = _blocks,
            }
            .QuickSchedule(inputDeps);
        }

        public void FinalizeConstructGeometry()
        {

            new ChunkTriangleCorrectionJob
            {
                triangles = _triangles,
            }
            .QuickSchedule()
            .Complete();

            _mesh.Update(_triangles.AsArray(), _vertices.AsArray());

            _vertices.Dispose();
            _triangles.Dispose();
        }

        void OnDestroy()
        {
            _blocks.Dispose();
        }
    }
}