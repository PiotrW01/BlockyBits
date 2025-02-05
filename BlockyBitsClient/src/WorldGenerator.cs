using BlockyBitsClient.src;
using BlockyBitsClient.src.Managers;
using BlockyBitsClient.src.Noise;
using LibNoise;
using LibNoise.Modifier;
using LibNoise.Primitive;
using LibNoise.Transformer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlockyBits.src
{
    public class WorldGenerator
    {
        LibNoise.Model.Plane plane;
        NoiseTest test;

        private int seed = 0;
        public WorldGenerator(int seed) 
        {
            this.seed = seed;
            float scale = 0.005f;
            SimplexPerlin perlin = new(22, NoiseQuality.Best);
            BevinsGradient bevinX = new(10, NoiseQuality.Standard);
            BevinsGradient bevinY = new(16, NoiseQuality.Standard);
            BevinsGradient bevinZ = new(24, NoiseQuality.Standard);
            Turbulence tb = new(perlin,bevinX, bevinY, bevinZ, 0.8f);
            LibNoise.Modifier.ScaleBias bias = new(tb, 1.4f, 0.3f);
            ScalePoint scaler1 = new(bias, scale, scale, scale);

            float scale2 = 0.001f;
            SimplexPerlin perlin2 = new(24, NoiseQuality.Best);
            BevinsGradient bevinX2 = new(12, NoiseQuality.Standard);
            BevinsGradient bevinY2 = new(12, NoiseQuality.Standard);
            BevinsGradient bevinZ2 = new(28, NoiseQuality.Standard);
            Turbulence tb2 = new(perlin2, bevinX2, bevinY2, bevinZ2, 0.8f);
            LibNoise.Modifier.ScaleBias bias2 = new(tb2, 1.4f, 0.3f);
            ScalePoint scaler2 = new(bias2, scale2, scale2, scale2);
            LibNoise.Modifier.Clamp clamp = new(scaler2,-0.5f, 0.5f);


            LibNoise.Combiner.Add adder = new(scaler1, clamp);
            Invert inv = new(adder);
            plane = new(inv);

            test = new(Game1.game.GraphicsDevice);

        }

        public Chunk GenerateChunk(int x, int y)
        {
            int xOffset, yOffset;
            xOffset = x * Chunk.width;
            yOffset = y * Chunk.width;
            Dictionary<Vector3, Block> blocks = new();

            var rand = new Random(22);

            for (int i = 0; i < Chunk.width; i++)
            {
                for (int j = 0; j < Chunk.width; j++)
                {
                    //float value = Noise.CalcPixel2D(xOffset + i, yOffset + j, 0.01f);
                    float value = (plane.GetValue(xOffset + i, yOffset + j) + 1) / 2f * 255f;
                    value = Math.Clamp(value, 0.0f, 255.0f);
                    //value = Utils.Map(value, 0, 1, 0, Chunk.height);
                    value /= 4f;
                    for (int k = 0; k <= (int)value; k++)
                    {
                        if(k < 16)
                        {
                            blocks[new Vector3(i, k, j)] = new Block("stone");
                        }
                        else if(k < (int)value)
                        {
                            blocks[new Vector3(i, k, j)] = new Block("dirt");
                        } else
                        {
                            blocks[new Vector3(i, k, j)] = new Block("grass_block");
                            if (rand.NextSingle() > 0.92f && k > 30) blocks[new Vector3(i, k + 1, j)] = new Block("oak_log");
                        }
                    }
                    for (int k = (int)value + 1; k <= 30; k++)
                    {
                        blocks[new Vector3(i, k, j)] = new Block("water");
                    }
                }
            }
            return new Chunk(x, y, blocks);
        }
        public Dictionary<Vector3, Block> GenerateBlockData(int x, int y)
        {
            int xOffset, yOffset;
            xOffset = x * Chunk.width;
            yOffset = y * Chunk.width;
            Dictionary<Vector3, Block> blocks = new();

            var rand = new Random(22);

            for (int i = 0; i < Chunk.width; i++)
            {
                for (int j = 0; j < Chunk.width; j++)
                {
                    //float value = Noise.CalcPixel2D(xOffset + i, yOffset + j, 0.01f);
                    //float value = (plane.GetValue(xOffset + i, yOffset + j) + 1) / 2f * 255f;
                    float value = test.GetValue(xOffset + i, yOffset + j) * 255f;
                    value = Math.Clamp(value, 0.0f, 255.0f);
                    //value = Utils.Map(value, 0, 1, 0, Chunk.height);
                    value /= 4f;
                    for (int k = 0; k <= (int)value; k++)
                    {
                        if (k < 16)
                        {
                            blocks[new Vector3(i, k, j)] = new Block("stone");
                        }
                        else if (k < (int)value)
                        {
                            blocks[new Vector3(i, k, j)] = new Block("dirt");
                        }
                        else
                        {
                            blocks[new Vector3(i, k, j)] = new Block("grass_block");
                            if (rand.NextSingle() > 0.92f && k > 30) blocks[new Vector3(i, k + 1, j)] = new Block("oak_log");
                        }
                    }
                    for (int k = (int)value + 1; k <= 30; k++)
                    {
                        blocks[new Vector3(i, k, j)] = new Block("water");
                    }
                }
            }
            return blocks;
        }

    }
}
