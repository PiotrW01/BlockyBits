using BlockyBits.src;
using BlockyBitsCommon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;

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
    public static Player player;
    public TcpClient client;
    SpriteFont font;
    int frameCounter = 0;
    int fps = 0;
    double timeSinceLastUpdate = 0;


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

        player = new Player();
        gameObjects.Add(player);

        foreach (Object obj in gameObjects) 
        {
            obj.Start();
            obj.ComponentStart();
        }

        client = new TcpClient("localhost", Network.serverPort);



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
        WorldGenerator gen = new(42);
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                chunks.Add(new Vector2(i,j),gen.GenerateChunk(i, j));
            }
        }

        int vertices = 0;
        foreach (Chunk chunk in chunks.Values)
        {
            chunk.GenerateMesh();
            vertices += chunk.vertexCount;
        }
        Debug.WriteLine(vertices);
    }

    protected override void Update(GameTime gameTime)
    {
        timeSinceLastUpdate += gameTime.ElapsedGameTime.TotalSeconds;
        frameCounter++;
        if(timeSinceLastUpdate >= 1)
        {
            fps = frameCounter;
            frameCounter = 0;
            timeSinceLastUpdate = 0;
        }
        
        
        elapsedTime = gameTime.TotalGameTime.TotalSeconds;
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.Escape))
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
        RenderDebugInfo();
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void HandleInput(float delta)
    {
        BlockyBits.src.Keyboard.UpdateKeyState();
        if (BlockyBits.src.Keyboard.IsKeyJustPressed(Keys.F11))
        {
            if (_graphics.IsFullScreen)
            {
                _graphics.PreferredBackBufferHeight = 720;
                _graphics.PreferredBackBufferWidth = 1280;
            } else
            {
                _graphics.HardwareModeSwitch = false;
                _graphics.PreferredBackBufferHeight = 1080;
                _graphics.PreferredBackBufferWidth = 1920;
            }
            _graphics.ToggleFullScreen();
            _graphics.ApplyChanges();
        }
        if (BlockyBits.src.Keyboard.IsKeyJustPressed(Keys.F3))
        {
            Debugger.showDebugInfo = !Debugger.showDebugInfo;
        }

        if (Microsoft.Xna.Framework.Input.Keyboard.GetState().GetPressedKeyCount() > 0)
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
            obj.UpdateChildrenAndComponents(delta);
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

    private void RenderDebugInfo()
    {
        if (!Debugger.showDebugInfo) return;
        _spriteBatch.DrawString(font, $"Fps: {fps}", new Vector2(20, 20), Color.White);
        _spriteBatch.DrawString(font, $"Coords: {{X: {player.pos.X.ToString("F0")}, Y: {player.pos.Y.ToString("F0")}, Z: {player.pos.Z.ToString("F0")}}}", new Vector2(20, 40), Color.White);
        _spriteBatch.DrawString(font, $"Chunk coords: {Utils.GetPlayerChunkPos()} at chunk: {Utils.GetChunkAt(player.pos)}", new Vector2(20, 60), Color.White);
    }
}
