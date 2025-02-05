using LibNoise.Combiner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockyBitsClient.src
{
    public static class Models
    {
        private static Dictionary<String, ModelShape> ModelShapes = new();

        private static readonly Vector3[] faceNormals = {
        new Vector3( 0,  0, -1), // north
        new Vector3( 1,  0,  0), // east
        new Vector3( 0,  0,  1), // south
        new Vector3( -1,  0,  0), // west
        new Vector3( 0,  1,  0), // up
        new Vector3( 0, -1,  0)  // down
        };

        public static ModelShape GetModelShape(string model)
        {
            if(ModelShapes.TryGetValue(model, out ModelShape info))
            {
                return info;
            }
            return ModelShapes["default"];
        }

        public static bool TryGetModelShape(string model, out  ModelShape shape)
        {
            if (ModelShapes.TryGetValue(model, out ModelShape modelShape))
            {
                shape = modelShape;
                return true;
            }
            shape = null;
            return false;
        }

        public static bool TryAddModelShape(String name, ModelShape shape)
        {
            if(ModelShapes.TryAdd(name, shape))
            {
                return true;
            }
            return false;
        }
        public static void LoadModels()
        {
            var modelNames = Directory.GetFiles("assets/models");

            foreach(var modelPath in modelNames)
            {
                string json = File.ReadAllText(modelPath);
                Model model = ModelImporter.ImportFromJson(json);

                List<int> indices = new();

                List<Vector3> vPos = new();
                List<Vector3> vNormal = new();
                List<Vector2> vUV = new();

                Vector3[] mins = new Vector3[model.Elements.Count];
                Vector3[] maxs = new Vector3[model.Elements.Count];

                for (int i = 0; i < model.Elements.Count; i++)
                {
                    Vector3 vMin = new Vector3(model.Elements[i].From[0], model.Elements[i].From[1], model.Elements[i].From[2]);
                    vMin /= 16f;
                    Vector3 vMax = new Vector3(model.Elements[i].To[0], model.Elements[i].To[1], model.Elements[i].To[2]);
                    vMax /= 16f;

                    mins[i] = vMin;
                    maxs[i] = vMax;

                    Vector3 v1 = new Vector3(vMax.X, vMin.Y, vMin.Z);
                    Vector3 v2 = new Vector3(vMax.X, vMax.Y, vMin.Z);
                    Vector3 v3 = new Vector3(vMin.X, vMax.Y, vMin.Z);
                    Vector3 v4 = new Vector3(vMax.X, vMin.Y, vMax.Z);
                    Vector3 v5 = new Vector3(vMin.X, vMin.Y, vMax.Z);
                    Vector3 v6 = new Vector3(vMin.X, vMax.Y, vMax.Z);

                    foreach (string face in model.Elements[i].Faces.Keys)
                    {
                        int startIndex = vPos.Count;
                        var uv = model.Elements[i].Faces[face].Uv;
                        uv[0] *= 0.0625f;
                        uv[1] *= 0.0625f;
                        uv[2] *= 0.0625f;
                        uv[3] *= 0.0625f;
                        Vector2 uv0 = new Vector2(uv[0], uv[1]);
                        Vector2 uv1 = new Vector2(uv[0], uv[3]);
                        Vector2 uv2 = new Vector2(uv[2], uv[1]);
                        Vector2 uv3 = new Vector2(uv[2], uv[3]);

                        switch (face)
                        {
                            case "north":
                                vPos.Add(vMin);
                                vNormal.Add(faceNormals[0]);
                                vUV.Add(uv1);

                                vPos.Add(v1);
                                vNormal.Add(faceNormals[0]);
                                vUV.Add(uv3);

                                vPos.Add(v2);
                                vNormal.Add(faceNormals[0]);
                                vUV.Add(uv2);

                                vPos.Add(v3);
                                vNormal.Add(faceNormals[0]);
                                vUV.Add(uv0);

                                break;
                            case "east":

                                vPos.Add(v1);
                                vNormal.Add(faceNormals[1]);
                                vUV.Add(uv1);

                                vPos.Add(v4);
                                vNormal.Add(faceNormals[1]);
                                vUV.Add(uv3);

                                vPos.Add(vMax);
                                vNormal.Add(faceNormals[1]);
                                vUV.Add(uv2);


                                vPos.Add(v2);
                                vNormal.Add(faceNormals[1]);
                                vUV.Add(uv0);


                                break;
                            case "south":
                                vPos.Add(v4);
                                vNormal.Add(faceNormals[2]);
                                vUV.Add(uv1);

                                vPos.Add(v5);
                                vNormal.Add(faceNormals[2]);
                                vUV.Add(uv3);

                                vPos.Add(v6);
                                vNormal.Add(faceNormals[2]);
                                vUV.Add(uv2);

                                vPos.Add(vMax);
                                vNormal.Add(faceNormals[2]);
                                vUV.Add(uv0);

                                break;
                            case "west":

                                vPos.Add(v5);
                                vNormal.Add(faceNormals[3]);
                                vUV.Add(uv1);

                                vPos.Add(vMin);
                                vNormal.Add(faceNormals[3]);
                                vUV.Add(uv3);


                                vPos.Add(v3);
                                vNormal.Add(faceNormals[3]);
                                vUV.Add(uv2);


                                vPos.Add(v6);
                                vNormal.Add(faceNormals[3]);
                                vUV.Add(uv0);


                                break;
                            case "up":

                                vPos.Add(vMax);
                                vNormal.Add(faceNormals[4]);
                                vUV.Add(uv1);

                                vPos.Add(v6);
                                vNormal.Add(faceNormals[4]);
                                vUV.Add(uv3);

                                vPos.Add(v3);
                                vNormal.Add(faceNormals[4]);
                                vUV.Add(uv2);

                                vPos.Add(v2);
                                vNormal.Add(faceNormals[4]);
                                vUV.Add(uv0);

                                break;
                            case "down":

                                vPos.Add(v1);
                                vNormal.Add(faceNormals[5]);
                                vUV.Add(uv1);

                                vPos.Add(vMin);
                                vNormal.Add(faceNormals[5]);
                                vUV.Add(uv3);

                                vPos.Add(v5);
                                vNormal.Add(faceNormals[5]);
                                vUV.Add(uv2);

                                vPos.Add(v4);
                                vNormal.Add(faceNormals[5]);
                                vUV.Add(uv0);

                                break;
                        }

                        indices.Add(startIndex);
                        indices.Add(startIndex + 1);
                        indices.Add(startIndex + 2);
                        indices.Add(startIndex);
                        indices.Add(startIndex + 2);
                        indices.Add(startIndex + 3);
                    }
                }
                ModelShapes.Add(model.Groups[0].Name, new ModelShape(model.Elements.Count, vPos, vNormal, mins, maxs, indices, vUV));
            }
        }
    }

    public class ModelShape
    {
        public int BlockCount;
        public List<int> indices;
        public List<VertexPositionNormalTexture> vertices = new();
        public List<Vector3> vPositions;
        public List<Vector3> vNormals;
        public List<Vector2> vTextureUV;
        public BoundingBox[] box;

        public ModelShape(int blockCount, List<Vector3> vPos, List<Vector3> vNormals, Vector3[] boxMin, Vector3[] boxMax, List<int> indices, List<Vector2> vTex = null)
        {
            vPositions = vPos;
            this.vNormals = vNormals;
            if(vTex == null)
            {
                vTextureUV = new();
            } else
            {
                vTextureUV = vTex;
            }
            BlockCount = blockCount;
            this.indices = indices;
            box = new BoundingBox[BlockCount];
            for(int i = 0; i < BlockCount; i++)
            {
                box[i] = new(boxMin[i], boxMax[i]);

                for(int j = 0; j < 6; j++)
                {
                    vertices.Add(new(vPositions[j * 4 + 24 * i], vNormals[j * 4 + 24 * i], vTextureUV[j * 4 + 24 * i]));
                    vertices.Add(new(vPositions[j * 4 + 1 + 24 * i], vNormals[j * 4 + 1 + 24 * i], vTextureUV[j * 4 + 1 + 24 * i]));
                    vertices.Add(new(vPositions[j * 4 + 2 + 24 * i], vNormals[j * 4 + 2 + 24 * i], vTextureUV[j * 4 + 2 + 24 * i]));
                    vertices.Add(new(vPositions[j * 4 + 3 + 24 * i], vNormals[j * 4 + 3 + 24 * i], vTextureUV[j * 4 + 3 + 24 * i]));
                }
            }


        }

        public void RefreshVertices()
        {
            vertices.Clear();
            for (int i = 0; i < BlockCount; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    vertices.Add(new(vPositions[j * 4 + 24 * i], vNormals[j * 4 + 24 * i], vTextureUV[j * 4 + 24 * i]));
                    vertices.Add(new(vPositions[j * 4 + 1 + 24 * i], vNormals[j * 4 + 1 + 24 * i], vTextureUV[j * 4 + 1 + 24 * i]));
                    vertices.Add(new(vPositions[j * 4 + 2 + 24 * i], vNormals[j * 4 + 2 + 24 * i], vTextureUV[j * 4 + 2 + 24 * i]));
                    vertices.Add(new(vPositions[j * 4 + 3 + 24 * i], vNormals[j * 4 + 3 + 24 * i], vTextureUV[j * 4 + 3 + 24 * i]));
                }
            }
        }

        public ModelShape(ModelShape shape)
        {
            BlockCount = shape.BlockCount;
            vPositions = new(shape.vPositions);
            vNormals = new(shape.vNormals);
            vTextureUV = new(shape.vTextureUV);
            box = (BoundingBox[])shape.box.Clone();
            vertices = new(shape.vertices);
            indices = new(shape.indices);
        }
    }
}
