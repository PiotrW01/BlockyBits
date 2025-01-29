using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockyBitsClient.src
{
    public static class Shaders
    {
        private static List<Effect> shaderList;

        public static Effect WaterShader = new(Game1.game.GraphicsDevice, File.ReadAllBytes("Content/shaders/HighlightEffect.mgfx"));





        public static void UpdateShaderParameters()
        {
            //WaterShader.Parameters["Time"].SetValue((float)Game1.game.elapsedTime);
        }
    }
}
