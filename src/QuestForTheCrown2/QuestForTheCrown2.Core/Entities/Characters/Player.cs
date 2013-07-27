﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestForTheCrown2.Levels.Mapping;
using QuestForTheCrown2.Levels;
using QuestForTheCrown2.Entities.Behaviors;
using QuestForTheCrown2.Entities.Weapons;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.Entities.Characters
{
    public class Player : Entity
    {
        #region Constants
        /// <summary>
        /// Spritesheet path
        /// </summary>
        const string spriteSheetPath = @"sprites\Characters\main.png";
        #endregion Constants

        #region Attributes
        InputBehavior _keyboardBehavior;
        InputBehavior _controller1Behavior, _controller2Behavior;
        #endregion

        #region Constructor
        /// <summary>
        /// Builds main character with its base spritesheet and animations.
        /// </summary>
        public Player()
            : base(spriteSheetPath, 9, 13)
        {
            Category = "Player";

            TimeSpan walkFrameDuration = TimeSpan.FromMilliseconds(100);
            SpriteSheet.AddAnimation("stopped", "up", line: 0, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "left", line: 1, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "down", line: 2, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "right", line: 3, count: 1, frameDuration: walkFrameDuration);


            SpriteSheet.AddAnimation("walking", "up", line: 0, skipFrames: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "left", line: 1, skipFrames: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "down", line: 2, skipFrames: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "right", line: 3, skipFrames: 1, frameDuration: walkFrameDuration);

            SpriteSheet.AddAnimation("dying", "default", line: 12, count: 6, frameDuration: walkFrameDuration, repeat: false);

            Padding = new Rectangle(22, 32, 22, 2);

            Speed = 32 * 5;

            Health = new Container(8);
            Look(new Vector2(0, 1), true);

            GetBehavior<BlinkBehavior>().BlinkDuration = TimeSpan.FromSeconds(1);

            _controller1Behavior = new InputBehavior(InputType.Controller, 0);
            _controller2Behavior = new InputBehavior(InputType.Controller, 1);
            _controller2Behavior.OnEnter += controllerBehavior_OnEnter;

            _keyboardBehavior = new InputBehavior(InputType.Keyboard);
            _keyboardBehavior.OnEnter += controllerBehavior_OnEnter;

            AddBehavior(
                _controller1Behavior,
                _keyboardBehavior
            );
            Arrows = new Container(50);
            Magic = new Container(10);

            DisplayName = "Player 1";
        }

        void controllerBehavior_OnEnter(object sender, GameEventArgs e)
        {
            var beh = (InputBehavior)sender;
            //TODO: criar entidade e adicionar este behavior na próxima
            var p2 = new Player2 { Position = Position + new Vector2(Size.X), CurrentLevel = CurrentLevel };

            if (!e.Level.Map.Collides(p2.CollisionRect) && !e.Level.CollidesWith(p2.CollisionRect, true).Any())
            {
                RemoveBehavior(beh);
                p2.RemoveBehaviors(b => b is InputBehavior);
                p2.AddBehavior(beh);
                e.Level.AddEntity(p2);
                beh.OnEnter -= controllerBehavior_OnEnter;
                var oldHealth = Health.Quantity;

                LevelCollection.CloneWaypoints(this, p2);

                if (Weapons != null)
                {
                    foreach (var weapon in Weapons)
                        p2.AddWeapon(GameStateManager.WeaponFactory[weapon.GetType().Name]());
                }

                foreach (var ctn in Containers)
                    p2.Containers[ctn.Key].Quantity = ctn.Key == "Magic" ? ctn.Value.Quantity : ctn.Value.Quantity / 2;

                Health.Quantity /= 2;
                p2.Health.Quantity = oldHealth - Health.Quantity;
                if (Health.Quantity <= 0)
                    Health.Quantity = 1;
                if (p2.Health.Quantity <= 0)
                    p2.Health.Quantity = 1;

                _controller2Behavior = null;
                _keyboardBehavior = null;
            }
        }

        public override void Update(GameTime gameTime, Level level)
        {
            if (_controller2Behavior != null)
                _controller2Behavior.CheckEnter(gameTime, level);

            if (_keyboardBehavior != null && _controller1Behavior.IsConnected)
            {
                var beh = GetBehavior<InputBehavior>(b => b.InputType == InputType.Keyboard);
                if (beh != null)
                    beh.CheckEnter(gameTime, level);
            }
            base.Update(gameTime, level);
        }
        #endregion Constructor
    }
}
