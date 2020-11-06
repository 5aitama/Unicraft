using UnityEngine;
using Unity.Mathematics;

using Unicraft.Core;

namespace MinecraftLike
{
    public class Test : MonoBehaviour
    {
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.O))
            {
                // The position of the chunk we want to create
                var position = new int3(0, 0, 0);

                // Create chunk...
                GameManager.World.CreateChunk(at: position);
                // Update it...
                GameManager.World.UpdateChunk(at: position);
                
                // And that's it :) you have created a chunk!!!!
            }

            if(Input.GetKeyDown(KeyCode.P))
            {
                for(var x = 0; x < 16; x++)
                {
                    for(var y = 0; y < 16; y++)
                    {
                        GameManager.World.CreateChunk(at: new int3(x, 0, y) * GameManager.ChunkSize);
                        GameManager.World.UpdateChunk(at: new int3(x, 0, y) * GameManager.ChunkSize);
                    }
                }

                GameManager.World.DestroyChunk(new int3(0, 0, 0));
                GameManager.World.CreateChunk(new int3(-1, 0, 0)*  GameManager.ChunkSize);
                GameManager.World.UpdateChunk(new int3(-1, 0, 0)*  GameManager.ChunkSize);
            }
        }
    }
}