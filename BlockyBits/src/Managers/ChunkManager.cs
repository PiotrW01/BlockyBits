using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockyBitsClient.src.Managers
{
    internal class ChunkManager
    {
        public static Dictionary<Vector2, Chunk> chunks = new();
        

        public static void RenderChunks()
        {
            foreach (Chunk chunk in chunks.Values)
            {
                Game1.game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                chunk.Render();
            }
        }

    }
}
