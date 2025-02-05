using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlockyBitsClient.src
{
    public class Item
    {
        public Rectangle Texture;
        string Name;
        uint ID;

        public Item(string name)
        {
            Name = name;
            //ID = Block.NameToId(name);
            Texture = TextureAtlas.GetTextureOf(Name);
        }

        public Item(ushort ID)
        {
            this.ID = ID;
            Name = Block.IdToName(ID);
            Texture = TextureAtlas.GetTextureOf(Name);
        }

    }
}
