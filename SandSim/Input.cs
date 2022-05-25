using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandSim
{
    internal class Input
    {
        public static List<string> Actions = new List<string>();
        static KeyboardState kstate = new KeyboardState();
        static KeyboardState kstateOld;
        static MouseState mstate = new MouseState();
        static MouseState mstateOld;
        public static void Update()
        {
            kstateOld = kstate;
            kstate = Keyboard.GetState();
            mstateOld = mstate;
            mstate = Mouse.GetState();

            Actions.Clear();

            if (mstate.LeftButton == ButtonState.Pressed)
            {
                Actions.Add("leftClick");
            }
            if (mstate.RightButton == ButtonState.Pressed)
            {
                Actions.Add("rightClick");
            }
            if (IsNewKey(Keys.D1))
            {
                Actions.Add("BlockMode");
            }
            if (IsNewKey(Keys.D2))
            {
                Actions.Add("SandMode");
            }

        }
        static bool IsNewKey(Keys key)
        {
            return kstate.IsKeyDown(key) && kstateOld.IsKeyDown(key);
        }
        public static (int x, int y) GetMousePosition()
        {
            return (mstate.X, mstate.Y);
        }
    }
}
