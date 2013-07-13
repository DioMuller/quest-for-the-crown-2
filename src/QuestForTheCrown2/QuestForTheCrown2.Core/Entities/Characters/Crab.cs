using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Characters
{
    public class Crab : Entity
    {
        #region Constants
        /// <summary>
        /// Spritesheet path
        /// </summary>
        const string spriteSheetPath = @"sprites\Characters\crab.png";
        #endregion Constants

        #region Constructor
        /// <summary>
        /// Builds main character with its base spritesheet and animations.
        /// </summary>
        public Crab()
            : base(spriteSheetPath, new Point(32, 32))
        {
            Category = "Enemy";

            TimeSpan walkFrameDuration = TimeSpan.FromMilliseconds(100);
            SpriteSheet.AddAnimation("stopped", "down", line: 0, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "left", line: 1, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "right", line: 2, count: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("stopped", "up", line: 3, count: 1, frameDuration: walkFrameDuration);


            SpriteSheet.AddAnimation("walking", "down", line: 0, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "left", line: 1, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "right", line: 2, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking", "up", line: 3, frameDuration: walkFrameDuration);

            Padding = new Rectangle(6, 11, 6, 6);

            Speed = new Vector2(100);

            Health = new Container(2);
            Look(new Vector2(0, 1), true);

            AddBehavior(
                new BlinkBehavior(TimeSpan.FromSeconds(0.5)),
                new HitOnTouchBehavior(),
                new FollowBehavior("Player", 5) { MaxDistance = 32 * 3 }
            );
        }
        #endregion Constructor

        public override void Update(GameTime gameTime, Levels.Level level)
        {
            var col = CollisionRect;
            var ent = level.CollidesWith(new Rectangle(
                x: (int)(col.X + CurrentDirection.X * 8),
                y: (int)(col.Y + CurrentDirection.Y * 8),
                width: col.Width,
                height: col.Height)).FirstOrDefault(e => e != this);

            if (ent != null)
                ent.Hit(this, level, CurrentDirection);

            base.Update(gameTime, level);
        }
    }
}
