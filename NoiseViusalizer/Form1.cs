using LibNoise.Filter;
using LibNoise.Primitive;
using LibNoise;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibNoise.Builder;
using LibNoise.Transformer;
using BlockyBitsClient.src.Noise;

namespace NoiseViusalizer
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
            pictureBox1.SetBounds(0, 0, 512, 512);
            trackBar1.ValueChanged += (s, e) =>
            {
                frequency = trackBar1.Value * 0.002f;
                GenerateNoise();
            };
            trackBar2.ValueChanged += (s, e) =>
            {
                octaves = trackBar2.Value;
                GenerateNoise();
            };
            trackBar3.ValueChanged += (s, e) =>
            {
                lacunarity = 1 + trackBar3.Value * 0.25f;
                GenerateNoise();
            };
            trackBar4.ValueChanged += (s, e) =>
            {
                gain = trackBar4.Value * 0.3f;
                GenerateNoise();
            };
            trackBar5.ValueChanged += (s, e) =>
            {
                scale = trackBar5.Value * 0.01f;
                GenerateNoise();
            };


            LibNoise.Filter.HybridMultiFractal mf = new HybridMultiFractal();
            mf.Primitive2D = new LibNoise.Primitive.ImprovedPerlin(22, NoiseQuality.Standard);
            mf.Primitive3D = new LibNoise.Primitive.ImprovedPerlin(22, NoiseQuality.Standard);
            mf.Lacunarity = 2.2f;
            mf.Frequency = 0.006f;
            mf.Gain = 1.0f;
            mf.Offset = 0.7f;
            mf.SpectralExponent = 0.25f;
            mf.OctaveCount = 4;
            float[,] arr = new float[16,16];
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    arr[i, j] = mf.GetValue(i * 1.2f, j * 1.2f);
                }
            }


            GenerateNoise();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        float frequency = 0.002f;
        int octaves = 1;
        float lacunarity = 1f;
        float gain = 1.0f;
        float scale = 0.01f;

        void GenerateNoise()
        {
            int size = 512;

            IcariaNoise noise = new IcariaNoise();
            


            Bitmap noise = new Bitmap(size, size);
            SimplexPerlin p = new LibNoise.Primitive.SimplexPerlin(20, NoiseQuality.Standard);
            SimplexPerlin p2 = new LibNoise.Primitive.SimplexPerlin(62, NoiseQuality.Standard);

            ScalePoint scaler = new ScalePoint(p, scale, scale, scale);
            ScalePoint scaler2 = new ScalePoint(p2, scale, scale, scale);
            LibNoise.Combiner.Multiply multiplier = new LibNoise.Combiner.Multiply(scaler, scaler2);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int val = (int)((IcariaNoise.GradientNoise(i, j, 22) + 1f) / 2f * 255);
                    /*                    if (val < 55)
                                        {
                                            noise.SetPixel(i, j, Color.FromArgb(255, 0, 0, 200));
                                        } else
                                        {
                                            noise.SetPixel(i, j, Color.FromArgb(255, 0, 200, 0));
                                        }*/
                    noise.SetPixel(i, j, Color.FromArgb(255, val, val, val));
                }
            }
            pictureBox1.Image = noise;
        }


        public float GetValue(float x, float y, IModule3D module)
        {
            float total = 0;
            float maxValue = 0;
            float frequency = this.frequency;
            float amplitude = 1.0f;

            for (int i = 0; i < octaves; i++)
            {
                total += (module.GetValue(x * frequency, 10, y * frequency) + 1f) / 2f * amplitude;
                maxValue += amplitude;

                amplitude *= gain; // Zmniejsz amplitudę
                frequency *= lacunarity; // Podwój częstotliwość
            }

            return total / maxValue; // Normalizacja do zakresu 0-1
        }



        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {

        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {

        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {

        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {

        }

        private void trackBar6_Scroll(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void trackBar5_Scroll_1(object sender, EventArgs e)
        {

        }
    }
}
