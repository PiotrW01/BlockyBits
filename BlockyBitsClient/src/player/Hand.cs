﻿using BlockyBits.src;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockyBitsClient.src.player
{
    internal class Hand : GameObject
    {
        Vector3 start = new Vector3(0.3f, 0.2f, -2f);
        public override void Start()
        {
            Transform.Position = start;
        }


        public override void Update(float deltaTime)
        {
            //Transform.Position = forward;
            //Transform.Position += Transform.Forward * deltaTime / 3f;
            //Debug.WriteLine(Transform.Forward);
        }

        public override void HandleInput(float deltaTime)
        {
            if (Input.IsKeyJustPressed(Keys.Y))
            {
                Transform.Position = new Vector3(0, 0, -1);
            }
        }

        public override void LoadContent(ContentManager cm)
        {
            model = Models.GetModelShape("right_hand");
        }

        public override void Render()
        {
            Matrix rotation = Matrix.CreateFromQuaternion(Transform.GlobalQuaternion);
            Matrix trans = Matrix.CreateTranslation(Transform.GlobalPosition);
            Matrix pivot = Matrix.CreateTranslation(-Transform.Position);
            Matrix pivotBack = Matrix.CreateTranslation(Transform.Position);

            Matrix worldMatrix = rotation * trans;
            model.Render(worldMatrix);
        }
    }
}
