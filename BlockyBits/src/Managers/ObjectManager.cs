using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BlockyBitsClient.src.Managers
{
    public static class ObjectManager
    {
        static List<Object> gameObjects = new();
        static List<Object> objectsToDelete = new();
        //public static event Action onObjectAdded;
        //public static event Action onObjectRemoved;

        public static void Add(Object ob)
        {
            gameObjects.Add(ob);
            ob.LoadContent(Game1.game.Content);
            ob.Start();
            ob.ChildrenStart(Game1.game.Content);
            ob.ComponentsStart(Game1.game.Content);
        }

        public static void DeleteObject(Object ob)
        {
            objectsToDelete.Add(ob);
        }

        public static void RenderObjects()
        {
            foreach (Object obj in gameObjects)
            {
                //BoundingFrustum frustum = new(camera.viewMatrix * camera.projectionMatrix);
                Game1.game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                obj.Render();
                obj.RenderChildren();
            }
        }

        public static void UpdateObjects(float delta)
        {
            foreach(Object obj in objectsToDelete)
            {
                obj.OnDelete();
                obj.OnDeleteChildrenAndComponents();
                gameObjects.Remove(obj);
            }
            objectsToDelete.Clear();

            foreach (Object obj in gameObjects)
            {
                //engine updates
                obj.UpdateChildrenAndComponents(delta);
                //user scripts updates
                obj.Update(delta);
            }
        }
        public static void HandleInput(float delta)
        {
            foreach (Object obj in gameObjects)
            {
                obj.HandleInput(delta);
                obj.HandleComponentInput(delta);
                obj.HandleChildrenInput(delta);
            }
        }

        public static void HandleMouseInput(float delta, Vector2 mouseVec)
        {
            foreach (Object obj in gameObjects)
            {
                obj.HandleMouseInput(delta, mouseVec);
                obj.HandleComponentMouseInput(delta, mouseVec);
                obj.HandleChildrenMouseInput(delta, mouseVec);
            }
        }
    }
}
