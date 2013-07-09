using Microsoft.Xna.Framework;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Weapons
{
    class Arrow : Entity
    {
        Vector2 _direction;
        Vector2 _hitLocation;
        Entity _hitEntity;
        TimeSpan _maxHitTime = TimeSpan.FromSeconds(5);
        TimeSpan _entHitTime;
        TimeSpan _timeFromCreation;

        double _spriteAngle = (Math.PI / 8) * 6;


        Vector2 _collisionDirection;

        public Arrow(Vector2 direction)
            : base(@"sprites\Arrow.png", null)
        {
            _direction = direction;
            OverlapEntities = true;
            Angle = (float)(Math.Atan2(-direction.X, direction.Y) + _spriteAngle);
            Origin = new Vector2(Size.X / 2, Size.Y / 2);
            _timeFromCreation = TimeSpan.Zero;

            Speed = new Vector2(32 * 10);

            const int rectSize = 15;
            _collisionDirection = VectorHelper.AngleToV2((float)(Angle - _spriteAngle), rectSize);
            _collisionDirection = new Vector2(-_collisionDirection.Y, _collisionDirection.X);
        }

        public override Rectangle CollisionRect
        {
            get
            {
                var location = Position + _collisionDirection;
                return new Rectangle((int)(location.X - 4), (int)(location.Y - 4), 8, 8);
            }
        }

        public override void Update(GameTime gameTime, Levels.Level level)
        {
            _timeFromCreation += gameTime.ElapsedGameTime;

            if (level.Map.Collides(CollisionRect))
            {
                level.RemoveEntity(this);
                return;
            }

            if (_hitEntity != null)
            {
                if (gameTime.TotalGameTime > _entHitTime + _maxHitTime || !level.ContainsEntity(_hitEntity))
                {
                    level.RemoveEntity(this);
                }
                else
                {
                    OverlapEntities = !OverlapEntities;
                    Position = _hitEntity.CenterPosition - _hitLocation;
                }

                return;
            }

            var timeFactor = gameTime.ElapsedGameTime.TotalMilliseconds / 1000;
            Position += _direction * (float)timeFactor * Speed;

            foreach (var ent in level.CollidesWith(CollisionRect).Distinct())
            {
                if (ent != this && ent != Parent && ent.Health != null)
                {
                    _entHitTime = gameTime.TotalGameTime;
                    _hitEntity = ent;
                    ent.Hit(this, level, Angle);
                    OverlapEntities = false;
                    _hitLocation = ent.CenterPosition - Position - _direction * new Vector2(8);
                    return;
                }
            }

            if (_timeFromCreation > TimeSpan.FromSeconds(3.0))
            {
                level.RemoveEntity(this);
            }
        }
    }
}
