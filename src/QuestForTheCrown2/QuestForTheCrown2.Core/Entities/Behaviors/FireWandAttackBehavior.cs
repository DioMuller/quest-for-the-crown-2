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
    class FireWandAttackBehavior : WalkBehavior
    {
        #region Attributes
        EntityRelativePosition _target;
        FireWand _fireWand;
        TimeSpan _lastAttackTime;
        string _targetCategory;
        float _maxDistance;
        #endregion

        #region Properties
        public TimeSpan TimeBetweenAttacks { get; set; }
        #endregion

        public FireWandAttackBehavior(string targetCategory, float shootDistance, float maxDistance)
        {
            _maxDistance = maxDistance;
            _targetCategory = targetCategory;
            TimeBetweenAttacks = TimeSpan.FromSeconds(1);
        }

        public override bool IsActive(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            if (Entity.IsDead)
                return false;

            _fireWand = Entity.Weapons.OfType<FireWand>().FirstOrDefault();
            _target = level.EntityCloserTo(Entity, _targetCategory);

            if (_target == null)
                return false;

            return _fireWand != null && Entity.Magic > 0 && _target.Distance <= _maxDistance;
        }

        public override void Deactivated(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            _fireWand.Attack(gameTime, level, false, Vector2.Zero);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            Entity.ChangeWeapon(_fireWand, level);

            var enemyPosition = Entity.PreviewEnemyLocation(gameTime, level, _target.Entity, FireBall.FlyghtSpeed);

            var passedTime = gameTime.TotalGameTime - _lastAttackTime;

            if ((Math.Abs(enemyPosition.X) < 32 || Math.Abs(enemyPosition.Y) < 32) && passedTime.TotalSeconds > 2)
            {
                Entity.Look(enemyPosition, true);

                _fireWand.Attack(gameTime, level, true, enemyPosition);
                _fireWand.Attack(gameTime, level, false, enemyPosition);

                _lastAttackTime = gameTime.TotalGameTime;
            }
            else
            {
                if (Math.Abs(enemyPosition.X) < Math.Abs(enemyPosition.Y))
                {
                    if (Math.Abs(enemyPosition.X) > 5)
                        Walk(gameTime, level, new Vector2(enemyPosition.X > 0 ? 1 : -1, 0));
                    else StopWalking(gameTime, level);
                }
                else
                {
                    if (Math.Abs(enemyPosition.Y) > 5)
                        Walk(gameTime, level, new Vector2(0, enemyPosition.Y > 0 ? 1 : -1));
                    else StopWalking(gameTime, level);
                }
            }
        }
    }
}
