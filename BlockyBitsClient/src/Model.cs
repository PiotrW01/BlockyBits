using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlockyBitsClient.src
{
    public class Model
    {
        public string Credit { get; set; }
        public int[] TextureSize { get; set; }
        public Dictionary<string, string> Textures { get; set; }
        public List<Element> Elements { get; set; }
        public List<Group> Groups { get; set; }
    }

    public class Element
    {
        public string Name { get; set; }
        public float[] From { get; set; }
        public float[] To { get; set; }
        public Rotation Rotation { get; set; }
        public Dictionary<string, Face> Faces { get; set; }
        public int? Color { get; set; } // Nullable in case some elements don't have it
    }

    public class Rotation
    {
        public float Angle { get; set; }
        public string Axis { get; set; }
        public float[] Origin { get; set; }
    }


    public class Face
    {
        public float[] Uv { get; set; }
        public string Texture { get; set; }
    }

    public class Group
    {
        public string Name { get; set; }
        public float[] Origin { get; set; }
        public int Color { get; set; }
        public List<int> Children { get; set; }
    }

    public static class ModelImporter
    {
        public static Model ImportFromJson(string json)
        {
            return JsonSerializer.Deserialize<Model>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
