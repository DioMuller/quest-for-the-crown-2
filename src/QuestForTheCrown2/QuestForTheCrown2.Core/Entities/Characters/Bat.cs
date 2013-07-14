using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Characters
{
    public class Bat : Entity
    {
        #region Constants
        /// <summary>
        /// Spritesheet path
        /// </summary>
        const string spriteSheetPath = @"sprites\Characters\bat.png";
        #endregion Constants

        #region Constructor
        /// <summary>
        /// Builds main character with its base spritesheet and animations.
        /// </summary>
        public Bat()
            : base(spriteSheetPath, new Point(18, 16))
        {
            Category = "Enemy";

            TimeSpan walkFrameDuration = TimeSpan.FromMilliseconds(50);
            SpriteSheet.AddAnimation("default", "default", line: 0, frameDuration: walkFrameDuration);

            Padding = new Rectangle(1, 2, 1, 6);

            Speed = new Vector2(32 * 4);

            Health = new Container(1);
            CurrentDirection = new Vector2(0, 1);

            AddBehavior(
                new HitOnTouchBehavior(e => e.Category == "Player"),
                new TouchAndDodgeBehavior("Player", 32 * 2)
            );
        }
        #endregion Constructor
    }
}
