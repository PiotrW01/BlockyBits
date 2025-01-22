using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

internal class Movement: Component
{
    public Vector3 velocity = Vector3.Zero;
    float acceleration = 1.0f;
    float maxSpeed = 5f;
    float friction = 5.0f;
    int prevScrollValue = 0;
    private float g = 0.81f;
    bool isFlying = true;

    public override void Start()
    {
        base.Start();
    }

    public override void HandleInput(float deltaTime)
    {
        if (BlockyBits.src.Keyboard.IsKeyJustPressed(Keys.L)) 
        { 
            isFlying = !isFlying;
        }
    }

    public override void Update(float deltaTime)
    {
        if (!isFlying) 
        {
            velocity = Vector3.Lerp(velocity, velocity - new Vector3(0, g, 0), deltaTime);
        }

        KeyboardState state = Keyboard.GetState();
        Vector3 move = Vector3.Zero;
        if (state.IsKeyDown(Keys.A))
        {
            move.X += -1;
        }
        if (state.IsKeyDown(Keys.D))
        {
            move.X += 1;
        }
        if (state.IsKeyDown(Keys.S))
        {
            move.Z += 1;
        }
        if (state.IsKeyDown(Keys.W))
        {
            move.Z += -1;
        }
        if (state.IsKeyDown(Keys.Space))
        {
            velocity = Vector3.Lerp(velocity, velocity + new Vector3(0, 0.8f, 0), deltaTime);
        }
        if (state.IsKeyDown(Keys.LeftShift))
        {
            velocity = Vector3.Lerp(velocity, velocity + new Vector3(0, -0.8f, 0), deltaTime);
        }
        if (move.Z == 0 && move.X == 0)
        {
            velocity = Vector3.Lerp(velocity, Vector3.Zero, deltaTime * friction);
        } else
        {
            Matrix yawMatrix = Matrix.CreateFromYawPitchRoll(Game1.camera.rotation.Y, 0, 0);
            Vector3 direction = Vector3.Transform(move, yawMatrix);
            //velocity = Vector3.Lerp(velocity, direction * maxSpeed, deltaTime * acceleration);
            velocity = Utils.EaseInOut(velocity, direction * maxSpeed, deltaTime * acceleration);
        }
        if (Utils.CollidesWithBlockAt(owner.pos + velocity))
        {
            velocity = Vector3.Zero;
        }
        owner.pos += velocity;
    }

    public override void HandleMouseInput(float deltaTime, Vector2 mouseVec)
    {
    }

}
