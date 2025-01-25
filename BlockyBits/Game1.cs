using BlockyBits.src;
using BlockyBitsClient.src;
using BlockyBitsClient.src.gui;
using BlockyBitsClient.src.Managers;
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
    public SpriteBatch spriteBatch;
    public Point screenCenter;
    public static Camera camera;
    
    
    public double elapsedTime = 0f;
    public static Game1 game;
    public static Player player;
    public TcpClient client;
    int frameCounter = 0;
    int fps = 0;
    double timeSinceLastUpdate = 0;
    public bool mouseLocked = true;

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
        camera = new Camera(GraphicsDevice);
        ObjectManager.Add(camera);
        ChunkManager.Start();
        Debugger.Enable(GraphicsDevice);
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.ApplyChanges();

        //client = new TcpClient("localhost", Network.serverPort);
        screenCenter = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        Mouse.SetPosition(screenCenter.X, screenCenter.Y);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        TextureAtlas.LoadAtlas(Content);
        Globals.LoadContent(Content);
        Block.InitializeBlocks();
        GUIManager.RegisterUIElement(new MainMenu());
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        timeSinceLastUpdate += gameTime.ElapsedGameTime.TotalSeconds;
        frameCounter++;
        if(timeSinceLastUpdate >= 1)
        {
            fps = frameCounter;
            frameCounter = 0;
            timeSinceLastUpdate = 0;
        }
        
        elapsedTime = gameTime.TotalGameTime.TotalSeconds;
        float delta = ((float)gameTime.ElapsedGameTime.TotalSeconds);
        HandleInput(delta);
        ObjectManager.UpdateObjects(delta);
        ChunkManager.UpdateChunks();
        

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

        GraphicsDevice.Clear(Color.CornflowerBlue);
        GraphicsDevice.SamplerStates[0] = new SamplerState()
        {
            Filter = TextureFilter.Point
        };
        ChunkManager.RenderChunks();

        ObjectManager.RenderObjects();
        Debugger.DrawDebugLines();
        spriteBatch.Begin();
        RenderDebugInfo();
        spriteBatch.End();
        GUIManager.RenderGUI(spriteBatch);

        base.Draw(gameTime);
    }

    private void HandleInput(float delta)
    {
        BlockyBits.src.Input.UpdateState();
        GUIManager.CheckMouseEvents();


        if (BlockyBits.src.Input.IsKeyJustPressed(Keys.F11))
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
        if (BlockyBits.src.Input.IsKeyJustPressed(Keys.F3))
        {
            Debugger.showDebugInfo = !Debugger.showDebugInfo;
        }

        if (BlockyBits.src.Input.IsKeyJustPressed(Keys.F4))
        {
            mouseLocked = !mouseLocked;
        }

        if (Keyboard.GetState().GetPressedKeyCount() > 0)
        {
            ObjectManager.HandleInput(delta);
        }

        if(Input.GetScrollDirection() != 0)
        {
            ObjectManager.HandleScrollInput();
        }

        if (Mouse.GetState().Position != screenCenter && IsActive)
        {
            Point p = screenCenter - Mouse.GetState().Position;
            Vector2 mouseVec = p.ToVector2();
            ObjectManager.HandleMouseInput(delta, mouseVec);

            if (mouseLocked)
            {
                Mouse.SetPosition(screenCenter.X, screenCenter.Y);
            }
        }
    }

    private void RenderDebugInfo()
    {
        if (!Debugger.showDebugInfo || player == null) return;
        spriteBatch.DrawString(Globals.font, $"Fps: {fps}", new Vector2(20, 20), Color.White);
        spriteBatch.DrawString(Globals.font, $"Coords: {{X: {player.pos.X.ToString("F2")}, Y: {player.pos.Y.ToString("F2")}, Z: {player.pos.Z.ToString("F2")}}}", new Vector2(20, 40), Color.White);
        spriteBatch.DrawString(Globals.font, $"Chunk coords: {Utils.GetPlayerChunkPos()} at chunk: {Utils.WorldToChunkCoord(player.pos)}", new Vector2(20, 60), Color.White);
    }

    public void StartGame()
    {
        player = new Player();
        player.pos = new Vector3(33, 62, 70);
        ObjectManager.Add(player);
    }
}
