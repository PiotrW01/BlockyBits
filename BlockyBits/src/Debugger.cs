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

    private static BasicEffect effect;
    private static GraphicsDevice gd;
    public static bool showDebugInfo = false;
    private static List<DrawRequest> drawQueue = new();

    public static void Enable(GraphicsDevice gd)
    {
        Debugger.gd = gd;
        effect = new BasicEffect(gd)
        {
            VertexColorEnabled = true,
            World = Matrix.Identity,
            View = Game1.camera.viewMatrix,
            Projection = Game1.camera.projectionMatrix,
        };
    }

    public static void QueueDraw(Vector3 from, Vector3 to, Color color)
    {
        drawQueue.Add(new DrawRequest(from, to, color));
    }

    public static void QueueDraw(BoundingBox box)
    {
        Vector3 counterCorner = new Vector3(box.Max.X, box.Min.Y, box.Max.Z);
        //bottom ring
        QueueDraw(box.Min, new Vector3(box.Max.X, box.Min.Y, box.Min.Z), Color.Magenta);
        QueueDraw(box.Min, new Vector3(box.Min.X, box.Min.Y, box.Max.Z), Color.Magenta);


        QueueDraw(counterCorner, new Vector3(box.Max.X, box.Min.Y, box.Min.Z), Color.Magenta);
        QueueDraw(counterCorner, new Vector3(box.Min.X, box.Min.Y, box.Max.Z), Color.Magenta);

        //top ring
        QueueDraw(box.Max, new Vector3(box.Min.X, box.Max.Y, box.Max.Z), Color.Magenta);
        QueueDraw(box.Max, new Vector3(box.Max.X, box.Max.Y, box.Min.Z), Color.Magenta);

        counterCorner = new Vector3(box.Min.X, box.Max.Y, box.Min.Z);
        QueueDraw(counterCorner, new Vector3(box.Min.X, box.Max.Y, box.Max.Z), Color.Magenta);
        QueueDraw(counterCorner, new Vector3(box.Max.X, box.Max.Y, box.Min.Z), Color.Magenta);

        QueueDraw(box.Min, box.Max, Color.Magenta);
    }

    public static void DrawDebugLines()
    {
        if (drawQueue.Count == 0) return;
        effect.View = Game1.camera.viewMatrix;
        VertexPositionColor[] vertices = new VertexPositionColor[drawQueue.Count * 2];
        int index = 0;

        foreach (DrawRequest request in drawQueue)
        {
            vertices[index++] = new VertexPositionColor(request.from, request.color);
            vertices[index++] = new VertexPositionColor(request.to, request.color);
        }

        using VertexBuffer buff = new VertexBuffer(gd, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
        buff.SetData(vertices);
        gd.SetVertexBuffer(buff);


        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            gd.DrawPrimitives(PrimitiveType.LineList, 0, vertices.Length / 2);
        }
        drawQueue.Clear();
    }
}
