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
    class FireWandAttackBehavior : AttackBehavior
    {
        #region Attributes
        FireWand _wand;
        TimeSpan _lastAttackTime;
        #endregion

        #region Properties
        public TimeSpan TimeBetweenAttacks { get; set; }
        #endregion

        public FireWandAttackBehavior(string targetCategory, float shootDistance, float maxDistance)
            : base(targetCategory, FireBall.FlightSpeed, maxDistance: maxDistance)
        {
            TimeBetweenAttacks = TimeSpan.FromSeconds(1);
        }

        public override bool IsActive(GameTime gameTime, Levels.Level level)
        {
            if (base.IsActive(gameTime, level) && Entity.Weapons != null)
            {
                _wand = Entity.Weapons.OfType<FireWand>().FirstOrDefault();
                return _wand != null && Entity.Magic > 0;
            }
            return false;
        }

        public override void Update(GameTime gameTime, Levels.Level level)
        {
            Entity.ChangeWeapon(_wand, level);

            var passedTime = gameTime.TotalGameTime - _lastAttackTime;

            if ((Math.Abs(CurrentTarget.Position.X) < 32 || Math.Abs(CurrentTarget.Position.Y) < 32) && passedTime.TotalSeconds > 2)
            {
                Entity.Look(CurrentTarget.Position, true);

                _wand.Attack(gameTime, level, true, CurrentTarget.Position);
                _wand.Attack(gameTime, level, false, CurrentTarget.Position);

                _lastAttackTime = gameTime.TotalGameTime;
            }
            else
            {
                if (Math.Abs(CurrentTarget.Position.X) < Math.Abs(CurrentTarget.Position.Y))
                {
                    if (Math.Abs(CurrentTarget.Position.X) > 5)
                        Walk(gameTime, level, new Vector2(CurrentTarget.Position.X > 0 ? 1 : -1, 0));
                    else StopWalking(gameTime, level);
                }
                else
                {
                    if (Math.Abs(CurrentTarget.Position.Y) > 5)
                        Walk(gameTime, level, new Vector2(0, CurrentTarget.Position.Y > 0 ? 1 : -1));
                    else StopWalking(gameTime, level);
                }
            }
        }
    }
}
