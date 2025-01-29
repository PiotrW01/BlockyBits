using BlockyBits.src;
using BlockyBitsClient.src.gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BlockyBitsClient.src.player
{
    public class HotbarInterface : GUIElement
    {
        private Hotbar hotbar;
        int slotSize = 64;
        Rectangle slot;
        Rectangle selectedSlot;

        public HotbarInterface(Hotbar hotbar)
        {
            this.hotbar = hotbar;
        }

        public override void Ready()
        {
            slot = TextureAtlas.GetTextureOf("dirt");
            selectedSlot = TextureAtlas.GetTextureOf("stone");
        }

        public override void Render(SpriteBatch sb)
        {
            Viewport vp = Game1.game.GraphicsDevice.Viewport;
            int startX = vp.Width / 2 - hotbar.hotbarItems.Length / 2 * slotSize;
            int startY = vp.Height - slotSize;
            sb.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            for (int x = 0; x < hotbar.hotbarItems.Length; x++)
            {
                Rectangle rect = new(startX + x * slotSize, startY, slotSize, slotSize);
                if (x == hotbar.selectedSlot)
                {
                    sb.Draw(TextureAtlas.atlas, rect, selectedSlot, Color.White);
                }
                else
                {
                    sb.Draw(TextureAtlas.atlas, rect, slot, Color.White);
                }
                if (hotbar.hotbarItems[x] != null)
                {
                    sb.Draw(TextureAtlas.atlas, rect, hotbar.hotbarItems[x].sprite.spriteTextureRect, Color.White);
                }
            }
            sb.End();
            base.Render(sb);
        }






    }
}
