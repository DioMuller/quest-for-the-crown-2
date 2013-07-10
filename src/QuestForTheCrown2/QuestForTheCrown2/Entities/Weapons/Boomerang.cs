using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Weapons
{
    class Boomerang : Entity, IWeapon
    {
        Vector2 _direction;
        TimeSpan _startTime;
        TimeSpan _maxFlyTime = TimeSpan.FromSeconds(0.5);
        float _spinSpeed = (float)Math.PI / 8;

        public new Entity Entity { get; set; }

        public Boomerang()
            : base(@"sprites\Boomerang.png", null)
        {
            Origin = new Vector2(Size.X / 2, Size.Y / 2);
            Speed = new Vector2(32 * 8);
            //Padding = new Rectangle(4, 4, 4, 4);
        }

        public void Attack(GameTime gameTime, Level level, bool attackButton, Vector2 direction)
        {
            if (direction == Vector2.Zero)
                direction = Entity.CurrentDirection;

            if (!level.ContainsEntity(this) && attackButton)
            {
                _direction = direction;
                _startTime = gameTime.TotalGameTime;
                Position = Entity.CenterPosition + _direction;
                level.AddEntity(this);
            }
        }

        public override void Update(GameTime gameTime, Level level)
        {
            if (Entity == null)
            {
                // Boomerang has no owner!
                Entity = level.CollidesWith(CollisionRect).FirstOrDefault(e => e.Category == "Player");
                if (Entity != null)
                {
                    Entity.AddWeapon(this);
                    level.RemoveEntity(this);
                    OverlapEntities = false;
                }
                return;
            }

            var timeFactor = gameTime.ElapsedGameTime.TotalMilliseconds / 1000;

            bool flyingBack = gameTime.TotalGameTime > _startTime + _maxFlyTime;

            if (flyingBack)
            {
                _direction = new Vector2(Entity.CenterPosition.X - Position.X, Entity.CenterPosition.Y - Position.Y);
                _direction.Normalize();
            }

            Position += _direction * (float)timeFactor * Speed;

            Angle += _spinSpeed;

            if (level.Map.Collides(CollisionRect))
            {
                if (flyingBack)
                {
                    Entity.RemoveWeapon(this);
                    OverlapEntities = true;
                    return;
                }
                else
                {
                    _startTime = TimeSpan.MinValue;
                }
            }

            foreach (var ent in level.CollidesWith(CollisionRect))
            {
                if (ent == this)
                    continue;

                if (ent == Entity)
                {
                    if (flyingBack)
                    {
                        level.RemoveEntity(this);
                        return;
                    }
                }
                else
                    ent.Hit(this, level, _direction);
            }
        }
    }
}
