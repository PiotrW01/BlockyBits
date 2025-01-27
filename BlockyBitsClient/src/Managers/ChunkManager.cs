using BlockyBits.src;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockyBitsClient.src.Managers
{
    internal class ChunkManager
    {
        public static Dictionary<Vector2, Chunk> chunks = new();
        //public static ConcurrentDictionary<Vector2, Chunk> chunks = new();
        //private static ConcurrentQueue<Chunk> chunkQueue = new();
        private static HashSet<Chunk> chunksToRemove = new();
        private static Vector2 lastCameraChunkPos;
        private static WorldGenerator worldGen = new(42);

        static bool updateChunks = true;

        public static void Start()
        {
            lastCameraChunkPos = Utils.WorldToChunkPosition(Game1.camera.Transform.GlobalPosition);
            //Shaders.WaterShader.Parameters["LightDirection"].SetValue(Vector3.Normalize(new Vector3(-0.5f, -1.0f, 0)));
            //Shaders.WaterShader.Parameters["LightColor"].SetValue(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));
            //Shaders.WaterShader.Parameters["AmbientColor"].SetValue(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
        }

        public static void RenderChunks()
        {
            BoundingFrustum frustum = new(Game1.camera.viewMatrix * Game1.camera.projectionMatrix);
            Game1.game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game1.game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            Game1.game.GraphicsDevice.BlendState = BlendState.Opaque;
            foreach (Chunk chunk in chunks.Values)
            {
                if (!frustum.Intersects(chunk.boundingBox)){
                    continue;
                }
                chunk.Render();
            }

        }

        public static void RenderWater()
        {
            BoundingFrustum frustum = new(Game1.camera.viewMatrix * Game1.camera.projectionMatrix);
            Game1.game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Game1.game.GraphicsDevice.DepthStencilState = new DepthStencilState()
            {
                DepthBufferEnable = true,   // Depth testing ON
                DepthBufferWriteEnable = false // Depth writing OFF
            };
            foreach (Chunk chunk in chunks.Values)
            {
                if (!frustum.Intersects(chunk.boundingBox))
                {
                    continue;
                }
                chunk.RenderWater();
            }
        }

        public static void UpdateChunks()
        {
            if (!updateChunks) return;
            Vector2 newChunkPos = Utils.WorldToChunkPosition(Game1.camera.Transform.GlobalPosition);
            if (newChunkPos != lastCameraChunkPos)
            {
                lastCameraChunkPos = newChunkPos;
                GenerateChunks();
/*                Task.Run(() =>
                {
                    Vector2 pos = lastCameraChunkPos;
                    GenerateChunks(pos);
                });*/
            }

/*            while (chunkQueue.TryDequeue(out Chunk chunk))
            {
                chunks.TryAdd(chunk.pos, chunk);
            }


            foreach (Chunk chunk in chunks.Values)
            {
                int distanceX = (int)Math.Abs(lastCameraChunkPos.X - chunk.posX);
                int distanceY = (int)Math.Abs(lastCameraChunkPos.Y - chunk.posY);

                if (distanceX > Settings.renderDistance || distanceY > Settings.renderDistance)
                {
                    chunksToRemove.Add(chunk);
                }
            }


            foreach (Chunk chunk in chunksToRemove)
            {
                chunk.Dispose();
                chunks.TryRemove(chunk.pos, out Chunk c);
            }
            chunksToRemove.Clear();*/
        }
        /*public static void GenerateChunks(Vector2 pos)
        {
            Debug.WriteLine($"chunk generation starting at: {pos}");
            for (int i = (int)pos.X - Settings.renderDistance; i <= (int)pos.X + Settings.renderDistance; i++)
            {
                for (int j = (int)pos.Y - Settings.renderDistance; j <= (int)pos.Y + Settings.renderDistance; j++)
                {
                    if (chunks.ContainsKey(new Vector2(i, j))) continue;
                    Chunk c = worldGen.GenerateChunk(i, j);
                    c.GenerateMesh();
                    chunkQueue.Enqueue(c);
                }
            }
        }*/

        public static void GenerateChunks()
        {
            lastCameraChunkPos = Utils.WorldToChunkPosition(Game1.camera.Transform.GlobalPosition);
            Debug.WriteLine($"chunk generation starting at: {lastCameraChunkPos}");
            for (int i = (int)lastCameraChunkPos.X - Settings.renderDistance; i <= (int)lastCameraChunkPos.X + Settings.renderDistance; i++)
            {
                for (int j = (int)lastCameraChunkPos.Y - Settings.renderDistance; j <= (int)lastCameraChunkPos.Y + Settings.renderDistance; j++)
                {
                    if (chunks.ContainsKey(new Vector2(i, j))) continue;
                    chunks.Add(new Vector2(i, j), worldGen.GenerateChunk(i, j));
                }
            }


            Debug.WriteLine($"Chunks generated: {chunks.Count}");
            foreach (Chunk chunk in chunks.Values)
            {
                int distanceX = (int)Math.Abs(lastCameraChunkPos.X - chunk.posX);
                int distanceY = (int)Math.Abs(lastCameraChunkPos.Y - chunk.posY);

                if (distanceX > Settings.renderDistance || distanceY > Settings.renderDistance)
                {
                    chunksToRemove.Add(chunk);
                }
                else if (distanceX == Settings.renderDistance - 1 || distanceY == Settings.renderDistance - 1)
                {
                    chunk.GenerateMesh();
                }

                if (chunk.vertexCount == 0)
                {
                    chunk.GenerateMesh();
                }
            }

            foreach (Chunk chunk in chunksToRemove)
            {
                chunk.Dispose();
                chunks.Remove(chunk.pos);
            }
            chunksToRemove.Clear();
            updateChunks = false;
        }
    }
}
