using BlockyBits.src;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlockyBitsClient.src.Managers
{
    internal class ChunkManager
    {

        public static readonly Dictionary<Vector2, Chunk> Chunks = new();

        private static readonly ConcurrentQueue<Vector2> readyChunks = new();
        private static readonly Queue<Chunk> chunksToRemove = new();
        private static readonly List<Chunk> chunksToRegenerate = new();

        private static Vector2 lastCameraChunkPos;
        private static WorldGenerator worldGen = new(42);
        static bool updateChunks = true;

        public static void Start()
        {
            lastCameraChunkPos = Utils.WorldToChunkPosition(Game1.camera.Transform.GlobalPosition);
        }

        public static void Stop()
        {

        }

        private static void TryUnloadChunks()
        {
            if (chunksToRemove.TryDequeue(out Chunk chun))
            {
                if (chun.hasMesh)
                {
                    if (Chunks.Remove(chun.pos, out Chunk c))
                    {
                        c.Dispose();
                    }
                }
                else
                {
                    chunksToRemove.Enqueue(chun);
                }
            }
        }

        private static void ScheduleChunksForRemoval()
        {
            foreach (Chunk chunk in Chunks.Values)
            {
                int distanceX = (int)Math.Abs(lastCameraChunkPos.X - chunk.posX);
                int distanceY = (int)Math.Abs(lastCameraChunkPos.Y - chunk.posY);

                if (distanceX > Settings.renderDistance || distanceY > Settings.renderDistance && !chunk.isScheduledToUnload)
                {
                    chunk.isScheduledToUnload = true;
                    chunksToRemove.Enqueue(chunk);
                }
            }
        }

        private static void ScheduleNewChunks()
        {
            for (int i = (int)lastCameraChunkPos.X - Settings.renderDistance; i <= (int)lastCameraChunkPos.X + Settings.renderDistance; i++)
            {
                for (int j = (int)lastCameraChunkPos.Y - Settings.renderDistance; j <= (int)lastCameraChunkPos.Y + Settings.renderDistance; j++)
                {
                    if (Chunks.ContainsKey(new Vector2(i, j))) continue;
                    Chunk c = new Chunk(i, j);
                    Chunks.Add(c.pos, c);
                    Task.Run(() =>
                    {
                        c.SetBlockData(worldGen.GenerateBlockData(c.posX, c.posY));
                        Task.Yield();
                        c.GenerateMeshNoOptimization();
                        readyChunks.Enqueue(c.pos);
                    });
                }
            }
        }

        public static void UpdateChunks()
        {
            if (!updateChunks) return;

            TryUnloadChunks();

            Vector2 newChunkPos = Utils.WorldToChunkPosition(Game1.camera.Transform.GlobalPosition);
            if (newChunkPos != lastCameraChunkPos)
            {
                lastCameraChunkPos = newChunkPos;
                ScheduleChunksForRemoval();
                ScheduleNewChunks();
            }

            while(readyChunks.TryDequeue(out var pos))
            {
                Chunks[pos].hasMesh = true;
            }
        }

        public static bool PlaceBlockAt(Vector3 pos, Block block)
        {
            int x, z;
            Vector2 chunkPos = Utils.WorldToChunkPosition(pos);

            // Compute local block coordinates (ensuring positive values)
            x = (int)((pos.X % Chunk.width + Chunk.width) % Chunk.width);
            z = (int)((pos.Z % Chunk.depth + Chunk.depth) % Chunk.depth);

            if (Chunks.TryGetValue(chunkPos, out var chunk))
            {
                if (!chunk.HasBlockAt(new Vector3(x, (int)pos.Y, z)))
                {
                    chunk.SetBlock(new Vector3(x, (int)pos.Y, z), block);
                    return true;
                }
            }
            return false;
        }
        public static Block GetBlockAt(Vector3 pos)
        {
            int x, z;
            Vector2 chunkPos = Utils.WorldToChunkPosition(pos);

            // Compute local block coordinates (ensuring positive values)
            x = (int)((pos.X % Chunk.width + Chunk.width) % Chunk.width);
            z = (int)((pos.Z % Chunk.depth + Chunk.depth) % Chunk.depth);


            if (Chunks.TryGetValue(chunkPos, out var chunk))
            {
                if (chunk.GetBlocks().TryGetValue(new Vector3(x, (int)pos.Y, z), out Block block))
                {
                    return block;
                }
            }
            return null;
        }

        public static bool RemoveBlockAt(Vector3 pos)
        {
            int x, z;
            Vector2 chunkPos = Utils.WorldToChunkPosition(pos);

            // Compute local block coordinates (ensuring positive values)
            x = (int)((pos.X % Chunk.width + Chunk.width) % Chunk.width);
            z = (int)((pos.Z % Chunk.depth + Chunk.depth) % Chunk.depth);


            if (Chunks.TryGetValue(chunkPos, out var chunk))
            {
                chunk.RemoveBlockNotOptimized(new Vector3(x, (int)pos.Y, z));
                return true;
            }
            return false;
        }



        public static void RenderChunks()
        {
            BoundingFrustum frustum = new(Game1.camera.viewMatrix * Game1.camera.projectionMatrix);
            foreach (Chunk chunk in Chunks.Values)
            {
                if (frustum.Intersects(chunk.boundingBox) && chunk.hasMesh)
                {
                    chunk.Render();
                }
            }
        }

        public static void RenderWater()
        {
            BoundingFrustum frustum = new(Game1.camera.viewMatrix * Game1.camera.projectionMatrix);

            foreach (Chunk chunk in Chunks.Values)
            {
                if (frustum.Intersects(chunk.boundingBox) && chunk.hasMesh)
                {
                    chunk.RenderWater();
                }
            }
        }
    }
}






