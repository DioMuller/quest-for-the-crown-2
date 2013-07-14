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
            if (Entity == null)
                return false;

            _fireWand = Entity.Weapons.OfType<FireWand>().FirstOrDefault();
            _target = level.EntityCloserTo(Entity, _targetCategory);

            return _fireWand != null && Entity.Magic > 0 && _target.Distance <= _maxDistance;
        }

        public override void Deactivated(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            _fireWand.Attack(gameTime, level, false, Vector2.Zero);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            Entity.ChangeWeapon(_fireWand, level);

            var passedTime = gameTime.TotalGameTime - _lastAttackTime;

            if (Math.Abs(_target.Position.X) < 32 || Math.Abs(_target.Position.Y) < 32)
            {
                if (passedTime.TotalSeconds > 3)
                {
                    Entity.Look(_target.Position, true);

                    _fireWand.Attack(gameTime, level, true, _target.Position);
                    _fireWand.Attack(gameTime, level, false, _target.Position);

                    _lastAttackTime = gameTime.TotalGameTime;
                }
            }
            else
            {
                _fireWand.Attack(gameTime, level, false, Vector2.Zero);
                level.RemoveEntity(_fireWand);

                if (Math.Abs(_target.Position.X) < Math.Abs(_target.Position.Y))
                    Walk(gameTime, level, new Vector2(_target.Position.X > 0 ? 1 : -1, 0));
                else
                    Walk(gameTime, level, new Vector2(0, _target.Position.Y > 0 ? 1 : -1));
            }
        }
    }
}
