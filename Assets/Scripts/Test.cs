using UnityEngine;
using Unity.Mathematics;

namespace MinecraftLike
{
    public class Test : MonoBehaviour
    {
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.O))
            {

                for(var x = 0; x < 16; x++)
                {
                    for(var y = 0; y < 16; y++)
                    {
                        var pos = new int3(x, 0, y) * GameManager.ChunkSize;
                        if(GameManager.World.CreateChunk(at: pos))
                            // Construct it...
                            GameManager.World.ConstructChunk(at: pos);
                    }
                }
            }
        }
    }
}