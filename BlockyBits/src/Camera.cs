using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

internal class Camera: Object
{
    Vector3 target = Vector3.Zero;
    Vector3 cameraUp = Vector3.Up;

    int prevScrollValue = 0;
    float speed = 10.0f;
    float maxPitch = 80f;

    public Matrix viewMatrix;
    public Matrix projectionMatrix;

    public Camera(GraphicsDevice gd)
    {
        pos = new Vector3(0, 0, 0);
        viewMatrix = Matrix.CreateLookAt(pos, target, cameraUp);
        projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            gd.Viewport.AspectRatio,
            0.1f,
            1000f
            );
    }

    public override void Update(float deltaTime)
    {
        UpdateViewMatrix();

        if(Mouse.GetState().ScrollWheelValue != prevScrollValue)
        {
            if (Mouse.GetState().ScrollWheelValue < prevScrollValue)
            {
                speed += 2f;
            }
            else if (Mouse.GetState().ScrollWheelValue > prevScrollValue)
            {
                speed -= 2f;
            }
            prevScrollValue = Mouse.GetState().ScrollWheelValue;
            Debug.WriteLine(speed);
        }

    }
    public override void HandleInput(float deltaTime)
    {
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
            move.Z += -1;
        }
        if (state.IsKeyDown(Keys.W))
        {
            move.Z += 1;
        }
        if (state.IsKeyDown(Keys.Space))
        {
            pos += new Vector3(0, 5 * deltaTime, 0);
        }
        if (state.IsKeyDown(Keys.LeftShift))
        {
            pos += new Vector3(0, 5 * -deltaTime, 0);
        }


        Matrix yawMatrix = Matrix.CreateFromYawPitchRoll(rotation.Y, 0, 0);
        Vector3 flatForward = Vector3.Transform(Vector3.Forward, yawMatrix) * move.Z;
        Vector3 flatRight = Vector3.Transform(Vector3.Right, yawMatrix) * move.X;
        pos += (flatForward + flatRight) * deltaTime * speed;
    }

    public override void HandleMouseInput(float deltaTime, Vector2 mouseVec)
    {
        rotation.Y += mouseVec.X * deltaTime;
        rotation.X += mouseVec.Y * deltaTime;
        float pitch = Utils.DegToRad(maxPitch);
        rotation.X = Math.Clamp(rotation.X, -pitch, pitch);
    }

    private void UpdateViewMatrix()
    {

        Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, 0);
        Vector3 forward = Vector3.Transform(Vector3.Forward, rotationMatrix);
        viewMatrix = Matrix.CreateLookAt(pos, pos + forward, cameraUp);
    }

}

