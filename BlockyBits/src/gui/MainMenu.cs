using BlockyBits.src;
using BlockyBitsClient.src.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace BlockyBitsClient.src.gui
{
    internal class MainMenu : GUIElement
    {
        Rectangle backgroundTexture;
        int backgroundScale = 6;
        Color tint = new Color(0.5f, 0.5f, 0.5f);
        Button bt;

        public override void Ready()
        {
            backgroundTexture = TextureAtlas.GetTextureAt(0, 0);
            Debug.WriteLine("menu loaded");
            bt = new Button("Start the game!");
            bt.SetPosition(100, 100);
            AddGUIelement(bt);
            bt.OnClick += () =>
            {
                GUIManager.RemoveUIElement(this);
                GUIManager.RemoveUIElement(bt);
                Game1.game.StartGame();
            };
        }


        public override void Render(SpriteBatch sb)
        {
            int res = Block.blockResolution * backgroundScale;

            sb.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            for (int x = 0; x < Game1.game.GraphicsDevice.Viewport.Width; x += res)
            {
                for (int y = 0; y < Game1.game.GraphicsDevice.Viewport.Height; y += res)
                {
                    sb.Draw(TextureAtlas.atlas, new Rectangle(x, y, res, res), backgroundTexture, tint);
                }
            }
            sb.End();
        }
    }
}
