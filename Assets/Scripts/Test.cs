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
                if(GameManager.World.CreateChunk(at: position))
                    // Construct it...
                    GameManager.World.ConstructChunk(at: position);
                
                // And that's it :) you have created a chunk!!!!
            }

            if(Input.GetKeyDown(KeyCode.P))
                for(var x = 0; x < 16; x++)
                    for(var y = 0; y < 16; y++)
                        if(GameManager.World.CreateChunk(at: new int3(x, 0, y) * GameManager.ChunkSize))
                            GameManager.World.ConstructChunk(at: new int3(x, 0, y) * GameManager.ChunkSize);
        }
    }
}