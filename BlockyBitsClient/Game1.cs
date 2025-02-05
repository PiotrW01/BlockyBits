using BlockyBits.src;
using BlockyBitsClient.src;
using BlockyBitsClient.src.gui;
using BlockyBitsClient.src.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using Windows.UI.WebUI;

public class Game1 : Game
{
    RenderTarget2D renderTarget;
    RenderTarget2D renderTarget2;
    private GraphicsDeviceManager _graphics;
    SpriteBatch _spriteBatch;
    
    public static Camera MainCamera;
    public static Player Player;
    World world;
    public static Game1 game;
    
    public Point screenCenter;
    
    public bool mouseLocked = true;
    public double elapsedTime = 0f;


    double fixedUpdateTick = 1.0 / 20.0;
    double timeSinceLastUpdate = 0;
    double timeForFixedUpdate = 0;
    int frameCounter = 0;
    int fps = 0;

    public TcpClient client;


    public Game1()
    {
        if(game == null)
        {
            game = this;
        }
        _graphics = new GraphicsDeviceManager(this)
        {
            GraphicsProfile = GraphicsProfile.HiDef
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        if (Settings.VSyncEnabled)
        {
            IsFixedTimeStep = false;
            _graphics.SynchronizeWithVerticalRetrace = true;
        }
        else if (Settings.LimitFPS)
        {
            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1f / Settings.FpsLimit);
        } 
        else
        {
            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
        }
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.ApplyChanges();

        renderTarget = new(GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
        renderTarget2 = new(GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
        Shaders.WaterShader.Parameters["tex"].SetValue(renderTarget);

        MainCamera = new Camera(GraphicsDevice);
        Debugger.Enable(GraphicsDevice);

        screenCenter = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        Mouse.SetPosition(screenCenter.X, screenCenter.Y);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        TextureAtlas.LoadAtlas(Content);
        Models.LoadModels();
        Block.LoadBlocks();
        Globals.LoadContent(Content);
        world = new();
        world.LoadWorld();


        GUIManager.RegisterUIElement(new MainMenu());
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            ChunkManager.Stop();
            Exit();
        }
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


        timeForFixedUpdate += gameTime.ElapsedGameTime.TotalSeconds;
        if(timeForFixedUpdate >= fixedUpdateTick)
        {
            world.DoTick();
            timeForFixedUpdate = 0;
        }





        HandleInput(delta);
        ObjectManager.UpdateObjects(delta);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Shaders.UpdateShaderParameters();
        GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        GraphicsDevice.BlendState = BlendState.Opaque;
        GraphicsDevice.SamplerStates[0] = new SamplerState()
        {
            Filter = TextureFilter.Point,
            AddressU = TextureAddressMode.Clamp,
            AddressV = TextureAddressMode.Clamp,
        };
        ChunkManager.RenderChunks();


        GraphicsDevice.SetRenderTarget(renderTarget2);
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        GraphicsDevice.BlendState = BlendState.Opaque;
        GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0.0f, 0);
        GraphicsDevice.SamplerStates[0] = new SamplerState()
        {
            Filter = TextureFilter.Point
        };
        ChunkManager.RenderChunks();
        GraphicsDevice.BlendState = BlendState.AlphaBlend;
        ChunkManager.RenderWater();
        ObjectManager.RenderObjects();
        
        GraphicsDevice.SetRenderTarget(null);
        _spriteBatch.Begin();
        _spriteBatch.Draw(renderTarget2, Vector2.Zero, Color.White);
        RenderDebugInfo();
        _spriteBatch.End();



        Debugger.DrawDebugLines();
        GUIManager.RenderGUI(_spriteBatch);
        base.Draw(gameTime);
    }

    private void HandleInput(float delta)
    {
        if (!IsActive) return;
        Input.UpdateState();
        GUIManager.CheckMouseEvents();


        if (Input.IsKeyJustPressed(Keys.F11))
        {
            renderTarget.Dispose();
            renderTarget2.Dispose();
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
            
            screenCenter = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            Mouse.SetPosition(screenCenter.X, screenCenter.Y);
            renderTarget = new(GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            renderTarget2 = new(GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            Shaders.WaterShader.Parameters["tex"].SetValue(renderTarget);
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
            //noisegeninput();
        }

        if (Input.IsMouseClickChanged())
        {
            ObjectManager.HandleMouseClick();
        }

        if(Input.GetScrollDirection() != 0)
        {
            ObjectManager.HandleScrollInput();
        }

        if (Mouse.GetState().Position != screenCenter)
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
        if (!Debugger.showDebugInfo || Player == null) return;
        _spriteBatch.DrawString(Globals.font, $"Fps: {fps}", new Vector2(20, 20), Color.White);
        _spriteBatch.DrawString(Globals.font, $"Coords: {{X: {Player.Transform.GlobalPosition.X.ToString("F2")}, Y: {Player.Transform.GlobalPosition.Y.ToString("F2")}, Z: {Player.Transform.GlobalPosition.Z.ToString("F2")}}}", new Vector2(20, 40), Color.White);
        _spriteBatch.DrawString(Globals.font, $"Chunk coords: {Utils.GetPlayerChunkPos()} at chunk: {Utils.WorldToChunkPosition(Player.Transform.GlobalPosition)}", new Vector2(20, 60), Color.White);
        _spriteBatch.DrawString(Globals.font, $"Looking at block: {Player.lookingAtBlock}", new Vector2(20, 80), Color.White);
    }


/*    void noisegeninput()
    {
        if (Input.IsKeyJustPressed(Keys.OemPlus))
        {
            noiser.frequency += 0.0005f;
        }
        if (Input.IsKeyJustPressed(Keys.OemMinus))
        {
            noiser.frequency -= 0.0005f;
        }
        if (Input.IsKeyJustPressed(Keys.I))
        {
            noiser.lacunarity += 0.05f;
        }
        if (Input.IsKeyJustPressed(Keys.O))
        {
            noiser.lacunarity -= 0.05f;
        }
        if (Input.IsKeyJustPressed(Keys.J))
        {
            noiser.amplitude += 0.05f;
        }
        if (Input.IsKeyJustPressed(Keys.K))
        {
            noiser.amplitude -= 0.05f;
        }
        if (Input.IsKeyJustPressed(Keys.N))
        {
            noiser.octaves++;
        }
        if (Input.IsKeyJustPressed(Keys.M))
        {
            noiser.octaves--;
        }
        if (Input.IsKeyJustPressed(Keys.S))
        {
            noiser.gain += 0.1f;
        }
        if (Input.IsKeyJustPressed(Keys.D))
        {
            noiser.gain -= 0.1f;
        }
        noiser.GenerateMap();
        Debug.WriteLine($"frequency: {noiser.frequency}, amplitude: {noiser.amplitude}, octaves: {noiser.octaves}, lacunarity: {noiser.lacunarity}, gain: {noiser.gain}");
    } */
}