using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

using Unicraft.Extensions.Mathematics;

namespace Unicraft.Core
{
    public class Endless : MonoBehaviour
    {
        public Transform target;
        public int2 chunksAround = new int2(8);

        private NativeHashSet<float2> _tiles;
        private Plane[] _planes;

        private void Awake()
        {
            _tiles = new NativeHashSet<float2>(1, Allocator.Persistent);
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            var amount = chunksAround.Amount();
            var center = ((float2)chunksAround / 2f) * GameManager.ChunkSize.xz;
            var targetPos = (float3)TargetPosition(target.transform.position);

            var keepChunks = new NativeList<float2>(amount, Allocator.Temp);
            _planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

            for(var i = 0; i < amount; i++)
            {
                var localPos = i.To2D(chunksAround) * GameManager.ChunkSize.xz;
                var worldPos = (int2)(localPos - center + targetPos.xz);

                var centeredLocalPos = (localPos - center + targetPos.xz) + (float2)GameManager.ChunkSize.xz / 2f;

                var aabb = GeometryUtility.TestPlanesAABB(_planes, new Bounds(new float3(centeredLocalPos.x, 0, centeredLocalPos.y), (float3)GameManager.ChunkSize * 2));
                
                if(!aabb)
                {
                    continue;
                }
                
                if(!_tiles.Contains(worldPos))
                {
                    keepChunks.Add(worldPos);
                    var chunkPos = new int3(worldPos.x, 0, worldPos.y);
                    GameManager.World.CreateChunk(chunkPos);
                    GameManager.World.UpdateChunk(chunkPos);
                }
                else
                {
                    keepChunks.Add(worldPos);
                    _tiles.Remove(worldPos);
                }
            }

            var destroyedTiles = _tiles.ToNativeArray(Allocator.Temp);
            for(var i = 0; i < destroyedTiles.Length; i++)
            {
                var worldPos = (int2)destroyedTiles[i];
                GameManager.World.DestroyChunk(new int3(worldPos.x, 0, worldPos.y));
                _tiles.Remove(destroyedTiles[i]);
            }
            
            if(_tiles.Count() > 0)
            {
                UnityEngine.Debug.Log("Clear Tiles");
                _tiles.Clear();
            }

            for(var i = 0; i < keepChunks.Length; i++)
                _tiles.Add(keepChunks[i]);
        }

        private void OnDrawGizmos()
        {
            
        }

        public int3 TargetPosition(in float3 targetPosition)
        {
            return (int3)math.round(targetPosition / GameManager.ChunkSize) * GameManager.ChunkSize;
        }

        private void OnDestroy()
        {
            _tiles.Dispose();
        }
    }
}