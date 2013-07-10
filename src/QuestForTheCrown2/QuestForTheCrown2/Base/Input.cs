using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace QuestForTheCrown2.Base
{
    /// <summary>
    /// Input type.
    /// </summary>
    public enum InputType
    {
        Controller,
        Keyboard,
        KeyboardAndMouse
    }

    /// <summary>
    /// Works with all the input in the game.
    /// </summary>
    public class Input
    {
        #region Properties
        /// <summary>
        /// Input type.
        /// </summary>
        public InputType Type { get; private set; }

        /// <summary>
        /// Input index.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Movement direction.
        /// </summary>
        public Vector2 Movement
        {
            get
            {
                switch (Type)
                {
                    case InputType.Controller:
                        var leftStick = GamePad.GetState((PlayerIndex)Index).ThumbSticks.Left;
                        return new Vector2(
                            x: leftStick.X,
                            y: -leftStick.Y);
                    case InputType.Keyboard:
                    case InputType.KeyboardAndMouse:
                        var state = Keyboard.GetState((PlayerIndex)Index);
                        var movement = new Vector2(
                            x: (state.IsKeyDown(Keys.A) ? -1 : 0) + (state.IsKeyDown(Keys.D) ? +1 : 0),
                            y: (state.IsKeyDown(Keys.W) ? -1 : 0) + (state.IsKeyDown(Keys.S) ? +1 : 0));
                        if (movement.X != 0 && movement.Y != 0)
                            movement.Normalize();
                        return movement;
                    default:
                        return Vector2.Zero;
                }
            }
        }

        /// <summary>
        /// Attack button pressed?
        /// </summary>
        public bool Attack
        {
            get
            {
                switch (Type)
                {
                    case InputType.Controller:
                        var gpState = GamePad.GetState((PlayerIndex)Index);
                        return gpState.IsButtonDown(Buttons.A)
                            || gpState.Triggers.Right > 0.7;
                    case InputType.Keyboard:
                    case InputType.KeyboardAndMouse:
                        var kbState = Keyboard.GetState((PlayerIndex)Index);
                        return kbState.IsKeyDown(Keys.LeftControl)
                            || kbState.IsKeyDown(Keys.RightControl)
                            || kbState.IsKeyDown(Keys.Space);
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Attack direction.
        /// </summary>
        public Vector2 AttackDirection
        {
            get
            {
                switch (Type)
                {
                    case InputType.Controller:
                        var rightStick = GamePad.GetState((PlayerIndex)Index).ThumbSticks.Right;
                        return new Vector2(
                            x: rightStick.X,
                            y: -rightStick.Y);
                    case InputType.KeyboardAndMouse:
                    case InputType.Keyboard:
                        var state = Keyboard.GetState((PlayerIndex)Index);
                        var attack = new Vector2(
                            x: (state.IsKeyDown(Keys.Left) ? -1 : 0) + (state.IsKeyDown(Keys.Right) ? +1 : 0),
                            y: (state.IsKeyDown(Keys.Up) ? -1 : 0) + (state.IsKeyDown(Keys.Down) ? +1 : 0));
                        if (attack.X != 0 && attack.Y != 0)
                            attack.Normalize();
                        return attack;
                    default:
                        return Vector2.Zero;
                }
            }
        }

        /// <summary>
        /// Pause button pressed?
        /// </summary>
        public bool PauseButton
        {
            get
            {
                switch (Type)
                {
                    case InputType.Controller:
                        return GamePad.GetState((PlayerIndex)Index).IsButtonDown(Buttons.Start);
                    case InputType.Keyboard:
                    case InputType.KeyboardAndMouse:
                        return Keyboard.GetState((PlayerIndex)Index).IsKeyDown(Keys.Enter);
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Confirm button pressed?
        /// </summary>
        public bool ConfirmButton
        {
            get
            {
                switch (Type)
                {
                    case InputType.Controller:
                        return GamePad.GetState((PlayerIndex)Index).IsButtonDown(Buttons.A);
                    case InputType.Keyboard:
                    case InputType.KeyboardAndMouse:
                        return Keyboard.GetState((PlayerIndex)Index).IsKeyDown(Keys.Space);
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Cancel button pressed?
        /// </summary>
        public bool CancelButton
        {
            get
            {
                switch (Type)
                {
                    case InputType.Controller:
                        return GamePad.GetState((PlayerIndex)Index).IsButtonDown(Buttons.B);
                    case InputType.Keyboard:
                    case InputType.KeyboardAndMouse:
                        return Keyboard.GetState((PlayerIndex)Index).IsKeyDown(Keys.Escape);
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Is Next Weapon Button pressed?
        /// </summary>
        public bool NextWeapon
        {
            get
            {
                switch (Type)
                {
                    case InputType.Controller:
                        return GamePad.GetState((PlayerIndex)Index).IsButtonDown(Buttons.RightShoulder) || GamePad.GetState((PlayerIndex)Index).IsButtonDown(Buttons.DPadRight);
                    case InputType.Keyboard:
                    case InputType.KeyboardAndMouse:
                        return Keyboard.GetState((PlayerIndex)Index).IsKeyDown(Keys.E);
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Is Previous Weapon Button pressed?
        /// </summary>
        public bool PreviousWeapon
        {
            get
            {
                switch (Type)
                {
                    case InputType.Controller:
                        return GamePad.GetState((PlayerIndex)Index).IsButtonDown(Buttons.LeftShoulder) || GamePad.GetState((PlayerIndex)Index).IsButtonDown(Buttons.DPadLeft);
                    case InputType.Keyboard:
                    case InputType.KeyboardAndMouse:
                        return Keyboard.GetState((PlayerIndex)Index).IsKeyDown(Keys.Q);
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Is input connected?
        /// </summary>
        public bool IsConnected
        {
            get
            {
                switch (Type)
                {
                    case InputType.Controller:
                        return GamePad.GetState((PlayerIndex)Index).IsConnected;
                    case InputType.Keyboard:
                    case InputType.KeyboardAndMouse:
                        return true;
                    default:
                        return false;
                }
            }
        }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Initializes input with controller, if possible. If not, initializes with the keyboard.
        /// </summary>
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

        /// <summary>
        /// Initializes input with desired input type, first index.
        /// </summary>
        /// <param name="inputType">Input type.</param>
        public Input(InputType inputType)
            : this(inputType, 0)
        {
            //Nothing else to do.
        }

        /// <summary>
        /// Initializes input with desired input type and index.
        /// </summary>
        /// <param name="inputType">Input type.</param>
        /// <param name="index">Input index.</param>
        public Input(InputType inputType, int index)
        {
            Type = inputType;
            Index = index;
        }
        #endregion Costructor
    }
}
