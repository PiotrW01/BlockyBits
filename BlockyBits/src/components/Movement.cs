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
    Collider collider;

    public override void Start()
    {
        collider = owner.GetComponent<Collider>();

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
        if (isFlying){
            if (state.IsKeyDown(Keys.Space))
            {
                velocity = Vector3.Lerp(velocity, velocity + new Vector3(0, 0.8f, 0), deltaTime);
            }
            if (state.IsKeyDown(Keys.LeftShift))
            {
                velocity = Vector3.Lerp(velocity, velocity + new Vector3(0, -0.8f, 0), deltaTime);
            }
        } else {
            if (BlockyBits.src.Keyboard.IsKeyJustPressed(Keys.Space))
            {
                velocity = velocity + new Vector3(0, 0.4f, 0);
            }
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

        // BoundingBox

        // 0,1,4,5 Y+
        // 2,3,6,7 Y-

        // 1,2,5,6 X+
        // 0,3,4,7 X-

        // 0,1,2,3 Z+
        // 4,5,6,7 Z-

        BoundingBox playerBox = collider.box;
        Vector3[] corners = playerBox.GetCorners();
        foreach (Vector3 corner in corners)
        {
            BoundingBox blockBox = Utils.GetBoundingBoxAt(corner + velocity);
            Debugger.QueueDraw(blockBox);
        }

        //******** Y component ********//
        Vector3 vComponent = velocity * Vector3.Up;
        // bottom collider
        if (Utils.WorldToGridCoord(corners[2]) != Utils.WorldToGridCoord(corners[2] + vComponent))
        {
            if (vComponent.Y < 0f && Collides(corners[2], corners[3], corners[6], corners[7], vComponent)){
                velocity = velocity * new Vector3(1f, 0, 1f);
            }
        }
        //top collider
        if (Utils.WorldToGridCoord(corners[0]) != Utils.WorldToGridCoord(corners[0] + vComponent))
        {
            if (vComponent.Y > 0f && Collides(corners[0], corners[1], corners[4], corners[5], vComponent))
            {
                velocity = velocity * new Vector3(1f, 0, 1f);
            }
        }

        //******** X component ********//
        vComponent = velocity * Vector3.Right;
        // right collider
        if (Utils.WorldToGridCoord(corners[1]) != Utils.WorldToGridCoord(corners[1] + vComponent))
        {
            if (vComponent.X > 0f && Collides(corners[1], corners[2], corners[5], corners[6], vComponent))
            {
                velocity = velocity * new Vector3(0, 1f, 1f);
            }
        }
        // left collider
        if (Utils.WorldToGridCoord(corners[0]) != Utils.WorldToGridCoord(corners[0] + vComponent))
        {
            if (vComponent.X < 0f && Collides(corners[0], corners[3], corners[4], corners[7], vComponent))
            {
                velocity = velocity * new Vector3(0, 1f, 1f);
            }
        }

        //******** Z component ********//
        vComponent = velocity * Vector3.Backward;
        // front collider
        if (Utils.WorldToGridCoord(corners[0]) != Utils.WorldToGridCoord(corners[0] + vComponent))
        {
            if (vComponent.Z > 0f && Collides(corners[0], corners[1], corners[2], corners[3], vComponent))
            {
                velocity = velocity * new Vector3(1f, 1f, 0);
            }
        }
        // back collider
        if (Utils.WorldToGridCoord(corners[4]) != Utils.WorldToGridCoord(corners[4] + vComponent))
        {
            if (vComponent.Z < 0f && Collides(corners[4], corners[5], corners[6], corners[7], vComponent))
            {
                velocity = velocity * new Vector3(1f, 1f, 0);
            }
        }

        owner.pos += velocity;
    }

    public override void HandleMouseInput(float deltaTime, Vector2 mouseVec)
    {
    }

    private bool Collides(Vector3 corner1, Vector3 corner2, Vector3 corner3, Vector3 corner4, Vector3 vComponent)
    {
        if (Utils.CollidesWithBlockAt(corner1 + vComponent) ||
            Utils.CollidesWithBlockAt(corner2 + vComponent) ||
            Utils.CollidesWithBlockAt(corner3 + vComponent) ||
            Utils.CollidesWithBlockAt(corner4 + vComponent))
        {
            return true;
        }
        return false;
    }
}
