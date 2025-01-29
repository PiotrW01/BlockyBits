using BlockyBitsClient.src.Noise;
using LibNoise;
using LibNoise.Primitive;
using LibNoise.Transformer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlockyBits.src
{
    internal class WorldGenerator
    {
        LibNoise.Model.Plane plane;
        Turbulence tb;

        private int seed = 0;
        public WorldGenerator(int seed) 
        {
            this.seed = seed;
            float scale = 0.005f;
            SimplexPerlin perlin = new(22, NoiseQuality.Best);
            BevinsGradient bevinX = new(10, NoiseQuality.Standard);
            BevinsGradient bevinY = new(16, NoiseQuality.Standard);
            BevinsGradient bevinZ = new(24, NoiseQuality.Standard);
            tb = new(perlin,bevinX, bevinY, bevinZ, 0.8f);
            LibNoise.Modifier.ScaleBias bias = new(tb, 1.4f, 0.3f);

            ScalePoint scaler = new(bias, scale, scale, scale);
            plane = new(scaler);


        }

        public Chunk GenerateChunk(int x, int y)
        {
            int xOffset, yOffset;
            xOffset = x * Chunk.width;
            yOffset = y * Chunk.depth;
            Dictionary<Vector3, Block> blocks = new();

            var rand = new Random(22);

            for (int i = 0; i < Chunk.width; i++)
            {
                for (int j = 0; j < Chunk.depth; j++)
                {
                    //float value = Noise.CalcPixel2D(xOffset + i, yOffset + j, 0.01f);
                    float value = (plane.GetValue(xOffset + i, yOffset + j) + 1) / 2f * 255f;
                    //value = Utils.Map(value, 0, 1, 0, Chunk.height);
                    value /= 4f;
                    for (int k = 0; k <= (int)value; k++)
                    {
                        if(k < 16)
                        {
                            blocks[new Vector3(i, k, j)] = new Block(Block.Type.Stone);
                        }
                        else if(k < (int)value)
                        {
                            blocks[new Vector3(i, k, j)] = new Block(Block.Type.Dirt);
                        } else
                        {
                            blocks[new Vector3(i, k, j)] = new Block(Block.Type.Grass);
                            if (rand.NextSingle() > 0.92f && k > 30) blocks[new Vector3(i, k + 1, j)] = new Block(Block.Type.OakLog);
                        }
                    }
                    for (int k = (int)value + 1; k <= 30; k++)
                    {
                        blocks[new Vector3(i, k, j)] = new Block(Block.Type.Water);
                    }
                }
            }
            return new Chunk(x, y, blocks);
        }


    }
}
