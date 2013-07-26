using QuestForTheCrown2.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Behaviors
{
    class AttackTheAttackerBehavior : EntityUpdateBehavior
    {
        TimeSpan _hitAttackerTime = TimeSpan.FromSeconds(5);
        TimeSpan _attackTime;
        Entity _attackedBy;

        public override void Attached()
        {
            Entity.OnHit += Entity_OnHit;
        }

        void Entity_OnHit(object sender, HitEventArgs e)
        {
            if (e.Attacker == Entity)
                return;

            _attackedBy = e.Attacker;
            while (_attackedBy.Parent != null)
                _attackedBy = _attackedBy.Parent;
            _attackTime = e.GameTime.TotalGameTime;

            foreach (var beh in Entity.Behaviors.SelectMany(l => l.Value.OfType<AttackBehavior>()))
                beh.OverrideTarget = _attackedBy;
        }

        public override bool IsActive(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            return _attackedBy != null;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            if (_attackedBy == null || !level.ContainsEntity(_attackedBy) || _attackedBy.IsDead || _attackTime + _hitAttackerTime < gameTime.TotalGameTime)
            {
                foreach (var beh in Entity.Behaviors.SelectMany(l => l.Value.OfType<AttackBehavior>()))
                {
                    if (beh.OverrideTarget == _attackedBy)
                        beh.OverrideTarget = null;
                }

                _attackedBy = null;
            }
        }
    }
}
