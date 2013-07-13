﻿using Microsoft.Xna.Framework;
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
        Vector2 _hitLocation;
        public Entity HitEntity { get; set; }
        TimeSpan _maxHitTime = TimeSpan.FromSeconds(5);
        TimeSpan _entHitTime;
        TimeSpan _timeFromCreation;

        double _spriteAngle = (Math.PI / 8) * 6;

        public Arrow(Vector2 direction)
            : base(@"sprites\Objects\Arrow.png", null)
        {
            direction.Normalize();
            CurrentDirection = direction;
            OverlapEntities = true;
            Origin = new Vector2(Size.X / 2, Size.Y / 2);
            _timeFromCreation = TimeSpan.Zero;

            Speed = new Vector2(32 * 10);
        }

        public override Rectangle CollisionRect
        {
            get
            {
                var location = Position + CollisionDirection;
                return new Rectangle((int)(location.X - 4), (int)(location.Y - 4), 8, 8);
            }
        }

        Vector2 CollisionDirection
        {
            get
            {
                const int rectSize = 15;
                var collisionDirection = VectorHelper.AngleToV2((float)(Angle - _spriteAngle), rectSize);
                collisionDirection = new Vector2(-collisionDirection.Y, collisionDirection.X);
                return collisionDirection;
            }
        }

        public override void Update(GameTime gameTime, Levels.Level level)
        {
            Angle = (float)(Math.Atan2(-CurrentDirection.X, CurrentDirection.Y) + _spriteAngle);

            _timeFromCreation += gameTime.ElapsedGameTime;

            if (level.Map.Collides(CollisionRect))
            {
                level.RemoveEntity(this);
                return;
            }

            if (HitEntity != null)
            {
                if (gameTime.TotalGameTime > _entHitTime + _maxHitTime || !level.ContainsEntity(HitEntity))
                {
                    level.RemoveEntity(this);
                }
                else
                {
                    OverlapEntities = !OverlapEntities;
                    Position = HitEntity.CenterPosition - _hitLocation;
                }

                return;
            }

            var timeFactor = gameTime.ElapsedGameTime.TotalMilliseconds / 1000;
            Position += CurrentDirection * (float)timeFactor * Speed;

            foreach (var ent in level.CollidesWith(CollisionRect).Distinct())
            {
                if (ent != this && ent != Parent && ent.Health != null)
                {
                    _entHitTime = gameTime.TotalGameTime;
                    HitEntity = ent;

                    var direction = VectorHelper.AngleToV2(Angle, 5);
                    direction = new Vector2(-direction.Y, direction.X);

                    ent.Hit(this, level, direction);
                    OverlapEntities = false;
                    _hitLocation = ent.CenterPosition - Position - CurrentDirection * new Vector2(8);
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
