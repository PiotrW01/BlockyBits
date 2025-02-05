using LibNoise;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockyBitsClient.src
{
    internal class NoiseTest
    {
        LibNoise.Primitive.SimplexPerlin perlinNoise;
        Texture2D texture;
        int size = 512;
        public float frequency = 0.003f;
        public float amplitude = 0.5f;
        public int octaves = 2;
        public float lacunarity = 4.15f;
        public float gain = 0.2f;

        public NoiseTest(GraphicsDevice gd)
        {
            perlinNoise = new(960, NoiseQuality.Best);
            texture = new(gd, size, size, true, SurfaceFormat.Color);
        }

        public void GenerateMap()
        {
            Color[] colors = new Color[size*size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float noiseValue = FractalNoise(i, j);
                    colors[i * size + j] = new Color(noiseValue, noiseValue, noiseValue);
                }
            }
            texture.SetData(colors);
        }


        public float FractalNoise(float x, float y)
        {
            float total = 0;
            float maxValue = 0;
            float frequency = this.frequency;
            float amplitude = this.amplitude;

            for (int i = 0; i < octaves; i++)
            {
                total += (perlinNoise.GetValue(x * frequency, y * frequency) + 1f) / 2f * amplitude;
                maxValue += amplitude;

                amplitude *= gain; // Zmniejsz amplitudę
                frequency *= lacunarity; // Podwój częstotliwość
            }
            
            return total / maxValue; // Normalizacja do zakresu 0-1
        }

        public float GetValue(float x, float y)
        {
            float total = 0;
            float maxValue = 0;
            float frequency = this.frequency;
            float amplitude = this.amplitude;

            for (int i = 0; i < octaves; i++)
            {
                total += (perlinNoise.GetValue(x * frequency, y * frequency) + 1f) / 2f * amplitude;
                maxValue += amplitude;

                amplitude *= gain; // Zmniejsz amplitudę
                frequency *= lacunarity; // Podwój częstotliwość
            }

            return total / maxValue; // Normalizacja do zakresu 0-1
        }



        public void render(SpriteBatch sb)
        {
            if (texture == null) return;
            sb.Begin();
            sb.Draw(texture, Vector2.Zero, Color.White);
            sb.End();

        }

    }
}
