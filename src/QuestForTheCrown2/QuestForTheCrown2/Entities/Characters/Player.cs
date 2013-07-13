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

        #region Constructor
        /// <summary>
        /// Builds main character with its base spritesheet and animations.
        /// </summary>
        public Player()
            : base(spriteSheetPath, new Point(64, 64))
        {
            Category = "Player";

            TimeSpan walkFrameDuration = TimeSpan.FromMilliseconds(100);
            SpriteSheet.AddAnimation("stopped", "up", line: 0, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "left", line: 1, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "down", line: 2, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "right", line: 3, count: 1, frameDuration: walkFrameDuration);
            

            SpriteSheet.AddAnimation("walking", "up", line: 0, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "left", line: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "down", line: 2, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "right", line: 3, frameDuration: walkFrameDuration);

            Padding = new Rectangle(22, 30, 22, 2);

            Speed = new Vector2(32 * 5);

            Health = new Container(8);
            Look(new Vector2(0, 1), true);

            AddBehavior(
                new BlinkBehavior(TimeSpan.FromSeconds(1)),
                new InputBehavior(InputType.Controller),
                new InputBehavior(InputType.Keyboard)
            );
            AddWeapon(new Bow());
            Arrows = new Container(5);
            Magic = new Container(10);
        }
        #endregion Constructor
    }
}
