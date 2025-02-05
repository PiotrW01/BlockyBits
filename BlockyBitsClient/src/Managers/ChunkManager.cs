using ABI.System.Collections.Generic;
using BlockyBits.src;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BlockyBitsClient.src.Managers
{
    public class ChunkManager
    {

        public static readonly Dictionary<Vector2, Chunk> Chunks = new();
        private static ConcurrentQueue<MeshDataRequest> meshesToGenerate = new();

        private static readonly Queue<Chunk> chunksToRemove = new();
        private static readonly ConcurrentQueue<BlockDataResponse> chunkDataReady = new();
        private static readonly ConcurrentQueue<MeshDataResponse> meshDataReady = new();
        private static readonly ConcurrentQueue<BlockDataRequest> chunksToGenerate = new();

        private static Vector2 lastCameraChunkPos;
        public static WorldGenerator worldGen = new(42);
        static bool updateChunks = true;

        public static void Start()
        {
            lastCameraChunkPos = Utils.WorldToChunkPosition(Game1.MainCamera.Transform.GlobalPosition);
            Task.Run(async () =>
            {
                while (updateChunks)
                {
                    if(chunksToGenerate.TryDequeue(out var req))
                    {
                        var blocks = worldGen.GenerateBlockData((int)req.Position.X, (int)req.Position.Y);
                        chunkDataReady.Enqueue(new(req.Position, req.Timestamp, blocks));
                    }
                    await Task.Delay(1);
                }
            });

            Task.Run(async () =>
            {
                while (updateChunks)
                {
                    if (meshesToGenerate.TryDequeue(out var req))
                    {


                        MeshData meshData = GenerateMeshNoOptimization(req.blockData);
                        meshDataReady.Enqueue(new(meshData, req.Position, req.Timestamp));
                    }
                    await Task.Delay(1);
                }
            });
        }
        public static void Stop()
        {
            updateChunks = false;
        }
        public static void UpdateChunks()
        {
            Vector2 newChunkPos = Utils.WorldToChunkPosition(Game1.MainCamera.Transform.GlobalPosition);
            TryUnloadChunks();
            if (newChunkPos != lastCameraChunkPos)
            {
                lastCameraChunkPos = newChunkPos;
                ScheduleChunksForRemoval();
                ScheduleNewChunks();
            }

            // Update Block Data
            while(chunkDataReady.TryDequeue(out var res))
            {
                if(Chunks.TryGetValue(res.Position, out var chunk))
                {
                    if(chunk.lastBlocksUpdate < res.Timestamp)
                    {
                        chunk.lastBlocksUpdate = res.Timestamp;
                        chunk.SetBlockData(res.blocks);
                        chunk.isProcessed = true;
                        meshesToGenerate.Enqueue(new(res.Position, res.blocks, res.Timestamp));
                    }
                }
            }

            // Update Mesh
            while(meshDataReady.TryDequeue(out var res))
            {
                if (Chunks.TryGetValue(res.Position, out var chunk))
                {
                    if (chunk.lastMeshUpdate < res.Timestamp)
                    {
                        chunk.lastMeshUpdate = res.Timestamp;
                        chunk.SetMeshData(res.MeshData);
                        chunk.isProcessed = false;
                    }
                }
            }
        }
        private static void TryUnloadChunks()
        {
            while (chunksToRemove.TryDequeue(out Chunk chun))
            {
                int distanceX = (int)Math.Abs(lastCameraChunkPos.X - chun.posX);
                int distanceY = (int)Math.Abs(lastCameraChunkPos.Y - chun.posY);

                if (distanceX > Settings.renderDistance && distanceY > Settings.renderDistance && !chun.isProcessed)
                {
                    if (Chunks.Remove(chun.pos, out Chunk c))
                    {
                        c.Dispose();
                    }
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
                    Chunks.TryAdd(c.pos, c);
                    chunksToGenerate.Enqueue(new(c.pos, Game1.game.elapsedTime));
                }
            }
        }

        public static bool PlaceBlockAt(Vector3 pos, Block block)
        {
            int x, z;
            Vector2 chunkPos = Utils.WorldToChunkPosition(pos);

            // Compute local block coordinates (ensuring positive values)
            x = (int)((pos.X % Chunk.width + Chunk.width) % Chunk.width);
            z = (int)((pos.Z % Chunk.width + Chunk.width) % Chunk.width);

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
            z = (int)((pos.Z % Chunk.width + Chunk.width) % Chunk.width);


            if (Chunks.TryGetValue(chunkPos, out var chunk))
            {
                if (chunk.GetBlocks().TryGetValue(new Vector3(x, (int)pos.Y, z), out Block block))
                {
                    return block;
                }
            }
            return null;
        }

        public static bool GetBlockAt(Vector3 pos, out Block b)
        {
            int x, z;
            Vector2 chunkPos = Utils.WorldToChunkPosition(pos);

            // Compute local block coordinates (ensuring positive values)
            x = (int)((pos.X % Chunk.width + Chunk.width) % Chunk.width);
            z = (int)((pos.Z % Chunk.width + Chunk.width) % Chunk.width);


            if (Chunks.TryGetValue(chunkPos, out var chunk))
            {
                if (chunk.GetBlocks().TryGetValue(new Vector3(x, (int)pos.Y, z), out Block block))
                {
                    b = block;
                    return true;
                }
            }
            b = null;
            return false;
        }

        public static bool RemoveBlockAt(Vector3 pos)
        {
            int x, z;
            Vector2 chunkPos = Utils.WorldToChunkPosition(pos);

            // Compute local block coordinates (ensuring positive values)
            x = (int)((pos.X % Chunk.width + Chunk.width) % Chunk.width);
            z = (int)((pos.Z % Chunk.width + Chunk.width) % Chunk.width);

            if (Chunks.TryGetValue(chunkPos, out var chunk))
            {
                if(chunk.GetBlocks().TryGetValue(new Vector3(x, (int)pos.Y, z), out Block b))
                {
                    chunk.RemoveBlockNotOptimized(new Vector3(x, (int)pos.Y, z));
                    ObjectManager.CreateDroppedItem(new("dirt"), pos + Vector3.One * 0.5f);
                    return true;
                }
            }
            return false;
        }

        public static void RenderChunks()
        {
            BoundingFrustum frustum = new(Game1.MainCamera.viewMatrix * Game1.MainCamera.projectionMatrix);

            foreach (Chunk chunk in Chunks.Values)
            {
                if (frustum.Intersects(chunk.boundingBox))
                {
                    chunk.Render();
                }
            }
        }

        public static void RenderWater()
        {
            BoundingFrustum frustum = new(Game1.MainCamera.viewMatrix * Game1.MainCamera.projectionMatrix);

            foreach (Chunk chunk in Chunks.Values)
            {
                if (frustum.Intersects(chunk.boundingBox))
                {
                    chunk.RenderWater();
                }
            }
        }

        public static MeshData GenerateMeshNoOptimization(Dictionary<Vector3, Block> blocks)
        {

            List<VertexPositionNormalTexture> texCoords = new();
            List<VertexPositionNormalTexture> texCoordsWater = new();
            List<int> indices = new();
            List<int> indicesWater = new();
            int vertexCount = texCoords.Count;
            int waterVertexCount = texCoordsWater.Count;
            int indexCount = indices.Count;
            int waterIndexCount = indicesWater.Count;

            int highestPoint = 0;
            foreach (var pos in blocks.Keys)
            {
                Block block = blocks[pos];
                BlockProperties blockProps = Block.GetBlockProperties(block.ID);
                ModelShape shape = blockProps.ModelShape;
                if (pos.Y > highestPoint) highestPoint = (int)pos.Y;
                if (shape.BlockCount > 1)
                    Debug.WriteLine(shape.BlockCount);
                for (int v = 0; v < shape.BlockCount; v++)
                {
                    for (int face = 0; face < 6; face++)
                    {
                        Vector3 neighborPos = pos + faceNormals[face];
                        if (neighborPos.Y < 0) continue;
                        if (blocks.ContainsKey(neighborPos))
                        {
                            if (blocks.TryGetValue(neighborPos, out Block b))
                            {
                                var props = Block.GetBlockProperties(b.ID);
                                if (props.IsTransparent && blockProps.IsTransparent) continue;
                                if (!props.IsTransparent) continue;
                            }
                        }

                        if (blockProps.IsTransparent)
                        {
                            int startIndex = texCoordsWater.Count;
                            texCoordsWater.Add(new VertexPositionNormalTexture(pos + shape.vPositions[face * 4 + v * 24], shape.vNormals[face * 4 + v * 24], shape.vTextureUV[face * 4 + v * 24]));
                            texCoordsWater.Add(new VertexPositionNormalTexture(pos + shape.vPositions[face * 4 + 1 + v * 24], shape.vNormals[face * 4 + 1 + v * 24], shape.vTextureUV[face * 4 + 1 + v * 24]));
                            texCoordsWater.Add(new VertexPositionNormalTexture(pos + shape.vPositions[face * 4 + 2 + v * 24], shape.vNormals[face * 4 + 2 + v * 24], shape.vTextureUV[face * 4 + 2 + v * 24]));
                            texCoordsWater.Add(new VertexPositionNormalTexture(pos + shape.vPositions[face * 4 + 3 + v * 24], shape.vNormals[face * 4 + 3 + v * 24], shape.vTextureUV[face * 4 + 3 + v * 24]));

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
                            texCoords.Add(new VertexPositionNormalTexture(pos + shape.vPositions[face * 4 + v * 24], shape.vNormals[face * 4 + v * 24], shape.vTextureUV[face * 4 + v * 24]));
                            texCoords.Add(new VertexPositionNormalTexture(pos + shape.vPositions[face * 4 + 1 + v * 24], shape.vNormals[face * 4 + 1 + v * 24], shape.vTextureUV[face * 4 + 1 + v * 24]));
                            texCoords.Add(new VertexPositionNormalTexture(pos + shape.vPositions[face * 4 + 2 + v * 24], shape.vNormals[face * 4 + 2 + v * 24], shape.vTextureUV[face * 4 + 2 + v * 24]));
                            texCoords.Add(new VertexPositionNormalTexture(pos + shape.vPositions[face * 4 + 3 + v * 24], shape.vNormals[face * 4 + 3 + v * 24], shape.vTextureUV[face * 4 + 3 + v * 24]));

                            indices.Add(startIndex);
                            indices.Add(startIndex + 1);
                            indices.Add(startIndex + 2);
                            indices.Add(startIndex);
                            indices.Add(startIndex + 2);
                            indices.Add(startIndex + 3);
                        }
                    }
                }


            }
            vertexCount = texCoords.Count;
            waterVertexCount = texCoordsWater.Count;
            indexCount = indices.Count;
            waterIndexCount = indicesWater.Count;

            if (vertexCount == 0 || indexCount == 0) return null;

            return new(texCoords.Concat(texCoordsWater).ToArray(), indices.Concat(indicesWater).ToArray(), vertexCount, waterVertexCount, indexCount, waterIndexCount);
        }

        private static readonly Vector3[] faceNormals = {
        new Vector3( 0,  0, -1), // North
        new Vector3( 1,  0,  0), // East
        new Vector3( 0,  0,  1), // South
        new Vector3( -1,  0,  0), // West
        new Vector3( 0,  1,  0), // Up
        new Vector3( 0, -1,  0)  // Down
    };
        public class MeshData
        {
            public VertexPositionNormalTexture[] vertices;
            public int[] indices;

            public int vertexCount;
            public int waterVertexCount;
            public int indexCount;
            public int waterIndexCount;

            public MeshData(VertexPositionNormalTexture[] vertices, int[] indices, int vertexCount, int waterVertexCount, int indexCount, int waterIndexCount)
            {
                this.vertices = vertices;
                this.indices = indices;
                this.vertexCount = vertexCount;
                this.waterVertexCount = waterVertexCount;
                this.indexCount = indexCount;
                this.waterIndexCount = waterIndexCount;
            }
        }
        private class MeshDataResponse
        {
            public Vector2 Position;
            public MeshData MeshData;
            public double Timestamp;

            public MeshDataResponse(MeshData meshData, Vector2 position, double timestamp)
            {
                MeshData = meshData;
                Position = position;
                Timestamp = timestamp;
            }
        }
        private class MeshDataRequest
        {
            public Vector2 Position;
            public Dictionary<Vector3, Block> blockData;
            public double Timestamp;

            public MeshDataRequest(Vector2 position, Dictionary<Vector3, Block> blockData, double timestamp)
            {
                Position = position;
                this.blockData = blockData;
                Timestamp = timestamp;
            }
        }
        private class BlockDataRequest
        {
            public Vector2 Position;
            public double Timestamp;

            public BlockDataRequest(Vector2 position, double timestamp)
            {
                Position = position;
                Timestamp = timestamp;
            }
        }
        private class BlockDataResponse
        {
            public Vector2 Position;
            public double Timestamp;
            public Dictionary<Vector3, Block> blocks;

            public BlockDataResponse(Vector2 position, double timestamp, Dictionary<Vector3, Block> blocks)
            {
                Position = position;
                Timestamp = timestamp;
                this.blocks = blocks;
            }
        }


    }
}