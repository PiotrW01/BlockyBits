using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;


namespace BlockyBits.src
{
    public class Input
    {
        static Dictionary<Keys, double> lastKeyPressTime = new();
        private static float secondsInterval = 0.09f;

        static KeyboardState previousKeyState;
        static KeyboardState currentKeyState;

        static MouseState prevMouseState;
        static MouseState currentMouseState;

        private static int scrollDirection = 0;

        public static void UpdateState()
        {
            previousKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
            prevMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            
            double elapsedTime = Game1.game.elapsedTime;
            foreach(var key in previousKeyState.GetPressedKeys())
            {
                if (IsKeyJustReleased(key))
                {
                    if (lastKeyPressTime.ContainsKey(key))
                    {
                        lastKeyPressTime[key] = elapsedTime;
                    } else
                    {
                        lastKeyPressTime.Add(key, elapsedTime);
                    }
                }
            }

            if(currentMouseState.ScrollWheelValue < prevMouseState.ScrollWheelValue)
            {
                scrollDirection = -1;
            }
            else if(currentMouseState.ScrollWheelValue > prevMouseState.ScrollWheelValue)
            {
                scrollDirection = 1;
            } else
            {
                scrollDirection = 0;
            }
        }

        public static bool IsKeyJustPressed(Keys key)
        {
            return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
        }

        public static bool IsKeyJustReleased(Keys key)
        {
            return currentKeyState.IsKeyUp(key) && !previousKeyState.IsKeyUp(key);
        }

        public static bool IsLMBJustPressed()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton != ButtonState.Pressed;
        }

        public static bool IsRMBJustPressed()
        {
            return currentMouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton != ButtonState.Pressed;
        }

        public static bool IsMMBJustPressed()
        {
            return currentMouseState.MiddleButton == ButtonState.Pressed && prevMouseState.MiddleButton != ButtonState.Pressed;
        }

        public static bool IsMouseStateChanged()
        {
            return currentMouseState != prevMouseState;
        }

        public static bool IsMouseClickChanged()
        {
            return currentMouseState.LeftButton != prevMouseState.LeftButton ||
                   currentMouseState.MiddleButton != prevMouseState.MiddleButton ||
                   currentMouseState.RightButton != prevMouseState.RightButton;
        }

        public static int GetScrollDirection()
        {
            return scrollDirection;
        }

        public static bool AnyKeyPressed()
        {
            return currentKeyState.GetPressedKeyCount() > 0;
        }

        public static bool IsMouseMoved()
        {
            return currentMouseState.Position != prevMouseState.Position;
        }

        public static bool IsKeyPressedQuickly(Keys key)
        {
            if (IsKeyJustPressed(key))
            {
                if(lastKeyPressTime.TryGetValue(key, out double lastPressTime))
                {
                    double difference = Game1.game.elapsedTime - lastPressTime;
                    if (difference != 0 && difference <= secondsInterval)
                    {
                        lastKeyPressTime[key] = Game1.game.elapsedTime;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
