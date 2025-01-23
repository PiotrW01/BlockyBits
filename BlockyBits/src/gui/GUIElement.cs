using BlockyBitsClient.src.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace BlockyBitsClient.src.gui
{
    public abstract class GUIElement
    {
        public bool isActive = true;
        public Rectangle rect;
        public Point pos = new Point();
        public Vector2 localPos = new Vector2(0,0);
        public Vector2 rotation = new Vector2(0,0);
        public Vector2 scale = Vector2.One;
        public List<GUIElement> GUIelements = new();

        //public Object parent = null;
        //public List<Object> children = new List<Object>();
        //public List<Component> components = new List<Component>();

        public void Start()
        {
            
        }

        public virtual void Ready()
        {

        }

        public virtual void OnClickChanged()
        {
            
        }

        public virtual void OnHover()
        {
           
        }

        public virtual void OnScroll()
        {

        }

        public virtual void Render(SpriteBatch sb)
        {

        }

        public void SetPosition(int x, int y)
        {
            pos = new Point(x, y);
            rect.Location = pos;
        }

        public void AddGUIelement(GUIElement el)
        {
            GUIelements.Add(el);
            GUIManager.RegisterUIElement(el);
        }
    }
}
