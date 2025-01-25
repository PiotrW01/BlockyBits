using BlockyBitsClient.src;
using BlockyBitsClient.src.Managers;
using BlockyBitsClient.src.player;
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
    private Inventory inventory;
    private Hotbar hotbar;

    bool thirdPerson = false;

    public override void Start()
    {
        inventory = new Inventory(12);
        hotbar = new Hotbar(9);
        camera = Game1.camera;
        camera.localPos = Vector3.Up * 0.7f;

        AddChild(hotbar);
        AddChild(camera);
        Collider collider = new Collider();
        collider.SetSize(0.9f, 0.9f, 1.8f);
        AddComponent(collider);
        AddComponent<Movement>();
    }

    public override void HandleInput(float deltaTime)
    {
        if (BlockyBits.src.Input.IsKeyJustPressed(Keys.F5))
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
