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
    class BoomerangAttackBehavior : AttackBehavior
    {
        #region Attributes
        Boomerang _boomerang;
        FollowBehavior _followBehavior;
        TimeSpan _lastAttackTime;
        TimeSpan _stopTimeAfterHit = TimeSpan.FromSeconds(3);
        TimeSpan? _lastHitTime;
        #endregion

        #region Properties
        public TimeSpan TimeBetweenAttacks { get; set; }
        #endregion

        #region Constructors
        public BoomerangAttackBehavior(string targetCategory, float shootDistance, float maxDistance)
            : base(targetCategory, Arrow.FlightSpeed, maxDistance: maxDistance)
        {
            _followBehavior = new FollowBehavior(targetCategory, shootDistance) { MaxDistance = maxDistance };
            TimeBetweenAttacks = TimeSpan.FromSeconds(2);
        }
        #endregion

        public override bool IsActive(Microsoft.Xna.Framework.GameTime gameTime, QuestForTheCrown2.Levels.Level level)
        {
            if (_lastHitTime != null && _lastHitTime.Value + _stopTimeAfterHit > gameTime.TotalGameTime)
                return false;

            if (base.IsActive(gameTime, level) && Entity.Weapons != null)
            {
                if (_boomerang != null)
                    _boomerang.OnHit -= _boomerang_OnHit;

                _boomerang = Entity.Weapons.OfType<Boomerang>().FirstOrDefault();
                _followBehavior.Entity = Entity;
                if (_boomerang == null || !_followBehavior.IsActive(gameTime, level))
                    return false;

                _boomerang.OnHit += _boomerang_OnHit;
                var distanceInSeconds = CurrentTarget.Distance / Boomerang.FlightSpeed;
                var mpTravelDistance = distanceInSeconds;
                if (!level.ContainsEntity(_boomerang))
                    mpTravelDistance -= 0.5f;
                var mpNecessary = mpTravelDistance / 0.2;
                return Entity.Magic > mpNecessary;
            }
            return false;
        }

        void _boomerang_OnHit(object sender, EntityEventArgs e)
        {
            _lastHitTime = e.GameTime.TotalGameTime;
        }

        public override void Deactivated(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            if (_boomerang != null)
                _boomerang.Attack(gameTime, level, false, Vector2.Zero);
        }

        public override void Update(GameTime gameTime, Levels.Level level)
        {
            Entity.ChangeWeapon(_boomerang, level);

            var passedTime = gameTime.TotalGameTime - _lastAttackTime;

            if (_followBehavior.CurrentTarget.Distance <= _followBehavior.Distance)
            {
                Entity.Look(CurrentTarget.Position, true);

                bool attack = _lastAttackTime + TimeBetweenAttacks < gameTime.TotalGameTime;
                _boomerang.Attack(gameTime, level, attack, CurrentTarget.Position);
                OverrideRelativeTo = _boomerang;

                if (attack)
                    _lastAttackTime = gameTime.TotalGameTime;
            }
            else
            {
                _lastAttackTime = gameTime.TotalGameTime;
                _boomerang.Attack(gameTime, level, false, Vector2.Zero);
                level.RemoveEntity(_boomerang);
                _followBehavior.Update(gameTime, level);
            }
        }
    }
}
