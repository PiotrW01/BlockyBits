using BlockyBits.src;
using BlockyBitsClient.src.Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockyBitsClient.src.player
{
    public class Hotbar: Object
    {
        public Item[] hotbarItems;
        HotbarInterface hotbarInterface;
        public int selectedSlot = 0;



        public Hotbar(int size)
        {
            hotbarItems = new Item[size];
            hotbarInterface = new HotbarInterface(this);
            GUIManager.RegisterUIElement(hotbarInterface);
            Item chicken = new Item();
            chicken.sprite = new Sprite(TextureAtlas.GetTextureOf("raw-chicken"));
            hotbarItems[0] = chicken;
        }


        public override void HandleScrollInput()
        {
            int direction = Input.GetScrollDirection();

            if (direction == -1)
            {
                selectedSlot++;
            }
            else if (direction == 1)
            {
                selectedSlot--;
            }

            if (selectedSlot < 0)
            {
                selectedSlot = hotbarItems.Length - 1;
            }
            else
            {
                selectedSlot = selectedSlot % hotbarItems.Length;
            }
        }
    }
}
