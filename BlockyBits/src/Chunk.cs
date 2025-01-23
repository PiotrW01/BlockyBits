
using BlockyBitsClient.src;
using BlockyBitsClient.src.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Chunk
{
    public int posX;
    public int posY;

    public const int width = 16;
    public const int depth = 16;
    public const int height = 256;

    private Dictionary<Vector3, Block> blocks = new();
    private VertexBuffer vertexBuffer;
    private IndexBuffer indexBuffer;
    public int vertexCount = 0;
    private int indexCount = 0;

    private static readonly Vector3[] faceNormals = {
        new Vector3( 0,  0, -1), // Front
        new Vector3( 0,  0,  1), // Back
        new Vector3(-1,  0,  0), // Left
        new Vector3( 1,  0,  0), // Right
        new Vector3( 0,  1,  0), // Top
        new Vector3( 0, -1,  0)  // Bottom
    };

    // Cube face vertex positions
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
        GenerateBlocks();
        GenerateMesh();
    }

    public Chunk(int posX, int posY, Dictionary<Vector3, Block> blocks)
    {
        this.posX = posX;
        this.posY = posY;
        this.blocks = blocks;
        //GenerateMesh();
    }


    private void GenerateBlocks()
    {
        blocks.Add(new Vector3(0, 2 ,0), new Block(Block.Type.Dirt));
        blocks.Add(new Vector3(0, 2 ,1), new Block(Block.Type.Dirt));
        return;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                for (int k = 0; k < 1; k++)
                {
                    blocks.Add(new Vector3(i, k, j), new Block(Block.Type.Dirt));
                }
            }
        }
    }

    public bool HasBlockAt(Vector3 localPos)
    {
        return blocks.ContainsKey(localPos);
    }

    public Block GetBlockAtGlobalPos(Vector3 pos)
    {
        return blocks[pos];
    }

    public Block GetBlockAtLocalPos(Vector3 pos)
    {
        return blocks[pos];
    }

    public void SetBlock(Vector3 pos, Block block)
    {
        blocks[pos] = block;
        //GenerateMesh();
    }

    public void RemoveBlock(Vector3 pos)
    {
        blocks.Remove(pos);
        //GenerateMesh();
    }

    public void GenerateMesh()
    {
        //List<VertexPositionColor> vertices = new();
        List<VertexPositionTexture> texCoords = new();
        List<int> indices = new();

        //Vector3 cameraDir = Game1.camera.pos - new Vector3(posX * width, 0, posY * depth);
        //cameraDir.Normalize();

        foreach(var pos in blocks.Keys)
        {
            Block block = blocks[pos];
            Vector2[] blockUVs = Block.GetUVs(block.type);

            for (int face = 0; face < 6; face++)
            {
                Vector3 neighborPos = pos + faceNormals[face];
                if (blocks.ContainsKey(neighborPos)) continue;
                if (neighborPos.X < 0)
                {
                    if (ChunkManager.chunks.ContainsKey(new Vector2(posX - 1, posY)))
                    {
                        if (ChunkManager.chunks[new Vector2(posX - 1, posY)].HasBlockAt(new Vector3(width - 1, pos.Y, pos.Z))) continue;
                    }
                    else continue;
                }
                if (neighborPos.X == width)
                {
                    if (ChunkManager.chunks.ContainsKey(new Vector2(posX + 1, posY)))
                    {
                        if (ChunkManager.chunks[new Vector2(posX + 1, posY)].HasBlockAt(new Vector3(0, pos.Y, pos.Z))) continue;
                    }
                    else continue;
                }
                if (neighborPos.Z < 0)
                {
                    if (ChunkManager.chunks.ContainsKey(new Vector2(posX, posY - 1)))
                    {
                        if (ChunkManager.chunks[new Vector2(posX, posY - 1)].HasBlockAt(new Vector3(pos.X, pos.Y, depth - 1))) continue;
                    }
                    else continue;
                }
                if (neighborPos.Z == depth)
                {
                    if (ChunkManager.chunks.ContainsKey(new Vector2(posX, posY + 1)))
                    {
                        if (ChunkManager.chunks[new Vector2(posX, posY + 1)].HasBlockAt(new Vector3(pos.X, pos.Y, 0))) continue;
                    }
                    else continue;
                }
                if (neighborPos.Y < 0) continue;

                int startIndex = texCoords.Count;
                Vector2 uv0 = blockUVs[face]; // Top-left UV of the face texture
                Vector2 uv1 = uv0 + new Vector2(TextureAtlas.horizontalOffset, 0);
                Vector2 uv2 = uv0 + new Vector2(TextureAtlas.horizontalOffset, TextureAtlas.verticalOffset);
                Vector2 uv3 = uv0 + new Vector2(0, TextureAtlas.verticalOffset);
                texCoords.Add(new VertexPositionTexture(pos + faceVertices[face, 0], uv0));
                texCoords.Add(new VertexPositionTexture(pos + faceVertices[face, 1], uv1));
                texCoords.Add(new VertexPositionTexture(pos + faceVertices[face, 2], uv2));
                texCoords.Add(new VertexPositionTexture(pos + faceVertices[face, 3], uv3));

                indices.Add(startIndex);
                indices.Add(startIndex + 1);
                indices.Add(startIndex + 2);
                indices.Add(startIndex);
                indices.Add(startIndex + 2);
                indices.Add(startIndex + 3);
            }
        }
        vertexCount = texCoords.Count;
        indexCount = indices.Count;
        if (vertexCount == 0 || indexCount == 0) return;

        vertexBuffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionTexture), vertexCount, BufferUsage.WriteOnly);
        vertexBuffer.SetData(texCoords.ToArray());

        indexBuffer = new IndexBuffer(Game1.game.GraphicsDevice, IndexElementSize.ThirtyTwoBits, indexCount, BufferUsage.WriteOnly);
        indexBuffer.SetData(indices.ToArray());
    }

    public void Render()
    {
        if (vertexBuffer == null || indexBuffer == null) return;
        //Vector3 chunkPos = new Vector3(posX * width, Game1.camera.pos.Y, posY * depth);
        //Vector3 c = chunkPos - Game1.camera.pos;
        //Vector3 n = Game1.camera.forward;
        //n.Normalize();
        //c.Normalize();
        //float dot = Vector3.Dot(n, c);
        //if (dot < 0f) return;
        //Debug.WriteLine($"Visible at {posX}, {posY}, {Game1.game.elapsedTime}");
        BasicEffect effect = new BasicEffect(Game1.game.GraphicsDevice)
        {
            VertexColorEnabled = false,
            World = Matrix.CreateWorld(new Vector3(posX * width, 0, posY * depth), Vector3.Forward, Vector3.Up),
            View = Game1.camera.viewMatrix,
            Projection = Game1.camera.projectionMatrix,
            TextureEnabled = true,
            Texture = TextureAtlas.atlas,
        };

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
}

