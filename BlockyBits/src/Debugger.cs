using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

internal class Debugger
{
    private struct DrawRequest
    {
        public Vector3 from, to;
        public Color color;
        public DrawRequest(Vector3 from, Vector3 to, Color color)
        {
            this.from = from;
            this.to = to;
            this.color = color;
        }
    }

    private static GraphicsDevice gd;
    public static bool showDebugInfo = false;
    private static List<DrawRequest> drawQueue = new();

    public static void Enable(GraphicsDevice gd)
    {
        Debugger.gd = gd;
    }

    public static void QueueDraw(Vector3 from, Vector3 to, Color color)
    {
        drawQueue.Add(new DrawRequest(from, to, color));
    }

    public static void QueueDraw(BoundingBox box)
    {
        Vector3 counterCorner = new Vector3(box.Max.X, box.Min.Y, box.Max.Z);
        //bottom ring
        QueueDraw(box.Min, new Vector3(box.Max.X, box.Min.Y, box.Min.Z), Color.Red);
        QueueDraw(box.Min, new Vector3(box.Min.X, box.Min.Y, box.Max.Z), Color.Red);


        QueueDraw(counterCorner, new Vector3(box.Max.X, box.Min.Y, box.Min.Z), Color.Red);
        QueueDraw(counterCorner, new Vector3(box.Min.X, box.Min.Y, box.Max.Z), Color.Red);

        //top ring
        QueueDraw(box.Max, new Vector3(box.Min.X, box.Max.Y, box.Max.Z), Color.Red);
        QueueDraw(box.Max, new Vector3(box.Max.X, box.Max.Y, box.Min.Z), Color.Red);

        counterCorner = new Vector3(box.Min.X, box.Max.Y, box.Min.Z);
        QueueDraw(counterCorner, new Vector3(box.Min.X, box.Max.Y, box.Max.Z), Color.Red);
        QueueDraw(counterCorner, new Vector3(box.Max.X, box.Max.Y, box.Min.Z), Color.Red);

        QueueDraw(box.Min, box.Max, Color.Red);
    }

    public static void DrawDebugLines()
    {
        BasicEffect effect = new BasicEffect(gd)
        {
            VertexColorEnabled = true,
            World = Matrix.Identity,
            View = Game1.camera.viewMatrix,
            Projection = Game1.camera.projectionMatrix,
        };

        foreach (DrawRequest request in drawQueue)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[]
            {
                new VertexPositionColor(request.from, request.color),
                new VertexPositionColor(request.to, request.color)
            };
            VertexBuffer buff = new VertexBuffer(gd, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            buff.SetData(vertices);



            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.SetVertexBuffer(buff);
                gd.DrawPrimitives(PrimitiveType.LineList, 0, vertices.Length);
            }
        }

        drawQueue.Clear();
    }
}
