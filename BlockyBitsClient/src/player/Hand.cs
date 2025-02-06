using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        public override void Start()
        {
            Transform.Position = new Vector3(0.3f, 0f, 0f);
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
