using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

internal class Movement: Component
{
    public Vector3 velocity = Vector3.Zero;
    float acceleration = 8.0f;
    float maxSpeed = 20f;
    float friction = 3.0f;
    private float g = 14.81f;
    private float maxGravityAcceleration = 50f;
    private float maxVerticalFlySpeed = 20f;
    private float jumpForce;
    bool isFlying = true;
    Collider collider;
    bool isGrounded = true;
    public override void Start()
    {
        collider = owner.GetComponent<Collider>();

        // calc jump force to jump up to 1.2 blocks based on the gravity
        float jumpHeight = 1.4f;
        //jumpForce = (float)Math.Sqrt(jumpHeight*2 * (Math.Abs(g) * Game1.game.TargetElapsedTime.TotalSeconds));
        jumpForce = (float)Math.Sqrt(jumpHeight*2 * Math.Abs(g));


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
        KeyboardState state = Keyboard.GetState();
        Vector3 move = GetXYMovement(state);

        //****** VERTICAL MOVEMENT ******//
        if (!isFlying) 
        {
            //apply gravity
            velocity.Y -= g * deltaTime;
            velocity.Y = Math.Max(-maxGravityAcceleration, velocity.Y);

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && isGrounded)
            {
                velocity.Y += jumpForce;
                isGrounded = false;
                //Debug.WriteLine("Jumped at y: " + owner.pos.Y);
            }
        } 
        else 
        {
            float y = 0f;
            if (state.IsKeyDown(Keys.Space))
            {
                y += 0.8f;
            }
            if (state.IsKeyDown(Keys.LeftShift))
            {
                y -= 0.8f;
            }
            if(y != 0) // accelerate
            {
                velocity.Y = Utils.Lerp(velocity.Y, velocity.Y + y, deltaTime * acceleration);
                velocity.Y = Math.Clamp(velocity.Y, -maxVerticalFlySpeed, maxVerticalFlySpeed);
            } else // slow down
            {
                velocity.Y = Utils.Lerp(velocity.Y, 0, deltaTime * 10);
            }
            //Debug.WriteLine(velocity.Y);
        }

        //****** HORIZONTAL MOVEMENT ******//
        if (move.Z == 0 && move.X == 0)
        {
            velocity.X = Utils.Lerp(velocity.X, 0, deltaTime * friction);
            velocity.Z = Utils.Lerp(velocity.Z, 0, deltaTime * friction);
        } else
        {
            Matrix yawMatrix = Matrix.CreateFromYawPitchRoll(Game1.camera.rotation.Y, 0, 0);
            Vector3 direction = Vector3.Transform(move, yawMatrix);
            direction *= maxSpeed;
            direction.Y = velocity.Y;
            velocity = Utils.EaseInOut(velocity, direction, deltaTime * acceleration);
        }
        Vector3 deltaVelocity = ApplyCollisions(velocity * deltaTime);
        owner.pos += deltaVelocity;
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

    private Vector3 ApplyCollisions(Vector3 velocity)
    {
        // TODO: fix collision not detecting when running into edge of block
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
            BoundingBox? blockBox = Utils.GetBoundingBoxAt(corner + velocity);
            if(blockBox.HasValue)
            {
                Debugger.QueueDraw((BoundingBox)blockBox);
            }
        }

        //******** Y component ********//
        Vector3 vComponent = velocity * Vector3.Up;
        // bottom collider
        if (Utils.WorldToGridCoord(corners[2]) != Utils.WorldToGridCoord(corners[2] + vComponent))
        {
            if (vComponent.Y < 0f && Collides(corners[2], corners[3], corners[6], corners[7], vComponent))
            {
                velocity = velocity * new Vector3(1f, 0, 1f);
                this.velocity *= new Vector3(1f, 0, 1f);
                isGrounded = true;
            } else
            {
                isGrounded = false;
            }
        }
        //top collider
        if (Utils.WorldToGridCoord(corners[0]) != Utils.WorldToGridCoord(corners[0] + vComponent))
        {
            if (vComponent.Y > 0f && Collides(corners[0], corners[1], corners[4], corners[5], vComponent))
            {
                velocity = velocity * new Vector3(1f, 0, 1f);
                this.velocity *= new Vector3(1f, 0, 1f);
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
                this.velocity *= new Vector3(0, 1f, 1f);
            }
        }
        // left collider
        if (Utils.WorldToGridCoord(corners[0]) != Utils.WorldToGridCoord(corners[0] + vComponent))
        {
            if (vComponent.X < 0f && Collides(corners[0], corners[3], corners[4], corners[7], vComponent))
            {
                velocity = velocity * new Vector3(0, 1f, 1f);
                this.velocity *= new Vector3(0, 1f, 1f);
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
                 this.velocity *= new Vector3(1f, 1f, 0);
            }
        }
        // back collider
        if (Utils.WorldToGridCoord(corners[4]) != Utils.WorldToGridCoord(corners[4] + vComponent))
        {
            if (vComponent.Z < 0f && Collides(corners[4], corners[5], corners[6], corners[7], vComponent))
            {
                velocity = velocity * new Vector3(1f, 1f, 0);
                this.velocity *= new Vector3(1f, 1f, 0);
            }
        }
        return velocity;
    }

    private Vector3 GetXYMovement(KeyboardState state)
    {
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
        return move;
    }
}
