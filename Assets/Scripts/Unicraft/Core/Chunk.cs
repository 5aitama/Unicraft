using UnityEngine;

using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

using Unicraft.Core.Jobs;
using Unicraft.Core.Geometry;
using Unicraft.Extensions.Mathematics;

using System.Runtime.InteropServices;


namespace Unicraft.Core
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer))]
    public class Chunk : MonoBehaviour, System.IDisposable
    {
        private Mesh _mesh;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;

        private NativeList<Vertex> _vertices;
        private NativeList<Triangle> _triangles;
        private NativeArray<RawBlock> _blocks;

        public bool save = false;
        public bool load = false;

        public bool IsBlocksInitialized { get; set; }

        private void Awake()
        {
            _mesh = new Mesh();
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();

            _meshFilter.mesh = _mesh;
            _meshCollider.sharedMesh = _mesh;
            
            var blockAmount = (GameManager.ChunkSize + 2).Amount();
            if(_blocks.IsCreated)
                _blocks.Dispose();
            
            _blocks = new NativeArray<RawBlock>(blockAmount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            GetComponent<MeshRenderer>().material = GameManager.ChunkMaterial;
        }

        public void Update()
        {
            if(save)
            {
                SaveData();
                save = false;
            }

            if(Input.GetKeyDown(KeyCode.U) || load)
            {
                LoadData();
                load = false;
            }
        }

        public void SaveData()
        {
            using (var fs = new System.IO.FileStream(this.name + ".chunk", System.IO.FileMode.OpenOrCreate)) {
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(fs, _blocks.ToArray());
            }
        }

        public void LoadData()
        {
            if(!System.IO.File.Exists(this.name + ".chunk"))
                return;
            using (var fs = new System.IO.FileStream(this.name + ".chunk", System.IO.FileMode.Open)) {
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                var o = bf.Deserialize(fs);
                NativeArray<RawBlock> _loadedBlocks;
                unsafe
                {
                    var handle = GCHandle.Alloc(o, GCHandleType.Pinned);

                    var safetyHandle = Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle.Create();
                    _loadedBlocks = Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<RawBlock>(handle.AddrOfPinnedObject().ToPointer(), _blocks.Length, Allocator.None);
                    Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref _loadedBlocks, safetyHandle);
                    
                    Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle.SetAllowReadOrWriteAccess(safetyHandle, true);
                    Debug.Log(_loadedBlocks[0] + " ; " + _blocks[0]);
                    _blocks.CopyFrom(_loadedBlocks);
                }

                // _blocks.CopyFrom(_loadedBlocks);
            }
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

            _vertices  = new NativeList<Vertex>(blockAmount * 24, Allocator.Persistent);
            _triangles = new NativeList<Triangle>(blockAmount * 18, Allocator.Persistent);

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
            
            UpdateCollider();
        }

        public void Dispose()
        {
            if(_vertices.IsCreated)
                _vertices.Dispose();

            if(_triangles.IsCreated)
                _triangles.Dispose();

            if(_blocks.IsCreated)
                _blocks.Dispose();
        }

        public void ClearMesh()
        {
            _mesh.Clear();
        }

        public void UpdateCollider()
        {
            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = _mesh;
        }
    }
}