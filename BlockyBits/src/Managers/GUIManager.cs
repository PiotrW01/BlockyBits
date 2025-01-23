using BlockyBitsClient.src.gui;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockyBitsClient.src.Managers
{
    public static class GUIManager
    {
        private static List<GUIElement> guiElements = new();
        private static List<GUIElement> elementsToRemove = new();
        public static MouseState lastState = Mouse.GetState();

        public static void CheckMouseEvents()
        {
            MouseState state = Mouse.GetState();
            foreach (GUIElement element in guiElements)
            {
                if (element.rect.Contains(state.Position))
                {
                    element.OnHover();
                    break;
                }
            }

            if (state != lastState)
            {
                if (state.LeftButton != lastState.LeftButton ||
                   state.MiddleButton != lastState.MiddleButton ||
                   state.RightButton != lastState.RightButton)
                {
                    foreach (GUIElement element in guiElements)
                    {

                        if (element.rect.Contains(state.Position))
                        {
                            element.OnClickChanged();
                            break;
                        }
                    }
                }
                lastState = state;
            }

            foreach (GUIElement element in elementsToRemove)
            {
                guiElements.Remove(element);
            }
            elementsToRemove.Clear();
        }

        public static void RenderGUI(SpriteBatch sb)
        {
            foreach (GUIElement element in guiElements)
            {
                element.Render(sb);
            }
        }

        public static void RegisterUIElement(GUIElement el)
        {
            guiElements.Add(el);
            el.Ready();
        }


        public static void RemoveUIElement(GUIElement el)
        {
            elementsToRemove.Add(el);
        }
    }
}
