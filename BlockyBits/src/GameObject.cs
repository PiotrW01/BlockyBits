using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Net.Mime;

public class GameObject: Object
{
    public Matrix worldMatrix;
    public Model model;
    public Quaternion quaternion = Quaternion.Identity;


    public GameObject() 
    {
        worldMatrix = Matrix.CreateTranslation(pos) * Matrix.CreateScale(scale);
        collider = new BoxCollider();
        collider.SetOwner(this);
    }

    public override void Start()
    {
        foreach (var c in components)
        {
            c.Start();
        }
        AddComponent<Gravity>();
    }

    public override void Render()
    {
        if (model == null) return;
        worldMatrix = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z) * Matrix.CreateTranslation(pos);
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
        collider.Draw();
    }

    public override void Update(float deltaTime)
    {
        foreach (var c in components)
        {
            c.Update(deltaTime);
        }
        Debug.WriteLine(pos);
    }

    public override void LoadContent(ContentManager cm)
    {
        model = cm.Load<Model>("cube");
        collider.SetSize(1, 1, 1);




        foreach (var c in components)
        {
            c.LoadContent();
        }
    }

    public void AddComponent<T>() where T : Component, new()
    {
        T c = new T();
        c.SetOwner(this);
        components.Add(c);
    }
}
