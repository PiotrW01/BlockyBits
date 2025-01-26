using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class GameObject: Object
{
    public Model model;

    public GameObject() 
    {
    }

    public override void Start()
    {
    }

    public override void Render()
    {
        if (model == null) return;
        Matrix worldMatrix = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z) * Matrix.CreateTranslation(pos);
        foreach (ModelMesh mesh in model.Meshes)
        {
            foreach (BasicEffect effect in mesh.Effects)
            {
                effect.World = worldMatrix;
                effect.View = Game1.camera.viewMatrix;
                effect.Projection = Game1.camera.projectionMatrix;
                effect.EnableDefaultLighting();
            }
            mesh.Draw();
        }
    }

    public override void Update(float deltaTime)
    {

    }

    public override void LoadContent(ContentManager cm)
    {
        //model = cm.Load<Model>("cube");
    }
}
