using BlockyBitsClient.src.components;
using BlockyBitsClient.src.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;

namespace BlockyBitsClient.src
{
    public class DroppedItem : GameObject
    {
        public Item item;
        public String name;
        public double dropTime;

        public DroppedItem(Item item, string name)
        {
            //AddComponent<components.Rotation>();
            Transform.Scale = Vector3.One * 0.2f;
            this.name = name;
            model = Models.GetModelShape("oak_log");
            dropTime = Game1.game.elapsedTime;
        }

        public bool TryGetItem(out Item item)
        {
            if(Game1.game.elapsedTime - dropTime > 3f)
            {
                item = this.item;
                ObjectManager.RemoveDroppedItem(this);
                return true;
            }
            item = null;
            return false;
        }

        public override void LoadContent(ContentManager cm)
        {
        }

        public override void Render()
        {
            Matrix worldMatrix = Matrix.CreateTranslation(-Vector3.One * 0.5f) * 
                                 Matrix.CreateFromQuaternion(Transform.Quaternion) *
                                 Matrix.CreateScale(Transform.Scale) * 
                                 Matrix.CreateTranslation(Transform.GlobalPosition);
            model.Render(worldMatrix);
        }
    }
}
