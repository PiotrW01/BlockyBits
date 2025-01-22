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
    static List<Object> gameObjects = new List<Object>(10);
    public static Dictionary<Vector2, Chunk> chunks = new();
    public double elapsedTime = 0f;
    public static Game1 game;
    SpriteFont font;

    public Game1()
    {
        if(game == null)
        {
            game = this;
        }
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Debugger.Enable(GraphicsDevice);
        camera = new Camera(GraphicsDevice);
        gameObjects.Add(camera);
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.ApplyChanges();

        gameObjects.Add(new Player());

        foreach (Object obj in gameObjects) 
        {
            obj.Start();
            obj.ComponentStart();
        }


        screenCenter = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        Mouse.SetPosition(screenCenter.X, screenCenter.Y);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        TextureAtlas.SetAtlas(Content.Load<Texture2D>("textures/texture_atlas"));
        Block.InitializeBlocks();
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        LoadObjectContent();
        font = Content.Load<SpriteFont>("font");
        chunks.Add(new Vector2(0, 0), new Chunk(0, 0));
        chunks.Add(new Vector2(0, 1), new Chunk(0, 1));
        chunks.Add(new Vector2(-1, 0), new Chunk(-1, 0));
        chunks.Add(new Vector2(0, -1), new Chunk(0, -1));
        chunks.Add(new Vector2(-1, -1), new Chunk(-1, -1));

    }

    protected override void Update(GameTime gameTime)
    {
        elapsedTime = gameTime.TotalGameTime.TotalSeconds;
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
        GraphicsDevice.SamplerStates[0] = new SamplerState()
        {
            Filter = TextureFilter.Point
        };
        foreach (Object obj in gameObjects)
        {
            //BoundingFrustum frustum = new(camera.viewMatrix * camera.projectionMatrix);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            obj.Render();
        }

        foreach(Chunk chunk in chunks.Values)
        {
            chunk.Render();
        }

        _spriteBatch.Begin();
        _spriteBatch.DrawString(font, "hello!", new Vector2(20, 20), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void HandleInput(float delta)
    {
        BlockyBits.src.Keyboard.UpdateKeyState();
        if (Keyboard.GetState().GetPressedKeyCount() > 0)
        {
            foreach (Object obj in gameObjects)
            {
                obj.HandleInput(delta);
                obj.HandleComponentInput(delta);
            }
        }

        if (Mouse.GetState().Position != screenCenter && IsActive)
        {
            Point p = screenCenter - Mouse.GetState().Position;
            Vector2 mouseVec = p.ToVector2();
            foreach (Object obj in gameObjects)
            {
                obj.HandleMouseInput(delta, mouseVec);
                obj.HandleComponentMouseInput(delta, mouseVec);
            }
            Mouse.SetPosition(screenCenter.X, screenCenter.Y);
        }
    }

    private void UpdateObjects(float delta)
    {
        foreach (Object obj in gameObjects)
        {
            obj.ForceUpdate(delta);
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
