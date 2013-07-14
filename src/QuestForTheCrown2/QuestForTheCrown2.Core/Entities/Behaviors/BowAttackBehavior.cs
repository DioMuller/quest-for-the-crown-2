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
    class BowAttackBehavior : WalkBehavior
    {
        #region Attributes
        Bow _bow;
        FollowBehavior _followBehavior;
        TimeSpan _lastAttackTime;
        #endregion

        #region Properties
        public TimeSpan TimeBetweenAttacks { get; set; }
        #endregion

        public BowAttackBehavior(string targetCategory, float shootDistance, float maxDistance)
        {
            TimeBetweenAttacks = TimeSpan.FromSeconds(1);
            _followBehavior = new FollowBehavior(targetCategory, shootDistance) { MaxDistance = maxDistance };
        }

        public override bool IsActive(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            if (Entity == null)
                return false;

            _bow = Entity.Weapons.OfType<Bow>().FirstOrDefault();
            _followBehavior.Entity = Entity;

            return _bow != null && Entity.Arrows > 0 &&
                   _followBehavior.IsActive(gameTime, level);
        }

        public override void Deactivated(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            _bow.Attack(gameTime, level, false, Vector2.Zero);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            Entity.ChangeWeapon(_bow, level);

            var passedTime = gameTime.TotalGameTime - _lastAttackTime;

            if (_followBehavior.CurrentTarget.Distance <= _followBehavior.Distance && (!level.ContainsEntity(_bow.LastShotArrow) || _bow.LastShotArrow == null || _bow.LastShotArrow.Parent == null || passedTime.TotalSeconds > 3))
            {
                if (passedTime.TotalSeconds > 0.5)
                {
                    Entity.Look(_followBehavior.CurrentTarget.Position, true);

                    bool fireArrow = _lastAttackTime + TimeBetweenAttacks < gameTime.TotalGameTime;
                    _bow.Attack(gameTime, level, fireArrow, _followBehavior.CurrentTarget.Position * (OptionsManager.CurrentOptions.InvertAim? -1:1));

                    if (fireArrow)
                        _lastAttackTime = gameTime.TotalGameTime;
                }
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
