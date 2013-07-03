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
                    case InputType.Keyboard:
                    case InputType.KeyboardAndMouse:
                        var state = Keyboard.GetState((PlayerIndex)Index);
                        var movement = new Vector2(
                            x: (state.IsKeyDown(Keys.Left)? -1 : 0) + (state.IsKeyDown(Keys.Right)? +1 : 0),
                            y: (state.IsKeyDown(Keys.Up) ? -1 : 0) + (state.IsKeyDown(Keys.Down) ? +1 : 0));
                        if (movement.X != 0 && movement.Y != 0)
                            movement.Normalize();
                        return movement;
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
        public Input()
        {
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                this.Type = InputType.Controller;
            }
            else
            {
                this.Type = InputType.KeyboardAndMouse;
            }

            this.Index = 0;
        }

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
