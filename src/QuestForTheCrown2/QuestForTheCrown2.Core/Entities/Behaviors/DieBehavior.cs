using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Behaviors
{
    class DieBehavior : EntityUpdateBehavior
    {
        bool _playingAnimation;
        bool _dead = false;
        TimeSpan _deathTime;
        public TimeSpan BlinkTime { get; set; }

        public DieBehavior()
        {
            BlinkTime = TimeSpan.FromSeconds(0.5);
        }

        public override bool IsActive(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            if (_dead || (Entity.Health != null && Entity.Health <= 0))
            {
                if (!_dead)
                {
                    if (!Entity.SpriteSheet.Animations.ContainsKey("dying"))
                        _deathTime = gameTime.TotalGameTime;
                    else
                    {
                        _playingAnimation = true;
                        Entity.CurrentAnimation = "dying";
                        EventHandler animationEnded = null;
                        animationEnded = delegate
                        {
                            Entity.AnimationEnded -= animationEnded;
                            _deathTime = gameTime.TotalGameTime;
                            _playingAnimation = false;
                        };
                        Entity.AnimationEnded += animationEnded;
                    }
                }
                _dead = true;
                return true;
            }
            return false;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            if (_playingAnimation)
                return;

            if (_deathTime + BlinkTime < gameTime.TotalGameTime)
                level.RemoveEntity(Entity);
            else
                Entity.IsInvisible = !Entity.IsInvisible;
        }
    }
}
