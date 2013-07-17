using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Characters
{
    public class Slime : Entity
    {
        #region Constants
        /// <summary>
        /// Spritesheet path
        /// </summary>
        const string spriteSheetPath = @"sprites\Characters\slime.png";
        #endregion Constants

        #region Constructor
        /// <summary>
        /// Builds main character with its base spritesheet and animations.
        /// </summary>
        public Slime()
            : base(spriteSheetPath, new Point(32, 42))
        {
            Category = "Enemy";

            TimeSpan stoppedFrameDuration = TimeSpan.FromMilliseconds(300);
            SpriteSheet.AddAnimation("stopped", "left", line: 3, frameDuration: stoppedFrameDuration);
            SpriteSheet.AddAnimation("stopped", "down", line: 2, frameDuration: stoppedFrameDuration);
            SpriteSheet.AddAnimation("stopped", "right", line: 1, frameDuration: stoppedFrameDuration);
            SpriteSheet.AddAnimation("stopped", "up", line: 0, frameDuration: stoppedFrameDuration);

            TimeSpan walkFrameDuration = TimeSpan.FromMilliseconds(100);
            SpriteSheet.AddAnimation("walking", "left", line: 3, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "down", line: 2, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "right", line: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "up", line: 0, frameDuration: walkFrameDuration);

            Padding = new Rectangle(6, 11, 6, 6);

            Speed = 32 * 3;

            Health = new Container(1);
            Look(new Vector2(0, 1), true);

            AddBehavior(
                new HitOnTouchBehavior(e => e.Category == "Player"),
                new FollowBehavior("Player", 5) { MaxDistance = 32 * 4 },
                new WalkAroundBehavior()
            );
        }
        #endregion Constructor
    }
}
