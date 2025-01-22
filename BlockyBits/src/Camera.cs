using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

public class Camera: Object
{
    public Vector3 forward = Vector3.Zero;
    Vector3 cameraUp = Vector3.Up;
    float maxPitch = 80f;

    public Matrix viewMatrix;
    public Matrix projectionMatrix;

    public Camera(GraphicsDevice gd)
    {
        pos = new Vector3(0, 10, 0);
        viewMatrix = Matrix.CreateLookAt(pos, forward, cameraUp);
        projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.Pi/3f,
            gd.Viewport.AspectRatio,
            0.1f,
            1000f
            );
    }

    public override void Update(float deltaTime)
    {
        UpdateViewMatrix();
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
        forward = Vector3.Transform(Vector3.Forward, rotationMatrix);
        viewMatrix = Matrix.CreateLookAt(pos, pos + forward, cameraUp);
    }

}

