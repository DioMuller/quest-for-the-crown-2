using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Behaviors
{
    class BlinkBehavior : EntityUpdateBehavior
    {
        int? oldHealth;
        TimeSpan _lastBlinkChange;
        TimeSpan? _startBlink;

        TimeSpan _maxBlinkTime = TimeSpan.FromSeconds(1);
        TimeSpan _blinkChangeTime = TimeSpan.FromMilliseconds(50);

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

            if (gameTime.TotalGameTime > _startBlink + _maxBlinkTime)
            {
                Entity.IsInvisible = false;
                Entity.IsBlinking = false;
                _startBlink = null;
            }
        }

        public override bool IsActive(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            return Entity.Health != null;
        }
    }
}
