using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using System;

namespace QuestForTheCrown2.Entities.Behaviors
{
    /// <summary>
    /// Enables one entity to blink when its health is decreased.
    /// </summary>
    class BlinkBehavior : EntityUpdateBehavior
    {
        int? oldHealth;
        TimeSpan _lastBlinkChange;
        TimeSpan? _startBlink;

        public TimeSpan BlinkDuration { get; set; }
        TimeSpan _blinkChangeTime = TimeSpan.FromMilliseconds(50);


        /// <summary>
        /// Created a new BlinkBehavior.
        /// </summary>
        public BlinkBehavior()
        {
            BlinkDuration = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Created a new BlinkBehavior.
        /// </summary>
        /// <param name="blinkDuration">The maximum ammount of time for an entity to be blinking.</param>
        public BlinkBehavior(TimeSpan blinkDuration)
        {
            BlinkDuration = blinkDuration;
        }

        public override void Update(GameTime gameTime, Levels.Level level)
        {
            if (oldHealth != null && oldHealth > Entity.Health)
                _startBlink = gameTime.TotalGameTime;

            Blink(gameTime);

            oldHealth = Entity.Health;
        }

        void Blink(GameTime gameTime)
        {
            if (_startBlink == null)
                return;

            Entity.IsBlinking = true;

            if (gameTime.TotalGameTime > _lastBlinkChange + _blinkChangeTime)
            {
                _lastBlinkChange = gameTime.TotalGameTime;
                Entity.IsInvisible = !Entity.IsInvisible;
            }

            if (gameTime.TotalGameTime > _startBlink + BlinkDuration)
            {
                Entity.IsInvisible = false;
                Entity.IsBlinking = false;
                _startBlink = null;
            }
        }

        public override bool IsActive(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            return !Entity.IsDead && Entity.Health != null;
        }
    }
}
