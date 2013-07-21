using Microsoft.Xna.Framework;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Behaviors;
using QuestForTheCrown2.Entities.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Characters
{
    class SlimeWorm : Entity
    {
        #region Constants
        /// <summary>
        /// Spritesheet path
        /// </summary>
        const string spriteSheetPath = @"sprites\Characters\slimeworm.png";
        #endregion Constants

        #region Properties
        Container _frontHealth, _normalHealth;

        public override string CurrentAnimation
        {
            get { return base.CurrentAnimation; }
            set
            {
                var beh = GetBehavior<ChainBehavior<SlimeWorm>>();
                if (beh != null && beh.FollowedBy != null && beh.Following == null)
                {
                    base.CurrentAnimation = value + "-front";
                    Health = _frontHealth;
                }
                else
                {
                    base.CurrentAnimation = value;
                    Health = _normalHealth;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Builds main character with its base spritesheet and animations.
        /// </summary>
        public SlimeWorm()
            : base(spriteSheetPath, new Point(40, 34))
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

            SpriteSheet.AddAnimation("stopped-front", "left", line: 7, frameDuration: stoppedFrameDuration);
            SpriteSheet.AddAnimation("stopped-front", "down", line: 6, frameDuration: stoppedFrameDuration);
            SpriteSheet.AddAnimation("stopped-front", "right", line: 5, frameDuration: stoppedFrameDuration);
            SpriteSheet.AddAnimation("stopped-front", "up", line: 4, frameDuration: stoppedFrameDuration);

            SpriteSheet.AddAnimation("walking-front", "left", line: 7, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking-front", "down", line: 6, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking-front", "right", line: 5, frameDuration: walkFrameDuration);
            SpriteSheet.AddAnimation("walking-front", "up", line: 4, frameDuration: walkFrameDuration);

            //Padding = new Rectangle(6, 11, 6, 6);

            Speed = 32 * 6;

            _frontHealth = new Container(8);
            _normalHealth = new Container(2);

            Health = _normalHealth;
            Look(new Vector2(0, 1), true);

            var chainBehavior = new ChainBehavior<SlimeWorm> { MaxDistance = 32 * 7 };

            AddBehavior(
                new HitOnTouchBehavior(e => e.Category == "Player"),
                chainBehavior,
                new AvoidBehavior(e =>
                    {
                        if (e == this || chainBehavior.Following != e)
                            return false;
                        var beh = e.GetBehavior<ChainBehavior<SlimeWorm>>();
                        return beh != null && beh.Following == null;
                    }),
                new SpiralAttackBehavior("Player", 32 * 8) { MaxDistance = 32 * 12 },
                new WalkAroundBehavior { MaxStoppedTime = TimeSpan.Zero, MaxWalkingTime = TimeSpan.MaxValue, SpeedMultiplier = 1 }
            );

            GetBehavior<DropItemsBehavior>().DropOnDeath((gameTime, level) =>
            {
                if(!level.GetEntities<SlimeWorm>().Any(e => e != this))
                    return new HealthContainer();
                return null;
            });
        }
        #endregion Constructor
    }
}
