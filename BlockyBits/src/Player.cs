using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

internal class Player: GameObject
{
    private Camera camera;
    bool thirdPerson = false;

    public override void Start()
    {
        collider = new Collider();
        collider.SetOwner(this);
        collider.SetSize(1, 1, 1);

        camera = Game1.camera;
        children.Add(camera);
        camera.localPos = Vector3.Up;
        AddComponent<Movement>();
    }

    public override void HandleInput(float deltaTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.F5))
        {
            thirdPerson = !thirdPerson;
            if (thirdPerson)
            {
                camera.localPos = new Vector3(0, 2, -3);
            }
            else
            {
                camera.localPos = Vector3.Up;
            }
        }
    }

    public override void Render()
    {
        collider.Draw();
    }

    public override void Update(float deltaTime)
    {
    }

    public override void LoadContent(ContentManager cm)
    {
            
    }
}
