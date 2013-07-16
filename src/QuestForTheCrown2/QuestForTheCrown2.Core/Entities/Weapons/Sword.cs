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
    class Sword : Weapon
    {
        bool _removeOnComplete, _rotationCompleted;
        float _desiredAngle, _swingedAngle;
        float _swingSpeed = (float)Math.PI * 4;
        int _swingDirection, _keepRotating;

        Vector2 _currentAttackDirection;
        bool _currentAttackButton;

        public Sword()
            : base(@"sprites\Objects\Sword.png", null)
        {
            OverlapEntities = true;
            Origin = new Vector2(Size.X / 2, Size.Y * 0.2f);
        }

        public override void Attack(GameTime gameTime, Level level, bool attackButton, Vector2 direction)
        {
            base.Update(gameTime, level);
            if (Parent == null)
                return;

            if (!attackButton && direction.Length() > 0.8)
                attackButton = true;
            else if (direction.Length() < 0.4)
                direction = Parent.CurrentDirection / 5;

            if (_currentAttackDirection == direction && attackButton == _currentAttackButton)
                return;

            CurrentDirection = direction;

            _currentAttackDirection = direction;
            _currentAttackButton = attackButton;


            if (!attackButton || direction == Vector2.Zero)
            {
                _removeOnComplete = true;
                if (!_rotationCompleted)
                    _keepRotating = 5;
                else if (_keepRotating <= 0)
                    level.RemoveEntity(this);
                return;
            }

            _removeOnComplete = false;
            _rotationCompleted = false;

            var oldDesired = _desiredAngle;
            _desiredAngle = (float)Math.Atan2(-direction.X, direction.Y);

            if (!level.ContainsEntity(this))
            {
                SoundManager.PlaySound("sword");
                if (_desiredAngle > 0)
                    _swingedAngle = MathHelper.ToRadians(90);
                else
                    _swingedAngle = MathHelper.ToRadians(-90);
                Angle = _swingedAngle;
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
            base.Update(gameTime, level);
            if (Parent == null)
                return;

            var lookDirection = VectorHelper.AngleToV2(Angle + (float)(Math.PI / 2), 1);
            Position = Parent.CenterPosition + lookDirection * new Vector2(15);

            foreach (var ent in GetCollisionRects().SelectMany(e => level.CollidesWith(e, true)).Distinct())
            {
                if (ent is Arrow && ((Arrow)ent).HitEntity == null)
                {
                    ent.CurrentDirection = CurrentDirection.Rotate(-_swingedAngle / 4).Normalized();
                    ent.Parent = Parent;
                }
                else if (ent != this && ent != Parent && ent.Health != null)
                {
                    var direction = VectorHelper.AngleToV2(Angle, 5);
                    direction = new Vector2(-direction.Y, direction.X);

                    ent.Hit(this, level, direction);
                }
            }

            if (!_rotationCompleted || _keepRotating > 0)
            {
                var oldAngle = _swingedAngle;
                _swingedAngle += (float)(_swingDirection * _swingSpeed * gameTime.ElapsedGameTime.TotalMilliseconds / 1000);

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
            Parent.Look(lookDirection, updateDirection: false);
        }

        private IEnumerable<Rectangle> GetCollisionRects()
        {
            const int rectSize = 5;

            var direction = VectorHelper.AngleToV2(Angle, rectSize);
            direction = new Vector2(-direction.Y, direction.X);

            var location = new Vector2(Position.X, Position.Y);

            for (int i = 0; i < 35; i += rectSize)
            {
                location += direction;
                yield return new Rectangle((int)(location.X - rectSize / 2), (int)(location.Y - rectSize / 2), rectSize, rectSize);
            }
        }
    }
}
