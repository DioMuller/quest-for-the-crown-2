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
        Vector2 _direction;
        Entity _hitEntity;
        TimeSpan _maxHitTime = TimeSpan.FromSeconds(1);
        TimeSpan _entHitTime;
        TimeSpan _timeFromCreation;

        public TimeSpan MaxFlyTime { get; set; }

        public FireBall(Vector2 direction)
            : base(@"sprites\Objects\FireBall.png", null)
        {
            MaxFlyTime = TimeSpan.FromSeconds(1);
            _direction = direction;
            OverlapEntities = true;
            Angle = (float)Math.Atan2(-direction.X, direction.Y);
            Origin = new Vector2(Size.X / 2, Size.Y / 2);
            _timeFromCreation = TimeSpan.Zero;
        }

        public override void Update(GameTime gameTime, Levels.Level level)
        {
            _timeFromCreation += gameTime.ElapsedGameTime;

            if (level.Map.Collides(CollisionRect, false, true))
            {
                level.RemoveEntity(this);
                return;
            }

            if (_hitEntity != null)
            {
                SoundManager.PlaySound("onfire");
                if (gameTime.TotalGameTime > _entHitTime + _maxHitTime)
                {
                    level.RemoveEntity(this);
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

                    var direction = VectorHelper.AngleToV2(Angle, 5);
                    direction = new Vector2(-direction.Y, direction.X);

                    ent.Hit(this, level, direction);
                    OverlapEntities = false;
                    Position = ent.CenterPosition;
                    Angle = 0;
                    return;
                }
            }

            if (_timeFromCreation > MaxFlyTime)
            {
                level.RemoveEntity(this);
            }
        }
    }
}
