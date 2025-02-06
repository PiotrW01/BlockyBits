using BlockyBits.src;
using BlockyBitsClient.src;
using BlockyBitsClient.src.gui;
using BlockyBitsClient.src.Managers;
using BlockyBitsClient.src.player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
        camera = Game1.MainCamera;
        camera.Transform.Position = Vector3.Up * 0.7f;
        //AddChild(new Hand());
        GUIManager.RegisterUIElement(new NoiseRenderer());
        AddChild(hotbar);
        AddChild(camera);
        camera.AddChild(new Hand());
        Collider collider = new Collider();
        collider.SetSize(0.9f, 0.9f, 1.8f);
        AddComponent(collider);
        AddComponent<PlayerMovement>();
    }

    public override void Update(float deltaTime)
    {
        var drops = ObjectManager.FindClosestItems(Transform.GlobalPosition, 3);
        foreach (DroppedItem drop in drops)
        {
            if(drop.TryGetItem(out var item))
            {
                if (!hotbar.AddItem(new Item("dirt")))
                {
                    break;
                }
            }
        }

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

        if (Input.IsKeyJustPressed(Keys.Q))
        {
            ObjectManager.CreateDroppedItem(new("dirt"), Transform.GlobalPosition);
            hotbar.hotbarItems[hotbar.selectedSlot] = null;
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

    public override void Render()
    {


        if (!isBlockReal) return;
        Matrix worldMatrix = Matrix.CreateTranslation(-0.5f, -0.5f, -0.5f) * Matrix.CreateScale(1.001f) * Matrix.CreateTranslation(lookingAtBlock + new Vector3(0.5f,0.5f,0.5f));
        ModelShape shape = Block.GetBlockProperties("outline").ModelShape;

        using VertexBuffer vbuffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionNormalTexture), shape.vertices.Count, BufferUsage.WriteOnly);
        using IndexBuffer ibuffer = new IndexBuffer(Game1.game.GraphicsDevice, IndexElementSize.ThirtyTwoBits, shape.indices.Count, BufferUsage.WriteOnly);

        vbuffer.SetData(shape.vertices.ToArray());
        ibuffer.SetData(shape.indices.ToArray());

        Shaders.OutlineShader.Parameters["World"].SetValue(worldMatrix);
        Shaders.OutlineShader.Parameters["View"].SetValue(Game1.MainCamera.viewMatrix);
        Shaders.OutlineShader.Parameters["Projection"].SetValue(Game1.MainCamera.projectionMatrix);

        Game1.game.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
        Game1.game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        foreach (EffectPass pass in Shaders.OutlineShader.CurrentTechnique.Passes)
        {
            pass.Apply();
            Game1.game.GraphicsDevice.SetVertexBuffer(vbuffer);
            Game1.game.GraphicsDevice.Indices = ibuffer;
            Game1.game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, shape.indices.Count / 3);
        }



    }

    public override void LoadContent(ContentManager cm)
    {
        
    }

    public void Ray(Vector3 startPos, Vector3 direction, int reach, Action<Vector3> callback)
    {
        float step = 0.02f;
        direction.Normalize();
        Vector3 endPos = startPos + direction * reach;
        Vector3 start = startPos;
        Vector3 endGridCoord = new Vector3(abc(endPos.X), abc(endPos.Y), abc(endPos.Z));

        Vector3 currentGridCoord = new Vector3(abc(startPos.X), abc(startPos.Y), abc(startPos.Z));
        Vector3 difference = endPos - startPos;

        float xStep = difference.X * step;
        float yStep = difference.Y * step;
        float zStep = difference.Z * step;
        Vector3 stepVector = new(xStep, yStep, zStep);

        while (endGridCoord != currentGridCoord && Vector3.Distance(start, currentGridCoord) <= reach)
        {
            previousBlock = currentGridCoord;
            startPos.X += stepVector.X;
            Vector3 nextGridCoord = new Vector3(abc(startPos.X), abc(startPos.Y), abc(startPos.Z));
            if (nextGridCoord != currentGridCoord)
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
            if (nextGridCoord != currentGridCoord)
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
            if (nextGridCoord != currentGridCoord)
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
        ChunkManager.RemoveBlockAt(globalCoord);
    }

    private void PlaceBlockAt(Vector3 globalCoord)
    {
        ChunkManager.PlaceBlockAt(globalCoord, new Block("thing"));
    }

    private int abc(float x)
    {
        return (int)Math.Floor(x);
    }
}
