using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

public class Player: GameObject
{
    private Camera camera;
    bool thirdPerson = false;

    public override void Start()
    {
        camera = Game1.camera;
        children.Add(camera);
        camera.localPos = Vector3.Up * 0.7f;
        AddComponent<Movement>();
        Collider collider = new Collider();
        collider.SetSize(0.9f, 0.9f, 1.8f);
        AddComponent(collider);
        AddComponent(new Collider());
        AddComponent<Collider>();
    }

    public override void HandleInput(float deltaTime)
    {
        if (BlockyBits.src.Keyboard.IsKeyJustPressed(Keys.F5))
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


    public override void Update(float deltaTime)
    {
        //Debug.WriteLine($"player pos: {pos}, local pos: {localPos}, camera pos: {camera.pos}, camera local pos: {camera.localPos}");
    }

    public override void LoadContent(ContentManager cm)
    {
            
    }
}
