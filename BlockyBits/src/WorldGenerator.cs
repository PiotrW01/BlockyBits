using BlockyBitsClient.src.Noise;
using LibNoise;
using LibNoise.Primitive;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlockyBits.src
{
    internal class WorldGenerator
    {
        LibNoise.Model.Plane plane;
        private int seed = 0;
        public WorldGenerator(int seed) 
        {
            this.seed = seed;
            float scale = 0.007f;
            SimplexPerlin perlin = new(22, LibNoise.NoiseQuality.Best);
            LibNoise.Transformer.ScalePoint scaler = new(perlin, scale, scale, scale);
            LibNoise.Modifier.Invert inverter = new(scaler);
            plane = new(inverter);


        }

        public Chunk GenerateChunk(int x, int y)
        {
            int xOffset, yOffset;
            xOffset = x * Chunk.width;
            yOffset = y * Chunk.depth;
            Dictionary<Vector3, Block> blocks = new();



            for (int i = 0; i < Chunk.width; i++)
            {
                for (int j = 0; j < Chunk.depth; j++)
                {
                    //float value = Noise.CalcPixel2D(xOffset + i, yOffset + j, 0.01f);
                    float value = (plane.GetValue(xOffset + i, yOffset + j) + 1) / 2f * 255f;
                    //value = Utils.Map(value, 0, 1, 0, Chunk.height);
                    value /= 4f;
                    for (int k = 0; k <= value; k++)
                    {
                        if(k < 16)
                        {
                            blocks.Add(new Vector3(i,k,j), new Block(Block.Type.Stone));
                        } else blocks.Add(new Vector3(i, k, j), new Block(Block.Type.Dirt));
                    }
                }
            }
            return new Chunk(x, y, blocks);
        }


    }
}
