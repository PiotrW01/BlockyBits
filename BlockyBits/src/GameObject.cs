using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using System.Threading.Tasks;

internal class GameObject: Object
{
    public Vector3 scale = Vector3.One;
    public Matrix worldMatrix;
    public Model model;
    public Quaternion quaternion = Quaternion.Identity;

    public GameObject() 
    {
        worldMatrix = Matrix.CreateWorld(pos, Vector3.Forward, Vector3.Up);
    }

    public override void Render(Camera c)
    {
        foreach (ModelMesh mesh in model.Meshes)
        {
            foreach (BasicEffect effect in mesh.Effects)
            {
                effect.World = worldMatrix;
                effect.View = c.viewMatrix;
                effect.Projection = c.projectionMatrix;
                effect.EnableDefaultLighting();
            }
            mesh.Draw();
        }
    }

    public override void Update(float deltaTime)
    {
        
    }
}
