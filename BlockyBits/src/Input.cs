using Microsoft.Xna.Framework.Input;


namespace BlockyBits.src
{
    public class Input
    {
        static KeyboardState previousKeyState;
        static KeyboardState currentKeyState;
        private static int prevScrollValue = 0;
        private static int scrollDirection = 0;

        public static void UpdateState()
        {
            previousKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
            MouseState state = Mouse.GetState();
            if(state.ScrollWheelValue < prevScrollValue)
            {
                scrollDirection = -1;
                prevScrollValue = state.ScrollWheelValue;
            }
            else if(state.ScrollWheelValue > prevScrollValue)
            {
                scrollDirection = 1;
                prevScrollValue = state.ScrollWheelValue;
            } else
            {
                scrollDirection = 0;
            }


        }

        public static bool IsKeyJustPressed(Keys key)
        {
            return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
        }

        public static int GetScrollDirection()
        {
            return scrollDirection;
        }

        public static bool AnyKeyPressed()
        {
            return currentKeyState.GetPressedKeyCount() > 0;
        }
    }
}
