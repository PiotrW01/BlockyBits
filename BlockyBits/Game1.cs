using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    Point screenCenter;
    public static Camera camera;
    GameObject obj;
    static List<Object> gameObjects = new List<Object>(10);
    SpriteFont font;
    Texture2D prototypeTexture;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Debugger.Enable(GraphicsDevice);


        camera = new Camera(GraphicsDevice);
        obj = new GameObject();
        gameObjects.Add(camera);

        Random rand = new Random();
        for (int i = 0; i < 2; i++)
        {
            GameObject go = new GameObject();
            go.pos = new Vector3(0, 0, i * 5);
            go.scale = new Vector3(rand.NextSingle()*5f);
            gameObjects.Add(go);
        }


        foreach (Object obj in gameObjects) 
        {
            obj.Start();
        }


        screenCenter = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        Mouse.SetPosition(screenCenter.X, screenCenter.Y);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        LoadObjectContent();
        font = Content.Load<SpriteFont>("font");
        prototypeTexture = Content.Load<Texture2D>("grey_512");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        float delta = ((float)gameTime.ElapsedGameTime.TotalSeconds);
        HandleInput(delta);
        UpdateObjects(delta);


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

        GraphicsDevice.Clear(Color.CornflowerBlue);

        foreach (Object obj in gameObjects)
        {
            BoundingFrustum frustum = new(camera.viewMatrix * camera.projectionMatrix);
            obj.Render();
        }


        _spriteBatch.Begin();
        _spriteBatch.DrawString(font, "hello!", new Vector2(20, 20), Color.White);
        _spriteBatch.End();


        //Debugger.DrawLine(Vector3.Zero, new Vector3(0, 0, 30), Color.Red);



        base.Draw(gameTime);
    }


    private void CheckCollisions()
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            if (gameObjects[i].collider != null)
            {
                for (int j = i; j < gameObjects.Count; j++)
                {
                    //gameObjects[i].collider.CollidesWith(gameObjects[j].collider);
                }
            }
        }


        foreach (var obj in gameObjects)
        {
            if(obj.collider != null)
            {

            }
        }
    }

    private void HandleInput(float delta)
    {
        if (Keyboard.GetState().GetPressedKeyCount() > 0)
        {
            foreach (Object obj in gameObjects)
            {
                obj.HandleInput(delta);
            }
        }

        if (Mouse.GetState().Position != screenCenter && IsActive)
        {
            Point p = screenCenter - Mouse.GetState().Position;
            Vector2 mouseVec = p.ToVector2();
            foreach (Object obj in gameObjects)
            {
                obj.HandleMouseInput(delta, mouseVec);
            }


            Mouse.SetPosition(screenCenter.X, screenCenter.Y);
        }
    }

    private void UpdateObjects(float delta)
    {
        foreach (Object obj in gameObjects)
        {
            obj.Update(delta);
        }
    }
    private void LoadObjectContent()
    {
        foreach (var obj in gameObjects)
        {
            obj.LoadContent(Content);
        }
    }
}
