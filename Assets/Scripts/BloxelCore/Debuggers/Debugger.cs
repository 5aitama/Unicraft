using UnityEngine;

namespace MinecraftLike
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
    public abstract class Debugger : MonoBehaviour
    {
        protected Mesh _m;
        protected MeshFilter _mf;
        protected MeshCollider _mc;

        protected Material _material;

        private void Awake()
        {
            _mf = GetComponent<MeshFilter>();
            _mc = GetComponent<MeshCollider>();

            _material = GameManager.ChunkMaterial;
            _m = new Mesh();
            _mf.mesh = _m;
            _mc.sharedMesh = _m;
            
            GetComponent<MeshRenderer>().material = _material;
        }
    }
}