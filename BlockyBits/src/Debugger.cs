using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

internal class Debugger
{
    private static GraphicsDevice gd;
    public static bool showDebugInfo = false;

    public static void Enable(GraphicsDevice gd)
    {
        Debugger.gd = gd;
    }

    public static void DrawLine(Vector3 from, Vector3 to, Color color)
    {
        BasicEffect effect = new BasicEffect(gd)
        {
            VertexColorEnabled = true,
            World = Matrix.Identity,
            View = Game1.camera.viewMatrix,
            Projection = Game1.camera.projectionMatrix,
        };



        VertexPositionColor[] vertices = new VertexPositionColor[]
        {
            new VertexPositionColor(from, color),
            new VertexPositionColor(to, color)
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
}
