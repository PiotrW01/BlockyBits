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
            backgroundTexture = TextureAtlas.GetTextureOf("dirt");
            Debug.WriteLine("menu loaded");
            bt = new Button("Start the game!");
            bt.SetPosition(100, 100);
            AddGUIelement(bt);
            bt.OnClick += () =>
            {
                GUIManager.RemoveUIElement(this);
                GUIManager.RemoveUIElement(bt);
                Game1.Player = new Player();
                Game1.Player.Transform.GlobalPosition = new Vector3(-120, 85, 82);
                ObjectManager.Add(Game1.Player);
            };
        }


        public override void Render(SpriteBatch sb)
        {
            int res = TextureAtlas.resolution * backgroundScale;

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
