using Microsoft.Xna.Framework;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using QuestForTheCrown2.Levels.Mapping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Weapons
{
    class Sword : Entity, IWeapon
    {
        bool _removeOnComplete, _rotationCompleted;
        float _desiredAngle, _swingedAngle;
        float _swingSpeed = (float)Math.PI * 6;
        int _swingDirection, _keepRotating;
        float _swingSpeedMultiplier;

        Vector2 _currentAttackDirection;
        float _currentAttackForce;

        public Sword()
            : base(@"sprites\Sword.png", null)
        {
            OverlapEntities = true;
            Origin = new Vector2(Size.X / 2, Size.Y * 0.2f);
        }

        public void Attack(GameTime gameTime, Level level, float force, Vector2 direction)
        {
            if (force <= 0.01 && direction.Length() > 0.8)
                force = direction.Length();
            else if (direction.Length() < 0.4)
                direction = Entity.CurrentDirection / 5;

            if (_currentAttackDirection == direction && force == _currentAttackForce)
                return;

            _currentAttackDirection = direction;
            _currentAttackForce = force;


            if (force < 0.4 || direction == Vector2.Zero)
            {
                _removeOnComplete = true;
                if (!_rotationCompleted)
                    _keepRotating = 5;
                else if (_keepRotating <= 0)
                    level.RemoveEntity(this);
                return;
            }

            _swingSpeedMultiplier = force;
            _removeOnComplete = false;
            _rotationCompleted = false;

            var oldDesired = _desiredAngle;
            _desiredAngle = (float)Math.Atan2(-direction.X, direction.Y);

            if (!level.ContainsEntity(this))
            {
                if (_desiredAngle > 0)
                    _swingedAngle = ToRadian(90);
                else
                    _swingedAngle = ToRadian(-90);
                Angle = _swingedAngle;
                Parent = Entity;
                level.AddEntity(this);
            }
            else if (_swingedAngle == 0)
            {
                var oldAngle = _swingedAngle;
                _swingedAngle -= _desiredAngle - Angle + _swingedAngle;
                if (Math.Abs(_swingedAngle) - 0.1 > Math.PI)
                {
                    if (_swingedAngle > 0)
                        _swingedAngle -= (float)Math.PI * 2;
                    else
                        _swingedAngle += (float)Math.PI * 2;
                }
            }

            if (_swingedAngle > 0)
                _swingDirection = -1;
            else
                _swingDirection = 1;
        }

        public override void Update(GameTime gameTime, Level level)
        {
            Position = Entity.CenterPosition;

            foreach (var ent in GetCollisionRects().SelectMany(level.CollidesWith).Distinct())
            {
                if (ent != this && ent != Entity && ent.Health != null)
                {
                    Hit(level, ent);
                }
            }

            if (!_rotationCompleted || _keepRotating > 0)
            {
                var oldAngle = _swingedAngle;
                _swingedAngle += (float)(_swingDirection * _swingSpeed * _swingSpeedMultiplier * gameTime.ElapsedGameTime.TotalMilliseconds / 1000);

                if (!_rotationCompleted && (_swingDirection > 0 == _swingedAngle > 0))
                {
                    _rotationCompleted = true;
                    if (!_removeOnComplete)
                        _swingedAngle = 0;
                }

                if (_rotationCompleted)
                {
                    _keepRotating--;
                    if (_removeOnComplete && _keepRotating <= 0)
                    {
                        level.RemoveEntity(this);
                        _removeOnComplete = false;
                    }
                }
            }

            Angle = _desiredAngle + _swingedAngle;
            Entity.Look(VectorHelper.AngleToV2(Angle + (float)(Math.PI / 2), 1), updateDirection: false);
        }

        private void Hit(Level level, Base.Entity ent)
        {
            if (!ent.IsBlinking)
                ent.Health--;

            if (ent.Health <= 0)
                level.RemoveEntity(ent);
            else
            {
                var direction = VectorHelper.AngleToV2(Angle, 5);
                direction = new Vector2(-direction.Y, direction.X);
                var oldPos = ent.Position;
                ent.Position += direction;
                if (level.CollidesWith(ent.CollisionRect).Any(e => e != ent) || level.Map.Collides(ent.CollisionRect))
                    ent.Position = oldPos;
            }
        }

        private IEnumerable<Rectangle> GetCollisionRects()
        {
            const int rectSize = 5;

            var direction = VectorHelper.AngleToV2(Angle, rectSize);
            direction = new Vector2(-direction.Y, direction.X);

            var location = new Vector2(Position.X, Position.Y);

            for (int i = 0; i < 30; i += rectSize)
            {
                location += direction;
                yield return new Rectangle((int)(location.X - rectSize / 2), (int)(location.Y - rectSize / 2), rectSize, rectSize);
            }
        }

        static float ToRadian(float angle)
        {
            return (float)(Math.PI / 180) * angle;
        }

        public new Entity Entity { get; set; }
    }
}
