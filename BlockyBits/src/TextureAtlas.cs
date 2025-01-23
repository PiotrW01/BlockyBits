using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public static class TextureAtlas
{
    public static Texture2D atlas;
    public static float horizontalOffset;
    public static float verticalOffset;


    public static void SetAtlas(Texture2D atlas)
    {
        TextureAtlas.atlas = atlas;
        horizontalOffset = (float)Block.blockResolution / (float)atlas.Width;
        verticalOffset = (float)Block.blockResolution / (float)atlas.Height;
        
    }

    public static Rectangle GetTextureAt(int x, int y)
    {
        return new Rectangle(x * Block.blockResolution, y * Block.blockResolution, Block.blockResolution, Block.blockResolution);
    }

}
