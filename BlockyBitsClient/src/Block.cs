using BlockyBitsClient.src;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;

public class Block
{
    public ushort ID;

    private static Dictionary<String, BlockProperties> Blocks2 = new();
    private static Dictionary<String, ushort> BlockRegistry = new();
    private static Dictionary<ushort, String> idToBlock = new();

    public Block(string name)
    {
        if(BlockRegistry.TryGetValue(name, out ushort id)){
            ID = id;
        } else
        {
            ID = 1;
        }
    }

    public Block(ushort id)
    {
        if (BlockRegistry.ContainsValue(id))
        {
            ID = id;
        }
    }

    public static void LoadBlocks()
    {
        var blockNames = Directory.GetFiles("assets/blocks");

        ushort idCounter = 1;
        foreach (var blockPath in blockNames)
        {
            var json = File.ReadAllText(blockPath);
            BlockProperties prop = JsonSerializer.Deserialize<BlockProperties>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true});

            if(prop.Faces.Count > 0)
            {
                prop.ModelShape = new(Models.GetModelShape("default"));
                prop.ModelShape.vTextureUV.Clear();

                Vector2 uv0;
                Vector2 uv1;
                Vector2 uv2;
                Vector2 uv3;

                if (prop.Faces.ContainsKey("all"))
                {
                    uv0 = TextureAtlas.GetUVCoordsof(prop.Faces["all"]);
                    uv1 = uv0 + new Vector2(0, TextureAtlas.verticalOffset);
                    uv2 = uv0 + new Vector2(TextureAtlas.horizontalOffset, 0);
                    uv3 = uv0 + new Vector2(TextureAtlas.horizontalOffset, TextureAtlas.verticalOffset);

                    //north
                    prop.ModelShape.vTextureUV.Add(uv3);
                    prop.ModelShape.vTextureUV.Add(uv1);
                    prop.ModelShape.vTextureUV.Add(uv0);
                    prop.ModelShape.vTextureUV.Add(uv2);

                    //east
                    prop.ModelShape.vTextureUV.Add(uv3);
                    prop.ModelShape.vTextureUV.Add(uv1);
                    prop.ModelShape.vTextureUV.Add(uv0);
                    prop.ModelShape.vTextureUV.Add(uv2);

                    //south
                    prop.ModelShape.vTextureUV.Add(uv3);
                    prop.ModelShape.vTextureUV.Add(uv1);
                    prop.ModelShape.vTextureUV.Add(uv0);
                    prop.ModelShape.vTextureUV.Add(uv2);

                    //west
                    prop.ModelShape.vTextureUV.Add(uv3);
                    prop.ModelShape.vTextureUV.Add(uv1);
                    prop.ModelShape.vTextureUV.Add(uv0);
                    prop.ModelShape.vTextureUV.Add(uv2);

                    //up
                    prop.ModelShape.vTextureUV.Add(uv3);
                    prop.ModelShape.vTextureUV.Add(uv1);
                    prop.ModelShape.vTextureUV.Add(uv0);
                    prop.ModelShape.vTextureUV.Add(uv2);

                    //down
                    prop.ModelShape.vTextureUV.Add(uv3);
                    prop.ModelShape.vTextureUV.Add(uv1);
                    prop.ModelShape.vTextureUV.Add(uv0);
                    prop.ModelShape.vTextureUV.Add(uv2);
                } 
                else
                {
                    if (prop.Faces.ContainsKey("sides"))
                    {

                        uv0 = TextureAtlas.GetUVCoordsof(prop.Faces["sides"]);
                        uv1 = uv0 + new Vector2(0, TextureAtlas.verticalOffset);
                        uv2 = uv0 + new Vector2(TextureAtlas.horizontalOffset, 0);
                        uv3 = uv0 + new Vector2(TextureAtlas.horizontalOffset, TextureAtlas.verticalOffset);

                        //north
                        prop.ModelShape.vTextureUV.Add(uv3);
                        prop.ModelShape.vTextureUV.Add(uv1);
                        prop.ModelShape.vTextureUV.Add(uv0);
                        prop.ModelShape.vTextureUV.Add(uv2);


                        //east
                        prop.ModelShape.vTextureUV.Add(uv3);
                        prop.ModelShape.vTextureUV.Add(uv1);
                        prop.ModelShape.vTextureUV.Add(uv0);
                        prop.ModelShape.vTextureUV.Add(uv2);

                        //south
                        prop.ModelShape.vTextureUV.Add(uv3);
                        prop.ModelShape.vTextureUV.Add(uv1);
                        prop.ModelShape.vTextureUV.Add(uv0);
                        prop.ModelShape.vTextureUV.Add(uv2);

                        //west
                        prop.ModelShape.vTextureUV.Add(uv3);
                        prop.ModelShape.vTextureUV.Add(uv1);
                        prop.ModelShape.vTextureUV.Add(uv0);
                        prop.ModelShape.vTextureUV.Add(uv2);

                    }
                    else
                    {
                        uv0 = TextureAtlas.GetUVCoordsof(prop.Faces["north"]);
                        uv1 = uv0 + new Vector2(0, TextureAtlas.verticalOffset);
                        uv2 = uv0 + new Vector2(TextureAtlas.horizontalOffset, 0);
                        uv3 = uv0 + new Vector2(TextureAtlas.horizontalOffset, TextureAtlas.verticalOffset);

                        //north
                        prop.ModelShape.vTextureUV.Add(uv3);
                        prop.ModelShape.vTextureUV.Add(uv1);
                        prop.ModelShape.vTextureUV.Add(uv0);
                        prop.ModelShape.vTextureUV.Add(uv2);


                        uv0 = TextureAtlas.GetUVCoordsof(prop.Faces["east"]);
                        uv1 = uv0 + new Vector2(0, TextureAtlas.verticalOffset);
                        uv2 = uv0 + new Vector2(TextureAtlas.horizontalOffset, 0);
                        uv3 = uv0 + new Vector2(TextureAtlas.horizontalOffset, TextureAtlas.verticalOffset);

                        //east
                        prop.ModelShape.vTextureUV.Add(uv3);
                        prop.ModelShape.vTextureUV.Add(uv1);
                        prop.ModelShape.vTextureUV.Add(uv0);
                        prop.ModelShape.vTextureUV.Add(uv2);


                        uv0 = TextureAtlas.GetUVCoordsof(prop.Faces["south"]);
                        uv1 = uv0 + new Vector2(0, TextureAtlas.verticalOffset);
                        uv2 = uv0 + new Vector2(TextureAtlas.horizontalOffset, 0);
                        uv3 = uv0 + new Vector2(TextureAtlas.horizontalOffset, TextureAtlas.verticalOffset);

                        //south
                        prop.ModelShape.vTextureUV.Add(uv3);
                        prop.ModelShape.vTextureUV.Add(uv1);
                        prop.ModelShape.vTextureUV.Add(uv0);
                        prop.ModelShape.vTextureUV.Add(uv2);

                        uv0 = TextureAtlas.GetUVCoordsof(prop.Faces["west"]);
                        uv1 = uv0 + new Vector2(0, TextureAtlas.verticalOffset);
                        uv2 = uv0 + new Vector2(TextureAtlas.horizontalOffset, 0);
                        uv3 = uv0 + new Vector2(TextureAtlas.horizontalOffset, TextureAtlas.verticalOffset);

                        //west
                        prop.ModelShape.vTextureUV.Add(uv3);
                        prop.ModelShape.vTextureUV.Add(uv1);
                        prop.ModelShape.vTextureUV.Add(uv0);
                        prop.ModelShape.vTextureUV.Add(uv2);
                    }

                    //up
                    uv0 = TextureAtlas.GetUVCoordsof(prop.Faces["top"]);
                    uv1 = uv0 + new Vector2(0, TextureAtlas.verticalOffset);
                    uv2 = uv0 + new Vector2(TextureAtlas.horizontalOffset, 0);
                    uv3 = uv0 + new Vector2(TextureAtlas.horizontalOffset, TextureAtlas.verticalOffset);


                    prop.ModelShape.vTextureUV.Add(uv3);
                    prop.ModelShape.vTextureUV.Add(uv1);
                    prop.ModelShape.vTextureUV.Add(uv0);
                    prop.ModelShape.vTextureUV.Add(uv2);

                    uv0 = TextureAtlas.GetUVCoordsof(prop.Faces["bottom"]);
                    uv1 = uv0 + new Vector2(0, TextureAtlas.verticalOffset);
                    uv2 = uv0 + new Vector2(TextureAtlas.horizontalOffset, 0);
                    uv3 = uv0 + new Vector2(TextureAtlas.horizontalOffset, TextureAtlas.verticalOffset);

                    //down
                    prop.ModelShape.vTextureUV.Add(uv3);
                    prop.ModelShape.vTextureUV.Add(uv1);
                    prop.ModelShape.vTextureUV.Add(uv0);
                    prop.ModelShape.vTextureUV.Add(uv2);
                }
                prop.ModelShape.RefreshVertices();
                Models.TryAddModelShape(prop.name, prop.ModelShape);
            } 
            else
            {
                if(Models.TryGetModelShape(prop.name, out ModelShape shape))
                {
                    prop.ModelShape = shape;
                } else
                {
                    Debug.WriteLine($"Block {prop.name} doesn't have a model, skipping...");
                    continue;
                }
            }



