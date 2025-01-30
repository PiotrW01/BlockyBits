
using BlockyBitsClient.src;
using BlockyBitsClient.src.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

public class Chunk
{

    public BoundingBox boundingBox;
    public Vector2 pos;

    public int posX;
    public int posY;

    public const int width = 32;
    public const int depth = 32;
    public const int height = 256;

    public bool hasMesh = false;
    public bool isScheduledToUnload = false;
    private Dictionary<Vector3, Block> blocks = new();
    private VertexBuffer vertexBuffer;
    private IndexBuffer indexBuffer;
    private int vertexCount = 0;
    private int waterVertexCount = 0;
    private int indexCount = 0;
    private int waterIndexCount = 0;
    private static BasicEffect effect = new(Game1.game.GraphicsDevice)
    {
        VertexColorEnabled = false,
        Projection = Game1.camera.projectionMatrix,
        TextureEnabled = true,
    };

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

    public Chunk(int posX, int posY)
    {
        this.posX = posX;
        this.posY = posY;
        pos = new Vector2(posX, posY);
        boundingBox = new(new Vector3(posX * width, 0, posY * depth),
                               new Vector3(posX * width + width, height, posY * depth + depth));
    }

    public Chunk(int posX, int posY, Dictionary<Vector3, Block> blocks)
    {
        this.posX = posX;
        this.posY = posY;
        this.blocks = blocks;
        pos = new Vector2(posX, posY);
        boundingBox = new(new Vector3(posX * width, 0, posY * depth),
                               new Vector3(posX * width + width, height, posY * depth + depth));
    }


    public void SetBlockData(Dictionary<Vector3, Block> blocks)
    {
        this.blocks = blocks;
    }

    public void GenerateMesh()
    {
        List<VertexPositionNormalTexture> texCoords = new();
        List<VertexPositionNormalTexture> texCoordsWater = new();
        List<int> indices = new();
        List<int> indicesWater = new();

        int highestPoint = 0;
        foreach(var pos in blocks.Keys)
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
                    if(blocks.TryGetValue(neighborPos, out Block b))
                    {
                        if (b.isTransparent && block.isTransparent) continue;
                        if (!b.isTransparent) continue;
                    }
                }
                if (neighborPos.X < 0)
                {
                    if(ChunkManager.Chunks.TryGetValue(new Vector2(posX - 1, posY), out Chunk chunk))
                    {
                        if(chunk.GetBlocks().TryGetValue(new Vector3(width - 1, pos.Y, pos.Z), out Block b))
                        {
                            if (b.isTransparent && block.isTransparent) continue;
                            if (!b.isTransparent) continue;
                        }
                    }
                    else continue;
                }
                if (neighborPos.Z < 0)
                {
                    if (ChunkManager.Chunks.TryGetValue(new Vector2(posX, posY - 1), out Chunk chunk))
                    {
                        if (chunk.GetBlocks().TryGetValue(new Vector3(pos.X, pos.Y, depth - 1), out Block b))
                        {
                            if (b.isTransparent && block.isTransparent) continue;
                            if (!b.isTransparent) continue;
                        }
                    }
                    else continue;
                }
                if (neighborPos.X == width)
                {
                    if (ChunkManager.Chunks.TryGetValue(new Vector2(posX + 1, posY), out Chunk chunk))
                    {
                        if (chunk.GetBlocks().TryGetValue(new Vector3(0, pos.Y, pos.Z), out Block b))
                        {
                            if (b.isTransparent && block.isTransparent) continue;
                            if (!b.isTransparent) continue;
                        }
                    }
                    else continue;
                }
                if (neighborPos.Z == depth)
                {
                    if (ChunkManager.Chunks.TryGetValue(new Vector2(posX, posY + 1), out Chunk chunk))
                    {
                        if (chunk.GetBlocks().TryGetValue(new Vector3(pos.X, pos.Y, 0), out Block b))
                        {
                            if (b.isTransparent && block.isTransparent) continue;
                            if (!b.isTransparent) continue;
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
        vertexCount = texCoords.Count;
        waterVertexCount = texCoordsWater.Count;
        indexCount = indices.Count;
        waterIndexCount = indicesWater.Count;

        if (vertexCount == 0 || indexCount == 0) return;

        if (vertexBuffer != null)
        {
            vertexBuffer.Dispose();
        }

        vertexBuffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionNormalTexture), vertexCount + waterVertexCount, BufferUsage.WriteOnly);
        vertexBuffer.SetData(texCoords.Concat(texCoordsWater).ToArray());

        if (indexBuffer != null)
        {
            indexBuffer.Dispose();
        }

        indexBuffer = new IndexBuffer(Game1.game.GraphicsDevice, IndexElementSize.ThirtyTwoBits, indexCount + waterIndexCount, BufferUsage.WriteOnly);
        indexBuffer.SetData(indices.Concat(indicesWater).ToArray());

        boundingBox.Max.Y = highestPoint + 2;
    }


