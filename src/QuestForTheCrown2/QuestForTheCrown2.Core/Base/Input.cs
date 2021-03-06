﻿using System;
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
        #region Attributes
        public bool _nextWeaponState, _previousWeaponState;
        public bool _confirmButtonState, _cancelButtonState, _attackButtonState;
        public bool _pauseButtonState;
        #endregion

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
                        var gpState = GamePad.GetState((PlayerIndex)Index);
                        var leftStick = gpState.ThumbSticks.Left;
                        var raw = new Vector2(
                            x: leftStick.X,
                            y: -leftStick.Y);
                        if (raw.Length() > 0.4)
                            return raw;
                        var dPadMovement = new Vector2(
                            x: (gpState.IsButtonDown(Buttons.DPadLeft) ? -1 : 0) + (gpState.IsButtonDown(Buttons.DPadRight) ? +1 : 0),
                            y: (gpState.IsButtonDown(Buttons.DPadUp) ? -1 : 0) + (gpState.IsButtonDown(Buttons.DPadDown) ? +1 : 0));
                        if (dPadMovement.X != 0 && dPadMovement.Y != 0)
                            dPadMovement.Normalize();
                        return dPadMovement;

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
                bool nextState;

                switch (Type)
                {
                    case InputType.Controller:
                        nextState = GamePad.GetState((PlayerIndex)Index).IsButtonDown(Buttons.Start);
                        break;
                    case InputType.Keyboard:
                    case InputType.KeyboardAndMouse:
                        nextState = Keyboard.GetState((PlayerIndex)Index).IsKeyDown(Keys.Enter);
                        break;
                    default:
                        return false;
                }

                if (nextState && !_pauseButtonState)
                {
                    _pauseButtonState = nextState;
                    return true;
                }

                _pauseButtonState = nextState;
                return false;
            }
        }

        /// <summary>
        /// Confirm button pressed?
        /// </summary>
        public bool ConfirmButton
        {
            get
            {
                bool nextState;

                switch (Type)
                {
                    case InputType.Controller:
                        nextState = GamePad.GetState((PlayerIndex)Index).IsButtonDown(Buttons.A);
                        break;
                    case InputType.Keyboard:
                    case InputType.KeyboardAndMouse:
                        nextState = Keyboard.GetState((PlayerIndex)Index).IsKeyDown(Keys.Enter);
                        break;
                    default:
                        return false;
                }

                if (nextState && !_confirmButtonState)
                {
                    _confirmButtonState = nextState;
                    return true;
                }

                _confirmButtonState = nextState;
                return false;
            }
        }

        /// <summary>
        /// Cancel button pressed?
        /// </summary>
        public bool CancelButton
        {
            get
            {
                bool nextState;

                switch (Type)
                {
                    case InputType.Controller:
                        nextState = GamePad.GetState((PlayerIndex)Index).IsButtonDown(Buttons.B);
                        break;
                    case InputType.Keyboard:
                    case InputType.KeyboardAndMouse:
                        nextState = Keyboard.GetState((PlayerIndex)Index).IsKeyDown(Keys.Escape);
                        break;
                    default:
                        return false;
                }

                if (nextState && !_cancelButtonState)
                {
                    _cancelButtonState = nextState;
                    return true;
                }

                _cancelButtonState = nextState;
                return false;
            }
        }

        /// <summary>
        /// Attack button pressed?
        /// </summary>
        public bool AttackButton
        {
            get
            {
                bool nextState;

                switch (Type)
                {
                    case InputType.Controller:
                        var gpState = GamePad.GetState((PlayerIndex)Index);
                        nextState = gpState.IsButtonDown(Buttons.X)
                            || gpState.Triggers.Right > 0.7;
                        break;
                    case InputType.Keyboard:
                    case InputType.KeyboardAndMouse:
                        var kbState = Keyboard.GetState((PlayerIndex)Index);
                        nextState = kbState.IsKeyDown(Keys.LeftControl)
                            || kbState.IsKeyDown(Keys.RightControl)
                            || kbState.IsKeyDown(Keys.Space);
                        break;
                    default:
                        return false;
                }

                if (nextState && !_attackButtonState)
                {
                    _attackButtonState = nextState;
                    return true;
                }

                _attackButtonState = nextState;
                return false;
            }
        }

        /// <summary>
        /// Is Next Weapon Button pressed?
        /// </summary>
        public bool NextWeapon
        {
            get
            {
                bool nextState;
                switch (Type)
                {
                    case InputType.Controller:
                        nextState = GamePad.GetState((PlayerIndex)Index).IsButtonDown(Buttons.RightShoulder) || GamePad.GetState((PlayerIndex)Index).IsButtonDown(Buttons.B);
                        break;
                    case InputType.Keyboard:
                    case InputType.KeyboardAndMouse:
                        nextState = Keyboard.GetState((PlayerIndex)Index).IsKeyDown(Keys.E);
                        break;
                    default:
                        nextState = false;
                        break;
                }

                if (nextState && !_nextWeaponState)
                {
                    _nextWeaponState = nextState;
                    return true;
                }

                _nextWeaponState = nextState;
                return false;
            }
        }

        /// <summary>
        /// Is Previous Weapon Button pressed?
        /// </summary>
        public bool PreviousWeapon
        {
            get
            {
                bool nextState;

                switch (Type)
                {
                    case InputType.Controller:
                        nextState = GamePad.GetState((PlayerIndex)Index).IsButtonDown(Buttons.LeftShoulder) || GamePad.GetState((PlayerIndex)Index).IsButtonDown(Buttons.Y);
                        break;
                    case InputType.Keyboard:
                    case InputType.KeyboardAndMouse:
                        nextState = Keyboard.GetState((PlayerIndex)Index).IsKeyDown(Keys.Q);
                        break;
                    default:
                        nextState = false;
                        break;
                }

                if (nextState && !_previousWeaponState)
                {
                    _previousWeaponState = nextState;
                    return true;
                }

                _previousWeaponState = nextState;
                return false;
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

            _confirmButtonState = true;
            _cancelButtonState = true;
            _attackButtonState = true;
            _pauseButtonState = true;
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

            _confirmButtonState = true;
            _cancelButtonState = true;
            _attackButtonState = true;
            _pauseButtonState = true;
        }
        #endregion Costructor
    }
}
