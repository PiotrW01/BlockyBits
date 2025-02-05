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
using System.Text.Json;
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

    public static int resolution = 16;
    public static Texture2D noise;

    private static Dictionary<String, Vector2> textureCoords = new();
    private static Dictionary<String, Vector2> UVCoords = new();

    public static Rectangle GetTextureOf(string name)
    {
        Vector2 coords = textureCoords[name];
        return new Rectangle((int)coords.X, (int)coords.Y, resolution, resolution);
    }

    public static Vector2 GetUVCoordsof(string name)
    {
        return UVCoords[name];
    }

    public static void LoadAtlas(ContentManager cm)
    {
        atlas = cm.Load<Texture2D>("textures/minecraft-atlas");
        atlas_medium = cm.Load<Texture2D>("textures/minecraft-atlas-medium");
        atlas_far = cm.Load<Texture2D>("textures/minecraft-atlas-far");

        item_atlas = cm.Load<Texture2D>("textures/item_atlas");
        itemXOffset = 16f / item_atlas.Width;
        itemYOffset = 16f / item_atlas.Height;


        horizontalOffset = (float)resolution / (float)atlas.Width;
        verticalOffset = (float)resolution / (float)atlas.Height;
        noise = cm.Load<Texture2D>("textures/perlin512");


        string json = File.ReadAllText("assets/texture_names.json");
        var tCoords = JsonSerializer.Deserialize<Dictionary<String, int[]>>(json);

        foreach(var name in tCoords.Keys)
        {
            textureCoords.TryAdd(name, new Vector2(tCoords[name][0] * resolution, tCoords[name][1] * resolution));
            UVCoords.TryAdd(name, new Vector2(tCoords[name][0] * horizontalOffset, tCoords[name][1] * verticalOffset));
        }
    }
}