    public void GenerateMeshNoOptimization()
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
        vertexCount = texCoords.Count;
        waterVertexCount = texCoordsWater.Count;
        indexCount = indices.Count;
        waterIndexCount = indicesWater.Count;

        if (vertexCount == 0 || indexCount == 0) return;

        if (vertexBuffer != null)
        {
            vertexBuffer.Dispose();
        }

        vertexBuffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionNormalTexture), vertexCount + waterVertexCount, BufferUsage.WriteOnly);
        vertexBuffer.SetData(texCoords.Concat(texCoordsWater).ToArray());

        if (indexBuffer != null)
        {
            indexBuffer.Dispose();
        }

        indexBuffer = new IndexBuffer(Game1.game.GraphicsDevice, IndexElementSize.ThirtyTwoBits, indexCount + waterIndexCount, BufferUsage.WriteOnly);
        indexBuffer.SetData(indices.Concat(indicesWater).ToArray());

        boundingBox.Max.Y = highestPoint + 2;
    }

    public void Render()
    {
        if (vertexBuffer == null || indexBuffer == null) return;
        effect.World = Matrix.CreateWorld(new Vector3(posX * width, 0, posY * depth), Vector3.Forward, Vector3.Up);
        effect.View = Game1.camera.viewMatrix;


        float distance = Vector2.Distance(pos * width, new Vector2(Game1.camera.Transform.GlobalPosition.X, Game1.camera.Transform.GlobalPosition.Z));
        if(pos == Utils.WorldToChunkPosition(Game1.camera.Transform.GlobalPosition))
        {
            Debugger.QueueDraw(boundingBox);
        }
        
        if (distance < 64)
        {
            effect.Texture = TextureAtlas.atlas;
        } else if(distance < 128)
        {
            effect.Texture = TextureAtlas.atlas_medium;
        }
        else
        {
            effect.Texture = TextureAtlas.atlas_far;
        }

        if (Settings.fog)
        {
            effect.FogEnabled = true;
            effect.FogColor = Color.CornflowerBlue.ToVector3();
            effect.FogStart = 10f;
            effect.FogEnd = Globals.fogDistance;
        }

        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            Game1.game.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            Game1.game.GraphicsDevice.Indices = indexBuffer;
            Game1.game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, indexCount / 3);
        }
    }

    public void RenderWater()
    {
        if (vertexBuffer == null || indexBuffer == null) return;

        Shaders.WaterShader.Parameters["World"].SetValue(Matrix.CreateWorld(new Vector3(posX * width, 0, posY * depth), Vector3.Forward, Vector3.Up));
        Shaders.WaterShader.Parameters["View"].SetValue(Game1.camera.viewMatrix);
        Shaders.WaterShader.Parameters["Projection"].SetValue(Game1.camera.projectionMatrix);
        Shaders.WaterShader.Parameters["GridPos"].SetValue(new Vector3(posX*width, 0, posY*depth));



        if (waterVertexCount > 0 && waterIndexCount > 0)
        {
            foreach (EffectPass pass in Shaders.WaterShader.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game1.game.GraphicsDevice.SetVertexBuffer(vertexBuffer);
                Game1.game.GraphicsDevice.Indices = indexBuffer;
                Game1.game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, vertexCount, indexCount, waterIndexCount / 3);
            }
        }
    }

    public bool HasBlockAt(Vector3 localPos)
    {
        return blocks.ContainsKey(localPos);
    }

    public Block GetBlockAtLocalPos(Vector3 pos)
    {
        blocks.TryGetValue(pos, out Block block);
        return block;
    }

    public void SetBlock(Vector3 pos, Block block)
    {
        blocks[pos] = block;
        GenerateMesh();
    }

    public void RemoveBlock(Vector3 localPos)
    {
        if (blocks.ContainsKey(localPos))
        {
            blocks.Remove(localPos);
            GenerateMesh();

            if(localPos.X == width - 1)
            {
                ChunkManager.Chunks[pos + Vector2.UnitX].GenerateMesh();
            }
            else if(localPos.X == 0)
            {
                ChunkManager.Chunks[pos - Vector2.UnitX].GenerateMesh();
            }
            if(localPos.Z == depth - 1)
            {
                ChunkManager.Chunks[pos + Vector2.UnitY].GenerateMesh();
            }
            else if(localPos.Z == 0)
            {
                ChunkManager.Chunks[pos - Vector2.UnitY].GenerateMesh();
            }
        }
    }

    public void RemoveBlockNotOptimized(Vector3 localPos)
    {
        blocks.Remove(localPos);
        GenerateMesh();
    }
    public Dictionary<Vector3, Block> GetBlocks()
    {
        return blocks;
    }

    public void Dispose()
    {
        if (vertexBuffer != null)
        {
            vertexBuffer.Dispose();
            vertexBuffer = null;
        }
        if (indexBuffer != null)
        {
            indexBuffer.Dispose();
            indexBuffer = null;
        }
        if(blocks != null)
        {
            blocks.Clear();
            blocks = null;
        }
    }
}

