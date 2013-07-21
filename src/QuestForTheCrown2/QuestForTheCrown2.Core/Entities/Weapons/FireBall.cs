using Microsoft.Xna.Framework;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Weapons
{
    class FireBall : Entity
    {
        public const int FlightSpeed = 32 * 8;

        public Entity HitEntity { get; set; }
        TimeSpan _maxHitTime = TimeSpan.FromSeconds(1);
        TimeSpan _entHitTime;
        TimeSpan _timeFromCreation;

        public TimeSpan MaxFlyTime { get; set; }

        public FireBall(Vector2 direction)
            : base(@"sprites\Objects\FireBall.png", null)
        {
            MaxFlyTime = TimeSpan.FromSeconds(0.5);
            CurrentDirection = direction;
            OverlapEntities = true;
            Angle = (float)Math.Atan2(-direction.X, direction.Y);
            Origin = new Vector2(Size.X / 2, Size.Y / 2);
            _timeFromCreation = TimeSpan.Zero;
            Speed = FlightSpeed;
        }

        public override void Update(GameTime gameTime, Levels.Level level)
        {
            _timeFromCreation += gameTime.ElapsedGameTime;

            if (level.Map.Collides(CollisionRect, false, true))
            {
                level.RemoveEntity(this);
                return;
            }

            if (HitEntity != null)
            {
                SoundManager.PlaySound("onfire");
                if (gameTime.TotalGameTime > _entHitTime + _maxHitTime)
                {
                    level.RemoveEntity(this);
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

                    ent.Hit(this, gameTime, level, direction);
                    if (HitEntity != null)
                    {
                        OverlapEntities = false;
                        Position = ent.CenterPosition;
                        Angle = 0;
                        return;
                    }
                    else _timeFromCreation = TimeSpan.Zero;
                }
            }

            if (_timeFromCreation > MaxFlyTime)
            {
                level.RemoveEntity(this);
            }
        }
    }
}
