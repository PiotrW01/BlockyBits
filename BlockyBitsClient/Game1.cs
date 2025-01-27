﻿using BlockyBits.src;
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
    public static RenderTarget2D renderTarget;
    
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
        //_graphics.GraphicsProfile = GraphicsProfile.HiDef;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.ApplyChanges();
        renderTarget = new(GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
        Shaders.WaterShader.Parameters["tex"].SetValue(renderTarget);
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
        Shaders.UpdateShaderParameters();
        
        GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Clear(Color.CornflowerBlue);
        GraphicsDevice.SamplerStates[1] = new SamplerState()
        {
            Filter = TextureFilter.Point
        };
        ChunkManager.RenderChunks();
        
        ChunkManager.RenderWater();
        ObjectManager.RenderObjects();
        GraphicsDevice.SetRenderTarget(null);

        spriteBatch.Begin();
        spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
        RenderDebugInfo();
        spriteBatch.End();

        Debugger.DrawDebugLines();
        GUIManager.RenderGUI(spriteBatch);

        base.Draw(gameTime);
    }

    private void HandleInput(float delta)
    {
        Input.UpdateState();
        GUIManager.CheckMouseEvents();


        if (Input.IsKeyJustPressed(Keys.F11))
        {
            renderTarget.Dispose();
            if (_graphics.IsFullScreen)
            {
                _graphics.PreferredBackBufferHeight = 720;
                _graphics.PreferredBackBufferWidth = 1280;
            }
            else
            {
                _graphics.HardwareModeSwitch = false;
                _graphics.PreferredBackBufferHeight = 1080;
                _graphics.PreferredBackBufferWidth = 1920;
            }
            _graphics.ToggleFullScreen();
            _graphics.ApplyChanges();
            renderTarget = new(GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
        }
        if (Input.IsKeyJustPressed(Keys.F3))
        {
            Debugger.showDebugInfo = !Debugger.showDebugInfo;
        }

        if (Input.IsKeyJustPressed(Keys.F4))
        {
            mouseLocked = !mouseLocked;
        }

        if (Keyboard.GetState().GetPressedKeyCount() > 0)
        {
            ObjectManager.HandleInput(delta);
        }

        if (Input.IsMouseClickChanged())
        {
            ObjectManager.HandleMouseClick();
        }

        if(Input.GetScrollDirection() != 0)
        {
            ObjectManager.HandleScrollInput();
        }

        if (Mouse.GetState().Position != screenCenter && IsActive)
        {
            Point p = screenCenter - Mouse.GetState().Position;
            Vector2 mouseVec = p.ToVector2();
            ObjectManager.HandleMouseMove(delta, mouseVec);

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
        spriteBatch.DrawString(Globals.font, $"Coords: {{X: {player.Transform.GlobalPosition.X.ToString("F2")}, Y: {player.Transform.GlobalPosition.Y.ToString("F2")}, Z: {player.Transform.GlobalPosition.Z.ToString("F2")}}}", new Vector2(20, 40), Color.White);
        spriteBatch.DrawString(Globals.font, $"Chunk coords: {Utils.GetPlayerChunkPos()} at chunk: {Utils.WorldToChunkPosition(player.Transform.GlobalPosition)}", new Vector2(20, 60), Color.White);
        spriteBatch.DrawString(Globals.font, $"Looking at block: {player.lookingAtBlock}", new Vector2(20, 80), Color.White);
    }

    public void StartGame()
    {
        player = new Player();
        player.Transform.GlobalPosition = new Vector3(-120, 35, 82);
        ObjectManager.Add(player);
    }
}