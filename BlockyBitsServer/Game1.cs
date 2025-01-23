using BlockyBitsCommon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace BlockyBitsServer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        TcpListener server;
        byte[] bytes = new byte[256];
        string data = null;
        List<TcpClient> clients = new();
        List<TcpClient> deadClients = new();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            server = new(IPAddress.Parse("127.0.0.1"), Network.serverPort);
            server.Start();
            Task task = Task.Run(() => Test());
            //task.Start();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
            if (server.Pending())
            {
                clients.Add(server.AcceptTcpClient());
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Blue);



            base.Draw(gameTime);
        }

        void Test()
        {
            while (true) 
            {
                foreach (TcpClient client in clients)
                {
                    SendMessage(client, "henlo");
                }
                foreach (TcpClient client in deadClients)
                {
                    clients.Remove(client);
                    ServerDebug.WriteLine("client disconnected! " + client.ToString());
                }
                deadClients.Clear();
                Task.Delay(1000).Wait();
            }
        }

        void SendMessage(TcpClient client, String message)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                ServerDebug.WriteLine("sent message!");
                stream.Write(data, 0, data.Length);
            } catch (IOException e)
            {
                deadClients.Add(client);
            }
        }
    }
}
