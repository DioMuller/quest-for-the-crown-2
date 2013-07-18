using Microsoft.Xna.Framework;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Behaviors
{
    class BowAttackBehavior : AttackBehavior
    {
        #region Attributes
        Bow _bow;
        FollowBehavior _followBehavior;
        TimeSpan _lastAttackTime;
        #endregion

        #region Properties
        public TimeSpan TimeBetweenAttacks { get; set; }
        #endregion

        #region Constructors
        public BowAttackBehavior(string targetCategory, float shootDistance, float maxDistance)
            : base(targetCategory, Arrow.FlightSpeed, maxDistance: maxDistance)
        {
            _followBehavior = new FollowBehavior(targetCategory, shootDistance) { MaxDistance = maxDistance };
            TimeBetweenAttacks = TimeSpan.FromSeconds(2);
        }
        #endregion

        public override bool IsActive(Microsoft.Xna.Framework.GameTime gameTime, QuestForTheCrown2.Levels.Level level)
        {
            if (base.IsActive(gameTime, level) && Entity.Weapons != null)
            {
                _bow = Entity.Weapons.OfType<Bow>().FirstOrDefault();
                _followBehavior.Entity = Entity;
                return _bow != null && Entity.Arrows > 0 && _followBehavior.IsActive(gameTime, level);
            }
            return false;
        }

        public override void Deactivated(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            if (_bow != null)
                _bow.Attack(gameTime, level, false, Vector2.Zero);
        }

        public override void Update(GameTime gameTime, Levels.Level level)
        {
            Entity.ChangeWeapon(_bow, level);

            var passedTime = gameTime.TotalGameTime - _lastAttackTime;

            if (_followBehavior.CurrentTarget.Distance <= _followBehavior.Distance)
            {
                Entity.Look(CurrentTarget.Position, true);

                bool fireArrow = _lastAttackTime + TimeBetweenAttacks < gameTime.TotalGameTime;
                _bow.Attack(gameTime, level, fireArrow, CurrentTarget.Position * (OptionsManager.CurrentOptions.InvertAim ? -1 : 1));

                if (fireArrow)
                    _lastAttackTime = gameTime.TotalGameTime;
            }
            else
            {
                _lastAttackTime = gameTime.TotalGameTime;
                _bow.Attack(gameTime, level, false, Vector2.Zero);
                level.RemoveEntity(_bow);
                _followBehavior.Update(gameTime, level);
            }
        }
    }
}
