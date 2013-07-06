using Microsoft.Xna.Framework;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using QuestForTheCrown2.Levels.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Weapons
{
    class Sword : Entity, IWeapon
    {
        bool _removeOnComplete;
        float _desiredAngle, _swingedAngle;
        float _swingSpeed = (float)Math.PI * 4;

        public Sword()
            : base(@"sprites\Sword.png", null)
        {
            OverlapEntities = true;
            Origin = new Vector2(Size.X / 2, Size.Y * 0.2f);
        }

        public void Attack(GameTime gameTime, Level level, Vector2 direction)
        {
            if (direction.Length() < 0.4)
            {
                if (_swingedAngle != 0)
                    _removeOnComplete = true;
                else
                {
                    level.RemoveEntity(this);
                }
                return;
            }
            _removeOnComplete = false;

            var oldDesired = _desiredAngle;
            _desiredAngle = (float)Math.Atan2(-direction.X, direction.Y);
            if (!level.ContainsEntity(this))
            {
                if (_desiredAngle > 0)
                    _swingedAngle = ToRadian(90);
                else
                    _swingedAngle = ToRadian(-90);
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
        }

        public override void Update(GameTime gameTime, Level level)
        {
            Position = new Vector2(
                x: Entity.Position.X + Entity.Size.X / 2,
                y: Entity.Position.Y + Entity.Size.Y / 2);

            foreach (var ent in GetCollisionRects().SelectMany(level.CollidesWith))
            {
                if (ent != this && !(ent is QuestForTheCrown2.Entities.Characters.Player))
                    level.RemoveEntity(ent);
            }

            if (_swingedAngle != 0)
            {
                var oldAngle = _swingedAngle;
                if (_swingedAngle < 0)
                    _swingedAngle += (float)(_swingSpeed * gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
                else
                    _swingedAngle -= (float)(_swingSpeed * gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
                if ((oldAngle < 0) != (_swingedAngle < 0))
                {
                    _swingedAngle = 0;
                    if (_removeOnComplete)
                    {
                        level.RemoveEntity(this);
                        _removeOnComplete = false;
                    }
                }
            }

            Angle = _desiredAngle + _swingedAngle;
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
                yield return new Rectangle((int)(location.X - rectSize/2), (int)(location.Y - rectSize / 2), rectSize, rectSize);
            }
        }

        static float ToRadian(float angle)
        {
            return (float)(Math.PI / 180) * angle;
        }

        public new Entity Entity { get; set; }
    }
}
