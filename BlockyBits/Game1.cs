using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BlockyBits
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Point screenCenter;
        Camera camera;
        GameObject axe;
        List<Object> gameObjects = new List<Object>(10);


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            camera = new Camera(GraphicsDevice);
            axe = new GameObject();
            gameObjects.Add(camera);
            gameObjects.Add(axe);
            screenCenter = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            Mouse.SetPosition(screenCenter.X, screenCenter.Y);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            axe.model = Content.Load<Model>("axe");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //axe.worldMatrix.Translation = new Vector3(0,0,-10*((float)gameTime.ElapsedGameTime.TotalSeconds));

            float delta = ((float)gameTime.ElapsedGameTime.TotalSeconds);
            if (Keyboard.GetState().GetPressedKeyCount() > 0)
            {
                foreach (Object obj in gameObjects)
                {
                    obj.HandleInput(delta);
                }
            }

            if(Mouse.GetState().Position != screenCenter && IsActive)
            {
                Point p = screenCenter - Mouse.GetState().Position;
                Vector2 mouseVec = p.ToVector2();
                foreach(Object obj in gameObjects)
                {
                    obj.HandleMouseInput(delta, mouseVec);
                }


                Mouse.SetPosition(screenCenter.X, screenCenter.Y);
            }

            foreach (Object obj in gameObjects)
            {
                obj.Update(delta);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (Object obj in gameObjects)
            {
                obj.Render(camera);
            }

            base.Draw(gameTime);
        }
    }
}
