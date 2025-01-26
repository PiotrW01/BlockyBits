using BlockyBitsClient.src.Noise;
using LibNoise.Primitive;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlockyBitsClient.src.gui
{
    public class NoiseRenderer : GUIElement
    {
        Texture2D noiseTexture;

        public override void Ready()
        {
            int size = 512;
            
            noiseTexture = new Texture2D(Game1.game.GraphicsDevice, size, size);
            Color[] pixelData = new Color[size * size];
            float scale = 0.007f;
            SimplexPerlin perlin = new(22, LibNoise.NoiseQuality.Best);
            LibNoise.Transformer.ScalePoint scaler = new(perlin, scale, scale, scale);
            LibNoise.Modifier.Invert inverter = new(scaler);
            LibNoise.Model.Plane plane = new(inverter);


            BevinsGradient grad = new(22, LibNoise.NoiseQuality.Best);

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {

                    float val = plane.GetValue(x, y);
                    val = Utils.Map(val, -1, 1, 0, 1);
                    pixelData[x + y * size] = new Color(val, val, val);
                }
            }
            noiseTexture.SetData(pixelData);
        }



        public override void Render(SpriteBatch sb)
        {
            return;
            sb.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            sb.Draw(noiseTexture, new Rectangle(0,0,512,512), Color.White);
            sb.End();
        }


        float sumOctave(int iterations, float x, float y, float persistence, float scale, float minVal, float maxVal)
        {
            float maxAmp = 0;
            float amp = 1;
            float freq = scale;
            float noise = 0;

            for(int i = 0; i < iterations; ++i)
            {
                noise += IcariaNoise.GradientNoise(x * freq, y * freq, 22);
                maxAmp += amp;
                amp *= persistence;
                freq *= 2;
            }

            noise /= maxAmp;

            return noise;
        }
    }
}
