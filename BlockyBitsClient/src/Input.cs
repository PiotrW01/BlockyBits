using Microsoft.Xna.Framework.Input;


namespace BlockyBits.src
{
    public class Input
    {
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
    }
}