/*using BlockyBits.src;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlockyBitsClient.src.Managers
{
    internal class ChunkManager
    {
        public static ConcurrentDictionary<Vector2, Chunk> Chunks = new();
        private static ConcurrentQueue<ChunkMeshData> generatedMeshes = new();
        private static ConcurrentQueue<ChunkBlockData> generatedBlocks = new();
        private static ConcurrentQueue<Vector2> blocksQueue = new();
        private static ConcurrentQueue<Vector2> meshQueue = new();


        private static List<Chunk> newChunks = new();
        private static List<Chunk> chunksToRegenerate = new();
        private static Vector2 lastCameraChunkPos;
        private static WorldGenerator worldGen = new(42);
        private static Task ChunkLoader;
        private static CancellationTokenSource cancellationTokenSource;
        static bool updateChunks = true;

        private static readonly ReaderWriterLockSlim rwLock = new(LockRecursionPolicy.NoRecursion);

        private static readonly Vector3[] faceNormals = {
        new Vector3( 0,  0, -1), // Front
        new Vector3( 0,  0,  1), // Back
        new Vector3(-1,  0,  0), // Left
        new Vector3( 1,  0,  0), // Right
        new Vector3( 0,  1,  0), // Top
        new Vector3( 0, -1,  0)  // Bottom
    };

        private static readonly Vector3[,] faceVertices = {
        { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(0, 1, 0) }, // Front
        { new Vector3(1, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(1, 1, 1) }, // Back
        { new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 1) }, // Left
        { new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 0) }, // Right
        { new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1), new Vector3(0, 1, 1) }, // Top
        { new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 0, 0), new Vector3(0, 0, 0) },  // Bottom
    };




        private class ChunkBlockData
        {
            public Dictionary<Vector3, Block> blocks;
            public Vector2 pos;

            public ChunkBlockData(Vector2 pos, Dictionary<Vector3, Block> blocks)
            {
                this.pos = pos;
                this.blocks = blocks;
            }
        }

        private class ChunkMeshData
        {
            public Vector2 pos;
            public List<VertexPositionNormalTexture> texCoords = new();
            public List<VertexPositionNormalTexture> texCoordsWater = new();
            public List<int> indices = new();
            public List<int> indicesWater = new();

            public ChunkMeshData(Vector2 pos, List<VertexPositionNormalTexture> texCoords, List<VertexPositionNormalTexture> texCoordsWater, List<int> indices, List<int> indicesWater)
            {
                this.pos = pos;
                this.texCoords = texCoords;
                this.texCoordsWater = texCoordsWater;
                this.indices = indices;
                this.indicesWater = indicesWater;
            }
        }



        public static void Stop()
        {
            updateChunks = false;
            cancellationTokenSource.Cancel();
            ChunkLoader.Wait();
        }

        public static void Start()
        {
            lastCameraChunkPos = Utils.WorldToChunkPosition(Game1.camera.Transform.GlobalPosition);
            cancellationTokenSource = new CancellationTokenSource();
            ChunkLoader = Task.Run(() =>
            {
                ChunkThread(cancellationTokenSource.Token);
            });
        }

        private static void ChunkThread(CancellationToken token)
        {
            while (updateChunks && !token.IsCancellationRequested)
            {
                while(blocksQueue.TryDequeue(out var pos))
                {
                    ChunkBlockData data = new(pos, worldGen.GenerateBlockData((int)pos.X, (int)pos.Y));
                    generatedBlocks.Enqueue(data);
                }

                while(meshQueue.TryDequeue(out var pos))
                {
                    ChunkMeshData data = GenerateMeshData((int)pos.X, (int)pos.Y, Chunks[pos].GetBlocks());
                    generatedMeshes.Enqueue(data);
                }

                Task.Yield();
            }
        }

        private static void RequestBlockData(int x, int y)
        {
            blocksQueue.Enqueue(new Vector2(x,y));
        }

        private static void RequestMeshData(int x, int y)
        {
            meshQueue.Enqueue(new Vector2(x,y));
        }



        private static ChunkMeshData GenerateMeshData(int posX, int posY, Dictionary<Vector3, Block> blocks)
        {
            List<VertexPositionNormalTexture> texCoords = new();
            List<VertexPositionNormalTexture> texCoordsWater = new();
            List<int> indices = new();
            List<int> indicesWater = new();

            int highestPoint = 0;
            foreach (var pos in blocks.Keys)
            {
                Block block = blocks[pos];
                BlockInformation blockInfo = Block.GetBlockInformation(block.type);
                Vector2[] blockUVs = blockInfo.UVs;

                if (pos.Y > highestPoint) highestPoint = (int)pos.Y;

                for (int face = 0; face < 6; face++)
                {
                    Vector3 neighborPos = pos + faceNormals[face];
                    if (neighborPos.Y < 0) continue;
                    if (blocks.ContainsKey(neighborPos))
                    {
                        if (blocks.TryGetValue(neighborPos, out Block b))
                        {
                            if (b.isTransparent && block.isTransparent) continue;
                            if (!b.isTransparent) continue;
                        }

                    }
                    if (neighborPos.X < 0)
                    {
                        if (ChunkManager.Chunks.TryGetValue(new Vector2(posX - 1, posY), out Chunk chunk))
                        {
                            if (chunk.isLoaded)
                            {
                                if (chunk.GetBlocks().TryGetValue(new Vector3(Chunk.width - 1, pos.Y, pos.Z), out Block b))
                                {
                                    if (b.isTransparent && block.isTransparent) continue;
                                    if (!b.isTransparent) continue;
                                }
                            }
                        }
                        else continue;
                    }
                    if (neighborPos.Z < 0)
                    {
                        if (ChunkManager.Chunks.TryGetValue(new Vector2(posX, posY - 1), out Chunk chunk))
                        {
                            if (chunk.isLoaded)
                            {
                                if (chunk.GetBlocks().TryGetValue(new Vector3(pos.X, pos.Y, Chunk.depth - 1), out Block b))
                                {
                                    if (b.isTransparent && block.isTransparent) continue;
                                    if (!b.isTransparent) continue;
                                }
                            }
                        }
                        else continue;
                    }
                    if (neighborPos.X == Chunk.width)
                    {
                        if (ChunkManager.Chunks.TryGetValue(new Vector2(posX + 1, posY), out Chunk chunk))
                        {
                            if (chunk.isLoaded)
                            {
                                if (chunk.GetBlocks().TryGetValue(new Vector3(0, pos.Y, pos.Z), out Block b))
                                {
                                    if (b.isTransparent && block.isTransparent) continue;
                                    if (!b.isTransparent) continue;
                                }
                            }
                        }
                        else continue;
                    }
                    if (neighborPos.X == Chunk.depth)
                    {
                        if (ChunkManager.Chunks.TryGetValue(new Vector2(posX, posY + 1), out Chunk chunk))
                        {
                            if (chunk.isLoaded)
                            {
                                if (chunk.GetBlocks().TryGetValue(new Vector3(pos.X, pos.Y, 0), out Block b))
                                {
                                    if (b.isTransparent && block.isTransparent) continue;
                                    if (!b.isTransparent) continue;
                                }
                            }
                        }
                        else continue;
                    }

                    if (block.isTransparent)
                    {
                        int startIndex = texCoordsWater.Count;
                        Vector2 uv0 = blockUVs[face]; // Top-left UV of the face texture
                        Vector2 uv1 = uv0 + new Vector2(TextureAtlas.horizontalOffset, 0);
                        Vector2 uv2 = uv0 + new Vector2(TextureAtlas.horizontalOffset, TextureAtlas.verticalOffset);
                        Vector2 uv3 = uv0 + new Vector2(0, TextureAtlas.verticalOffset);

                        texCoordsWater.Add(new VertexPositionNormalTexture(pos + faceVertices[face, 0], faceNormals[face], uv0));
                        texCoordsWater.Add(new VertexPositionNormalTexture(pos + faceVertices[face, 1], faceNormals[face], uv1));
                        texCoordsWater.Add(new VertexPositionNormalTexture(pos + faceVertices[face, 2], faceNormals[face], uv2));
                        texCoordsWater.Add(new VertexPositionNormalTexture(pos + faceVertices[face, 3], faceNormals[face], uv3));

                        indicesWater.Add(startIndex);
                        indicesWater.Add(startIndex + 1);
                        indicesWater.Add(startIndex + 2);
                        indicesWater.Add(startIndex);
                        indicesWater.Add(startIndex + 2);
                        indicesWater.Add(startIndex + 3);
                    }
                    else
                    {
                        int startIndex = texCoords.Count;
                        Vector2 uv0 = blockUVs[face]; // Top-left UV of the face texture
                        Vector2 uv1 = uv0 + new Vector2(TextureAtlas.horizontalOffset, 0);
                        Vector2 uv2 = uv0 + new Vector2(TextureAtlas.horizontalOffset, TextureAtlas.verticalOffset);
                        Vector2 uv3 = uv0 + new Vector2(0, TextureAtlas.verticalOffset);
                        texCoords.Add(new VertexPositionNormalTexture(pos + faceVertices[face, 0], faceNormals[face], uv3));
                        texCoords.Add(new VertexPositionNormalTexture(pos + faceVertices[face, 1], faceNormals[face], uv2));
                        texCoords.Add(new VertexPositionNormalTexture(pos + faceVertices[face, 2], faceNormals[face], uv1));
                        texCoords.Add(new VertexPositionNormalTexture(pos + faceVertices[face, 3], faceNormals[face], uv0));

                        indices.Add(startIndex);
                        indices.Add(startIndex + 1);
                        indices.Add(startIndex + 2);
                        indices.Add(startIndex);
                        indices.Add(startIndex + 2);
                        indices.Add(startIndex + 3);
                    }
                }
            }

            return new ChunkMeshData(new Vector2(posX, posY), texCoords, texCoordsWater, indices, indicesWater);
        }



        public static void UpdateChunks()
        {

            while (generatedBlocks.TryDequeue(out var blockData))
            {
                Chunks[blockData.pos].SetBlockData(blockData.blocks);
                RequestMeshData((int)blockData.pos.X, (int)blockData.pos.Y);
            }

            while(generatedMeshes.TryDequeue(out var meshData))
            {
                Chunks[meshData.pos].SetMeshData(meshData.texCoords, meshData.texCoordsWater, meshData.indices, meshData.indicesWater);
                Chunks[meshData.pos].meshReady = true;
            }


            Vector2 newChunkPos = Utils.WorldToChunkPosition(Game1.camera.Transform.GlobalPosition);
            if (newChunkPos != lastCameraChunkPos)
            {
                lastCameraChunkPos = newChunkPos;

                for (int i = (int)newChunkPos.X - Settings.renderDistance; i <= (int)newChunkPos.X + Settings.renderDistance; i++)
                {
                    for (int j = (int)newChunkPos.Y - Settings.renderDistance; j <= (int)newChunkPos.Y + Settings.renderDistance; j++)
                    {
                        if (!updateChunks) return;
                        if (Chunks.ContainsKey(new Vector2(i, j)))
                        {
                            if (Math.Abs(newChunkPos.X - i) == Settings.renderDistance - 1 ||
                               Math.Abs(newChunkPos.Y - j) == Settings.renderDistance - 1)
                            {
                                if (Chunks.TryGetValue(new Vector2(i, j), out Chunk chunk))
                                {
                                    chunksToRegenerate.Add(chunk);
                                }
                            }
                            continue;
                        }
                        Chunks.TryAdd(new Vector2(i, j), new Chunk(i,j));
                        RequestBlockData(i, j);
                    }
                }
            }
        }

        public static void RenderChunks()
        {
            BoundingFrustum frustum = new(Game1.camera.viewMatrix * Game1.camera.projectionMatrix);

            foreach (Chunk chunk in Chunks.Values)
            {
                if (frustum.Intersects(chunk.boundingBox) && chunk.meshReady)
                {
                    chunk.Render();
                }

            }
        }

        public static void RenderWater()
        {
            BoundingFrustum frustum = new(Game1.camera.viewMatrix * Game1.camera.projectionMatrix);
            foreach (Chunk chunk in Chunks.Values)
            {
                if (frustum.Intersects(chunk.boundingBox) && chunk.meshReady)
                {
                    chunk.RenderWater();
                }
            }
        }
    }
}*/