using Microsoft.Xna.Framework;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Weapons
{
    class Arrow : Entity
    {
        public const int FlightSpeed = 32 * 10;

        Vector2 _hitLocation;
        public Entity HitEntity { get; set; }
        TimeSpan _maxHitTime = TimeSpan.FromSeconds(5);
        TimeSpan _entHitTime;
        TimeSpan _timeFromCreation;

        public int PickupCount { get; set; }

        double _spriteAngle = (Math.PI / 8) * 6;
        Random _random;

        public Arrow()
            : base(@"sprites\Objects\Arrow.png", null)
        {
            PickupCount = 1;
            _random = new Random(Environment.TickCount);
            OverlapEntities = true;
            Origin = new Vector2(Size.X / 2, Size.Y / 2);
            _timeFromCreation = TimeSpan.Zero;
            Speed = FlightSpeed;
        }

        public Arrow(Vector2 direction)
            : this()
        {
            direction.Normalize();
            CurrentDirection = direction.Normalized();
        }

        public override Rectangle CollisionRect
        {
            get
            {
                var location = Position + CollisionDirection / 2;
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
            if (CurrentDirection != Vector2.Zero)
                Angle = (float)(Math.Atan2(-CurrentDirection.X, CurrentDirection.Y) + _spriteAngle);

            _timeFromCreation += gameTime.ElapsedGameTime;

            if (level.Map.Collides(CollisionRect, false, true) && CurrentDirection != Vector2.Zero)
            {
                if (level.Map.IsOutsideBorders(CollisionRect))
                    level.RemoveEntity(this);
                else
                    Dettach(level, removeFromLevel: Parent == null || Parent.Category != "Player");
                return;
            }

            if (HitEntity != null)
            {
                if (gameTime.TotalGameTime > _entHitTime + _maxHitTime || !level.ContainsEntity(HitEntity))
                    Dettach(level, HitEntity.Arrows == null || HitEntity.Health <= 0);
                else
                    Position = HitEntity.CenterPosition - _hitLocation;

                return;
            }

            if (Parent != null)
            {
                var timeFactor = gameTime.ElapsedGameTime.TotalMilliseconds / 1000;
                Position += CurrentDirection * (float)timeFactor * Speed;
            }

            foreach (var ent in level.CollidesWith(CollisionRect).Distinct())
            {
                if (Parent == null)
                {
                    if (ent.Arrows != null && ent.Arrows.Quantity < ent.Arrows.Maximum)
                    {
                        ent.Arrows.Quantity += PickupCount;
                        level.RemoveEntity(this);
                    }
                    return;
                }

                if (ent != this && ent != Parent && ent.Health != null)
                {
                    _entHitTime = gameTime.TotalGameTime;
                    HitEntity = ent;

                    var direction = VectorHelper.AngleToV2(Angle, 5);
                    direction = new Vector2(-direction.Y, direction.X);

                    ent.Hit(this, level, direction);
                    _hitLocation = ent.CenterPosition - Position - CurrentDirection;
                    return;
                }
            }
        }

        private void Dettach(Level level, bool removeFromLevel)
        {
            HitEntity = null;
            Parent = null;
            var oldPos = Position;
            Position -= new Vector2(CurrentDirection.X > 0 ? 8 : 0, CurrentDirection.Y > 0 ? 8 : 0);
            CurrentDirection = Vector2.Zero;

            if (level.Map.Collides(CollisionRect, false, true))
                Position = oldPos;

            if (removeFromLevel)
                level.RemoveEntity(this);
        }
    }
}
