using BlockyBitsClient.src;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public static class TextureAtlas
{
    public static Texture2D atlas;
    public static Texture2D atlas_far;
    public static Texture2D atlas_medium;
    public static float horizontalOffset;
    public static float verticalOffset;

    public static Texture2D item_atlas;
    public static float itemXOffset;
    public static float itemYOffset;

    public static Sprite GetItemSpriteAt(int x, int y)
    {
        return new Sprite(new Rectangle(x * 16, y * 16, 16, 16));
    }

    public static Rectangle GetTextureAt(int x, int y)
    {
        return new Rectangle(x * Block.blockResolution, y * Block.blockResolution, Block.blockResolution, Block.blockResolution);
    }

    public static Sprite GetSpriteAt(int x, int y)
    {
        return new Sprite(new Rectangle(x * Block.blockResolution, y * Block.blockResolution, Block.blockResolution, Block.blockResolution));
    }

    public static void LoadAtlas(ContentManager cm)
    {
        atlas = cm.Load<Texture2D>("textures/texture_atlas");
        atlas_far = cm.Load<Texture2D>("textures/texture_atlas_far");
        atlas_medium = cm.Load<Texture2D>("textures/texture_atlas_medium");

        item_atlas = cm.Load<Texture2D>("textures/item_atlas");
        itemXOffset = 16f / item_atlas.Width;
        itemYOffset = 16f / item_atlas.Height;


        horizontalOffset = (float)Block.blockResolution / (float)atlas.Width;
        verticalOffset = (float)Block.blockResolution / (float)atlas.Height;
    }

}
