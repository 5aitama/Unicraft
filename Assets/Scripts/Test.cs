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
                // The position of the chunk we want to create
                var position = new int3(0, 0, 0);

                // Create chunk...
                if(GameManager.World.CreateChunk(at: position))
                    // Construct it...
                    GameManager.World.ConstructChunk(at: position);
                
                // And that's it :) you have create a chunk!!!!
            }
        }
    }
}