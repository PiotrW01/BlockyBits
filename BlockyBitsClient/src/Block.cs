using Microsoft.Xna.Framework;
using System.Collections.Generic;

public class Block
{
    public enum Type {
        Air,
        Dirt,
        Stone,
        Water
    }
    public static int blockResolution = 16;
    public Type type;
    public bool isTransparent = false;
    public bool hasCollisions = true;

    public static Dictionary<Block.Type, Vector2[]> blockUVs;

    public Block(Block.Type type)
    {
        this.type = type;
        if(type == Type.Water)
        {
            hasCollisions = false;
            isTransparent = true;
        }
    }




    public static void InitializeBlocks()
    {
        blockUVs = new()
        {
            {Block.Type.Dirt, Utils.RepeatValues6(new Vector2(0,0))},
            {Block.Type.Stone, Utils.RepeatValues6(new Vector2(1,0))},
            {Block.Type.Water, Utils.RepeatValues6(new Vector2(2,0))},
        };
        CalculateUVs();
    }

    private static void CalculateUVs()
    {
        foreach (var UVs in blockUVs.Values)
        {
            for (int i = 0; i < UVs.Length; i++) 
            {
                UVs[i] = new Vector2(UVs[i].X*TextureAtlas.horizontalOffset, UVs[i].Y*TextureAtlas.verticalOffset);
            }
        }
    }



    public static Vector2[] GetUVs(Block.Type blockType)
    {
        Vector2[] UVs = blockUVs[blockType];
        return blockUVs[blockType];
    }
}

