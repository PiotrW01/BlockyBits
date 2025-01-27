using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockyBitsClient.src.gui
{
    internal class Button: GUIElement
    {
        public string text = "";
        public event Action OnClick;
        private Point padding = new Point(20, 20);

        public Button(string text): base()
        {
            this.text = text;
            rect = new Rectangle(pos, Globals.font.MeasureString(text).ToPoint() + padding);
        }

        public override void OnClickChanged()
        {
            if(Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                OnClick?.Invoke();
            }
        }


        public override void Render(SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            sb.Draw(Globals.defaultTexture, rect, Color.Gray);
            sb.DrawString(Globals.font, text, rect.Center.ToVector2() - Globals.font.MeasureString(text)/2, Color.White);
            sb.End();
        }



    }
}
