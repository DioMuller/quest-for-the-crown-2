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
    public class Player2 : Entity
    {
        #region Constants
        /// <summary>
        /// Spritesheet path
        /// </summary>
        const string spriteSheetPath = @"sprites\Characters\mage.png";
        #endregion Constants

        #region Attributes
        #endregion

        #region Constructor
        /// <summary>
        /// Builds main character with its base spritesheet and animations.
        /// </summary>
        public Player2()
            : base(spriteSheetPath, 9, 9)
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

            SpriteSheet.AddAnimation("dying", "default", line: 8, count: 6, frameDuration: walkFrameDuration, repeat: false);

            Padding = new Rectangle(22, 32, 22, 2);

            Speed = 32 * 5;

            Health = new Container(8);
            Look(new Vector2(0, 1), true);

            GetBehavior<BlinkBehavior>().BlinkDuration = TimeSpan.FromSeconds(1);

            var controllerBehavior = new InputBehavior(InputType.Controller);
            var keyboardBehavior = new InputBehavior(InputType.Keyboard);
            keyboardBehavior.OnEnter += controllerBehavior_OnEnter;

            Arrows = new Container(50);
            Magic = new Container(10);

            DisplayName = "Player 2";
        }

        void controllerBehavior_OnEnter(object sender, GameEventArgs e)
        {
            var beh = (InputBehavior)sender;
            //TODO: criar entidade e adicionar este behavior na próxima
            var p2 = new Player { Position = Position + new Vector2(Size.X), CurrentLevel = CurrentLevel };

            if (!e.Level.Map.Collides(p2.CollisionRect) && !e.Level.CollidesWith(p2.CollisionRect, true).Any())
            {
                RemoveBehavior(beh);
                p2.RemoveBehaviors(b => b is InputBehavior);
                p2.AddBehavior(beh);
                e.Level.AddEntity(p2);
                beh.OnEnter -= controllerBehavior_OnEnter;
                var oldHealth = Health.Quantity;
                Health.Quantity /= 2;
                p2.Health.Quantity = oldHealth - Health.Quantity;
                LevelCollection.CloneWaypoints(this, p2);
            }
        }

        public override void Update(GameTime gameTime, Level level)
        {
            if (GetBehavior<InputBehavior>(b => b.InputType == InputType.Controller) != null)
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
