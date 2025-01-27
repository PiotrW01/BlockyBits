using BlockyBitsClient.src.Managers;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockyBitsClient.src
{
    internal static class Globals
    {
        public static Texture2D defaultTexture;
        public static SpriteFont font;
        public static float fogDistance
        {
            get
            {
                return 16f * Settings.renderDistance;
            }
        }



        public static void LoadContent(ContentManager cm)
        {
            defaultTexture = cm.Load<Texture2D>("textures/default_background");
            font = cm.Load<SpriteFont>("font");
            
        }


    }
}
