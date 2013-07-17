using Microsoft.Xna.Framework;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Behaviors
{
    class SpiralAttackBehavior : WalkBehavior
    {
        #region Attributes
        EntityRelativePosition _currentTarget;
        bool _dodging = true;
        double _safeDistanceRange = 5;
        float _currentDistance;
        bool _forceRotate;
        bool _forcedDirection;
        int? _oldHealth;
        #endregion

        #region Properties
        public string TargetCategory { get; set; }
        public float? MaxDistance { get; set; }
        public float Distance { get; set; }
        #endregion

        public SpiralAttackBehavior(string targetCategory, float distance = 23 * 3)
        {
            TargetCategory = targetCategory;
            Distance = distance;
            _currentDistance = Distance;
        }

        public override bool IsActive(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            _currentTarget = level.EntityCloserTo(Entity, TargetCategory);
            if (_currentTarget != null && (MaxDistance == null || _currentTarget.Distance < MaxDistance))
                return true;

            Walk(gameTime, level, Vector2.Zero);
            return false;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            Entity.ChangeWeapon(null, level);

            if (_oldHealth != null && _oldHealth > Entity.Health.Quantity)
            {
                _dodging = true;
                _forceRotate = false;
                _currentDistance *= 1.5f;
                _forcedDirection = !_forcedDirection;
            }

            var targetLocation = Entity.PreviewEnemyLocation(gameTime, level, _currentTarget.Entity, Entity.Speed);
            var entLocation = Entity.CenterPosition;
            var directRoute = _currentTarget.Position;

            if (directRoute.X < 0)
                Entity.CurrentView = "left";
            else
                Entity.CurrentView = "right";
            Entity.CurrentAnimation = "walking";

            var normalized = directRoute.Normalized();

            if (_dodging)
            {
                if (!_forceRotate && directRoute.Length() > _currentDistance + _safeDistanceRange)
                {
                    if (!Walk(gameTime, level, normalized, e => e.GetType() != Entity.GetType()))
                        _dodging = false;
                    _safeDistanceRange = 5;
                }

                else if (!_forceRotate && directRoute.Length() < _currentDistance - _safeDistanceRange)
                {
                    if (!Walk(gameTime, level, normalized * -1, e => e.GetType() != Entity.GetType()))
                        _dodging = false;
                    _safeDistanceRange = 5;
                }
                else
                {
                    _safeDistanceRange = 32;

                    var targetRoute = (directRoute * -1).Rotate(MathHelper.ToRadians(1) * (_forcedDirection ? -1 : 1));
                    var walkRoute = (targetRoute + _currentTarget.Entity.CenterPosition) - entLocation;
                    walkRoute = walkRoute.Rotate(-0.2f * (_forcedDirection ? -1 : 1));
                    normalized = walkRoute.Normalized();

                    _currentDistance = (_currentTarget.Entity.CenterPosition - (entLocation + walkRoute)).Length();

                    if (!Walk(gameTime, level, normalized * -1, e => e.GetType() != Entity.GetType()))
                    {
                        _dodging = false;
                        _forceRotate = true;
                        _forcedDirection = !_forcedDirection;
                    }
                }
            }
            else if (!Walk(gameTime, level, normalized, e => e.GetType() != Entity.GetType()))
            {
                _dodging = true;
                _forceRotate = true;
                _forcedDirection = !_forcedDirection;
                _currentDistance = Distance;
            }

            _oldHealth = Entity.Health.Quantity;
        }
    }
}
