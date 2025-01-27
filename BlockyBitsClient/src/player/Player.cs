using BlockyBits.src;
using BlockyBitsClient.src;
using BlockyBitsClient.src.gui;
using BlockyBitsClient.src.Managers;
using BlockyBitsClient.src.player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

public class Player: GameObject
{
    private Camera camera;
    private Inventory inventory;
    private Hotbar hotbar;
    public Vector3 lookingAtBlock;
    private Vector3 previousBlock;
    public bool isBlockReal = false;

    bool thirdPerson = false;

    public override void Start()
    {
        inventory = new Inventory(12);
        hotbar = new Hotbar(9);
        camera = Game1.camera;
        camera.Transform.Position = Vector3.Up * 0.7f;

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
            camera.Transform.Position = rotatedOffset;
            Matrix c = Matrix.CreateLookAt(camera.Transform.GlobalPosition, Transform.GlobalPosition, Vector3.Up);
            camera.viewMatrix = c;
        }

        Ray(camera.Transform.GlobalPosition, camera.forward, 5, (blockPos) =>
        {
            lookingAtBlock = blockPos;
            isBlockReal = true;
            Debugger.QueueDraw(new BoundingBox(lookingAtBlock, lookingAtBlock + Vector3.One));
        });
    }

    public override void HandleInput(float deltaTime)
    {
        if (Input.IsKeyJustPressed(Keys.F5))
        {
            thirdPerson = !thirdPerson;
            if (!thirdPerson)
            {
                camera.Transform.Position = Vector3.Up * 0.7f;
            }
        }
    }

    public override void HandleMouseClick()
    {
        if (isBlockReal)
        {
            if (Input.IsLMBJustPressed())
            {
                RemoveBlockAt(lookingAtBlock);
            }
            else if (Input.IsRMBJustPressed())
            {
                PlaceBlockAt(previousBlock);
            }
        }


    }

    private void Ray(Vector3 startPos, Vector3 direction, int reach, Action<Vector3> callback)
    {
        float step = 0.02f;
        direction.Normalize();
        Vector3 endPos = startPos + direction * reach;

        Vector3 endGridCoord = new Vector3(abc(endPos.X), abc(endPos.Y), abc(endPos.Z));

        Vector3 currentGridCoord = new Vector3(abc(startPos.X), abc(startPos.Y), abc(startPos.Z));
        Vector3 difference = endPos - startPos;

        float xStep = difference.X * step;
        float yStep = difference.Y * step;
        float zStep = difference.Z * step;
        Vector3 stepVector = new(xStep, yStep, zStep);

        while(endGridCoord != currentGridCoord)
        {
            previousBlock = currentGridCoord;
            startPos.X += stepVector.X;
            Vector3 nextGridCoord = new Vector3(abc(startPos.X), abc(startPos.Y), abc(startPos.Z));
            if(nextGridCoord != currentGridCoord)
            {
                currentGridCoord = nextGridCoord;
                if (Utils.CollidesWithBlockAt(currentGridCoord))
                {
                    callback(currentGridCoord);
                    return;
                }
            }            
            startPos.Y += stepVector.Y;
            nextGridCoord = new Vector3(abc(startPos.X), abc(startPos.Y), abc(startPos.Z));
            if(nextGridCoord != currentGridCoord)
            {
                currentGridCoord = nextGridCoord;
                if (Utils.CollidesWithBlockAt(currentGridCoord))
                {
                    callback(currentGridCoord);
                    return;
                }
            }            
            startPos.Z += stepVector.Z;
            nextGridCoord = new Vector3(abc(startPos.X), abc(startPos.Y), abc(startPos.Z));
            if(nextGridCoord != currentGridCoord)
            {
                currentGridCoord = nextGridCoord;
                if (Utils.CollidesWithBlockAt(currentGridCoord))
                {
                    callback(currentGridCoord);
                    return;
                }
            }

            lookingAtBlock = new Vector3(0, 0, 0);
            isBlockReal = false;
        }
    }

    private void RemoveBlockAt(Vector3 globalCoord)
    {
        ChunkManager.chunks[Utils.WorldToChunkPosition(globalCoord)].RemoveBlock(Utils.WorldToLocalChunkCoord(globalCoord));
    }

    private void PlaceBlockAt(Vector3 globalCoord)
    {
        ChunkManager.chunks[Utils.WorldToChunkPosition(globalCoord)].SetBlock(Utils.WorldToLocalChunkCoord(globalCoord), new Block(Block.Type.Stone));
    }

    private int abc(float x)
    {
        return (int)Math.Floor(x);
    }

    private void RayCast(Vector3 startPos, Vector3 direction, int reach)
    {
        direction = Vector3.Normalize(direction); // Normalize direction vector

        Vector3 pos = startPos;
        Vector3 step = new Vector3(
            direction.X > 0 ? 1 : -1,
            direction.Y > 0 ? 1 : -1,
            direction.Z > 0 ? 1 : -1
        );

        Vector3 gridPos = Utils.WorldToLocalChunkCoord(startPos); // Convert to integer grid space

        Vector3 tMax = new Vector3(
            (step.X > 0 ? (MathF.Floor(pos.X) + 1 - pos.X) : (pos.X - MathF.Floor(pos.X))) / MathF.Abs(direction.X),
            (step.Y > 0 ? (MathF.Floor(pos.Y) + 1 - pos.Y) : (pos.Y - MathF.Floor(pos.Y))) / MathF.Abs(direction.Y),
            (step.Z > 0 ? (MathF.Floor(pos.Z) + 1 - pos.Z) : (pos.Z - MathF.Floor(pos.Z))) / MathF.Abs(direction.Z)
        );

        Vector3 tDelta = new Vector3(
            MathF.Abs(1 / direction.X),
            MathF.Abs(1 / direction.Y),
            MathF.Abs(1 / direction.Z)
        );

        for (int i = 0; i <= reach; i++)
        {
            // Check for collision with block at current grid position
            if (Utils.CollidesWithBlockAt(gridPos))
            {
                ChunkManager.chunks[Utils.WorldToChunkPosition(gridPos)].RemoveBlock(gridPos);
                Debug.WriteLine("collision at reach " + i);
                break;
            }

            // Move to the next voxel along the ray
            if (tMax.X < tMax.Y && tMax.X < tMax.Z)
            {
                tMax.X += tDelta.X;
                gridPos.X += step.X;
            }
            else if (tMax.Y < tMax.Z)
            {
                tMax.Y += tDelta.Y;
                gridPos.Y += step.Y;
            }
            else
            {
                tMax.Z += tDelta.Z;
                gridPos.Z += step.Z;
            }
        }
    }

}
