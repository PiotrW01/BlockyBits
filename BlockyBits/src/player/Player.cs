using BlockyBitsClient.src;
using BlockyBitsClient.src.gui;
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

        GUIManager.RegisterUIElement(new NoiseRenderer());

        AddChild(hotbar);
        AddChild(camera);
        Collider collider = new Collider();
        collider.SetSize(0.9f, 0.9f, 1.8f);
        AddComponent(collider);
        AddComponent<Movement>();
    }

    public override void Update(float deltaTime)
    {
        if (thirdPerson)
        {
            Matrix m = Matrix.CreateFromQuaternion(camera.Transform.Quaternion);
            Vector3 rotatedOffset = Vector3.Transform(new Vector3(0, 0f, 5.0f), m);
            camera.localPos = rotatedOffset;
            Matrix c = Matrix.CreateLookAt(camera.pos, pos, Vector3.Up);
            camera.viewMatrix = c;
        }
    }

    public override void HandleInput(float deltaTime)
    {
        if (BlockyBits.src.Input.IsKeyJustPressed(Keys.F5))
        {
            thirdPerson = !thirdPerson;
        }
    }

    public override void LoadContent(ContentManager cm)
    {
            
    }
}
