using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

public class Block
{
    public enum Type {
        Air,
        Dirt,
        Stone,
        Grass,
        Water,
        OakLog
    }
    public static int blockResolution = 16;
    public Type type;
    public bool isTransparent = false;
    public bool hasCollisions = true;

    public static Dictionary<Block.Type, Vector2[]> blockUVs;
    private static Dictionary<Block.Type, BlockInformation> BlockInfo = new();

    private static Dictionary<String, Vector2> UVCoords;
    private static Dictionary<String, Vector2> PixelCoords = new();

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

        PixelCoords = new()
        {
            {"dirt",  new Vector2(2,15)},
            {"stone",  new Vector2(14,28)},
            {"water",  new Vector2(0,0)},
            {"grass-top",  new Vector2(28,2)},
            {"grass-side",  new Vector2(28,1)},
            {"raw-chicken",  new Vector2(36,18)},
            {"oak-log-side", new Vector2(15,19)},
            {"oak-log-top", new Vector2(16,19)},
        };
        UVCoords = new();

        CalculateUVs();





        //front, back, left, right, top, bottom
        BlockInfo.Add(Type.Dirt, new()
        {
            UVs = Utils.GenerateUVs("dirt", "dirt", "dirt", "dirt", "dirt", "dirt"),
        });

        BlockInfo.Add(Type.Stone, new()
        {
            UVs = Utils.GenerateUVs("stone", "stone", "stone", "stone", "stone", "stone"),
        });

        BlockInfo.Add(Type.Water, new()
        {
            UVs = Utils.GenerateUVs("stone", "stone", "stone", "stone", "stone", "stone"),
        });

        BlockInfo.Add(Type.Grass, new()
        {
            UVs = Utils.GenerateUVs("grass-side", "grass-side", "grass-side", "grass-side", "grass-top", "dirt"),
        });

        BlockInfo.Add(Type.OakLog, new()
        {
            UVs = Utils.GenerateUVs("oak-log-side", "oak-log-side", "oak-log-side", "oak-log-side", "oak-log-top", "oak-log-top"),
        });
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

        // offset texture coordinates to correct texture UV coordinates
        foreach(var key in PixelCoords.Keys)
        {
            Vector2 newCoords = PixelCoords[key];
            PixelCoords[key] = new Vector2(newCoords.X * Block.blockResolution, newCoords.Y * Block.blockResolution);

            newCoords.X *= TextureAtlas.horizontalOffset;
            newCoords.Y *= TextureAtlas.verticalOffset;
            UVCoords.Add(key, newCoords);
        }
    }



    public static Vector2[] GetUVs(Block.Type blockType)
    {
        Vector2[] UVs = blockUVs[blockType];
        return blockUVs[blockType];
    }


    public static Vector2 GetUVCoordsof(String name)
    {
        return UVCoords[name];
    }

    public static Vector2 GetPixelCoordsOf(String name)
    {
        return PixelCoords[name];
    }

    public static BlockInformation GetBlockInformation(Block.Type type)
    {
        return BlockInfo[type];
    }


}

public class BlockInformation()
{
    public int hardness = 0;
    public bool isTransparent = false;
    public bool hasCollisions = true;

    // Front back left right top bottom
    public Vector2[] UVs =
    {

        };





    public int[] vertices = [];
    public int[] indices = [0, 1, 2, 0, 2, 3];

}

