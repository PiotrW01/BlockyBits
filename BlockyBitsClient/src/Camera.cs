using BlockyBitsClient.src;
using Microsoft.VisualBasic.Logging;
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
    GraphicsDevice gd;

    private float _fov = MathHelper.Pi / 3f;
    public float FOV
    {
        get { return _fov; }
        set 
        {
            _fov = value;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(_fov, gd.Viewport.AspectRatio, 0.1f, 1000f);
        }
    }
    public Matrix viewMatrix;
    public Matrix projectionMatrix;

    public Camera(GraphicsDevice gd)
    {
        this.gd = gd;
        Transform.GlobalPosition = new Vector3(0, 10, 0);
        viewMatrix = Matrix.CreateLookAt(Transform.GlobalPosition, forward, cameraUp);
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

    public override void HandleMouseMove(float deltaTime, Vector2 mouseVec)
    {
        if (!Game1.game.mouseLocked) return;
        float pitch = Utils.DegToRad(maxPitch);

        Transform.EulerAngles = new Vector3(
            Math.Clamp(Transform.EulerAngles.X + mouseVec.Y * Settings.mouseSensitivity, -pitch, pitch),
            Transform.EulerAngles.Y + mouseVec.X * Settings.mouseSensitivity,
            Transform.EulerAngles.Z
            );
    }

    private void UpdateViewMatrix()
    {
        Matrix rotationMatrix = Matrix.CreateFromQuaternion(Transform.Quaternion);
        forward = Vector3.Transform(Vector3.Forward, rotationMatrix);
        viewMatrix = Matrix.CreateLookAt(Transform.GlobalPosition, Transform.GlobalPosition + forward, cameraUp);
    }

}

