using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Objects
{
    abstract class Destructible : Entity
    {
        Func<Entity, bool> _canDestroy;

        public Destructible(string spriteSheetPath, Func<Entity, bool> canDestroy)
            : base(spriteSheetPath, null)
        {
            _canDestroy = canDestroy;
            Health = new Container(int.MaxValue);
            Health.ValueChanged += Health_ValueChanged;
        }

        void Health_ValueChanged(object sender, EventArgs e)
        {
            Health.Quantity = int.MaxValue;
        }

        public override void Hit(Entity attacker, GameTime gameTime, Levels.Level level, Vector2 direction)
        {
            if (_canDestroy(attacker))
            {
                Health.ValueChanged -= Health_ValueChanged;
                Health.Quantity = 0;
                base.Hit(attacker, gameTime, level, direction);
            }
        }
    }
}
