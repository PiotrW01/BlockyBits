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
        ModelShape shape;
        public double dropTime;

        VertexBuffer vbuffer;
        IndexBuffer ibuffer;

        BasicEffect effect = new(Game1.game.GraphicsDevice)
        {
            TextureEnabled = true,
            Texture = TextureAtlas.atlas
        };

        public DroppedItem(Item item, string name)
        {
            //AddComponent<components.Rotation>();
            Transform.Scale = Vector3.One * 0.2f;
            this.name = name;
            shape = Models.GetModelShape("oak_log");
            vbuffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionNormalTexture), shape.vertices.Count, BufferUsage.WriteOnly);
            ibuffer = new IndexBuffer(Game1.game.GraphicsDevice, IndexElementSize.ThirtyTwoBits, shape.indices.Count, BufferUsage.WriteOnly);
            vbuffer.SetData(shape.vertices.ToArray());
            ibuffer.SetData(shape.indices.ToArray());
            dropTime = Game1.game.elapsedTime;
        }

        public bool TryGetItem(out Item item)
        {
            if(Game1.game.elapsedTime - dropTime > 3f)
            {
                item = this.item;
                ObjectManager.RemoveDroppedItem(this);
                vbuffer.Dispose();
                ibuffer.Dispose();
                vbuffer = null;
                ibuffer = null;
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
            if (vbuffer == null || ibuffer == null) return; 
            Matrix worldMatrix = Matrix.CreateTranslation(-Vector3.One * 0.5f) * 
                                 Matrix.CreateFromQuaternion(Transform.Quaternion) *
                                 Matrix.CreateScale(Transform.Scale) * 
                                 Matrix.CreateTranslation(Transform.GlobalPosition);
            effect.World = worldMatrix;
            effect.View = Game1.MainCamera.viewMatrix;
            effect.Projection = Game1.MainCamera.projectionMatrix;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game1.game.GraphicsDevice.SetVertexBuffer(vbuffer);
                Game1.game.GraphicsDevice.Indices = ibuffer;
                Game1.game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, shape.indices.Count / 3);
            }
        }
    }
}
