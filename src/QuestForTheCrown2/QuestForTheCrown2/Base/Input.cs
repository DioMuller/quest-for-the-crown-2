using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace QuestForTheCrown2.Base
{
    public enum InputType
    {
        Controller,
        Keyboard,
        KeyboardAndMouse
    }

    public class Input
    {
        #region Properties
        public InputType Type { get; private set; }
        public int Index { get; private set; }

        public Vector2 Movement
        {
            get
            {
                switch(Type)
                {
                    case InputType.Controller:
                        return GamePad.GetState((PlayerIndex) Index).ThumbSticks.Left;
                    default:
                        return Vector2.Zero;
                }
            }
        }

        public Vector2 Attack
        {
            get
            {
                switch (Type)
                {
                    case InputType.Controller:
                        return GamePad.GetState((PlayerIndex)Index).ThumbSticks.Right;
                    default:
                        return Vector2.Zero;
                }
            }
        }

        public bool QuitButton
        {
            get
            {
                switch (Type)
                {
                    case InputType.Controller:
                        return GamePad.GetState((PlayerIndex)Index).IsButtonDown(Buttons.Start);
                    case InputType.Keyboard:
                    case InputType.KeyboardAndMouse:
                        return Keyboard.GetState((PlayerIndex)Index).IsKeyDown(Keys.Escape);
                    default:
                        return false;
                }
            }
        }
        #endregion Properties

        #region Constructor
        public Input(InputType inputType) : this(inputType, 0)
        {
            //Nothing else to do.
        }

        public Input(InputType inputType, int index)
        {
            Type = inputType;
            Index = index;
        }
        #endregion Costructor
    }
}