            if(Blocks2.TryAdd(prop.name, prop))
            {
                if(prop.id != 0)
                {
                    if(BlockRegistry.TryAdd(prop.name, prop.id))
                    {
                        idToBlock.Add(prop.id, prop.name);
                    }
                    else
                    {
                        BlockRegistry.Add(prop.name, idCounter);
                        idToBlock.Add(idCounter, prop.name);
                        idCounter++;
                    }
                } 
                else
                {
                    BlockRegistry.Add(prop.name, idCounter);
                    idToBlock.Add(idCounter, prop.name);
                    idCounter++;
                }
            }
        }




    }

    public static BlockProperties GetBlockProperties(string name)
    {
        if(Blocks2.TryGetValue(name, out var prop))
        {
            return prop;
        }
        return null;
    }

    public static string IdToName(ushort id)
    {
        return idToBlock[id];
    }

    public static ushort NameToId(string name)
    {
        return BlockRegistry[name];
    }
    public static BlockProperties GetBlockProperties(ushort id)
    {
        return Blocks2[idToBlock[id]];
    }
}

public class BlockProperties
{
    public string name {  get; set; }
    public ushort id { get; set; } = 0;
    public int Hardness { get; set; } = 0;
    public bool IsTransparent { get; set; } = false;
    public bool IsFluid {  get; set; } = false;
    public float FluidDensity { get; set; } = 1.0f;
    public bool HasCollisions { get; set; } = true;
    public Dictionary<string, string> Faces { get; set; } = new();

    public ModelShape ModelShape { get; set; }

}

