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
        static Queue<Object> newObjects = new();
        static Octree droppedItems = new(new Vector3(-100,0,-100), new Vector3(100,255,100));
        //public static event Action onObjectAdded;
        //public static event Action onObjectRemoved;

        public static void Add(Object ob)
        {
            newObjects.Enqueue(ob);
        }

        public static List<Object> FindClosestItems(Vector3 position, float radius)
        {
            return droppedItems.Search(position, radius);
        }

        public static bool RemoveDroppedItem(DroppedItem item)
        {
            objectsToDelete.Add(item);
            return droppedItems.Remove(item);
        }

        public static void CreateDroppedItem(Item item, Vector3 pos)
        {
            var drop = new DroppedItem(item, "dirt");
            drop.Transform.GlobalPosition = pos;
            newObjects.Enqueue(drop);
            droppedItems.Insert(drop, pos);
        }

        public static void AddDroppedItem(DroppedItem drop)
        {
            newObjects.Enqueue(drop);
            droppedItems.Insert(drop, drop.Transform.GlobalPosition);
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

            while(newObjects.TryDequeue(out var ob))
            {
                gameObjects.Add(ob);
                ob.LoadContent(Game1.game.Content);
                ob.Start();
                ob.ChildrenStart(Game1.game.Content);
                ob.ComponentsStart(Game1.game.Content);
            }

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

        public static void HandleMouseMove(float delta, Vector2 mouseVec)
        {
            foreach (Object obj in gameObjects)
            {
                obj.HandleMouseMove(delta, mouseVec);
                obj.HandleComponentMouseMove(delta, mouseVec);
                obj.HandleChildrenMouseMove(delta, mouseVec);
            }
        }
        public static void HandleMouseClick()
        {
            foreach (Object obj in gameObjects)
            {
                obj.HandleMouseClick();
                obj.HandleComponentMouseClick();
                obj.HandleChildrenMouseClick();
            }
        }

        public static void HandleScrollInput()
        {
            foreach (Object obj in gameObjects)
            {
                obj.HandleScrollInput();
                obj.HandleChildrenScrollInput();
            }
        }

    }
}
