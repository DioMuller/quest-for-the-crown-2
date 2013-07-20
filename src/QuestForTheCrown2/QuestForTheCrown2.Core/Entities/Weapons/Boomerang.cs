using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.Entities.Weapons
{
    class Boomerang : Weapon
    {
        bool _flyingBack;
        Vector2 _direction;
        TimeSpan _startTime;
        TimeSpan _maxFlyTime;
        float _spinSpeed = (float)Math.PI / 8;

        public Boomerang()
            : base(@"sprites\Objects\Boomerang.png", null)
        {
            Origin = new Vector2(Size.X / 2, Size.Y / 2);
            Speed = 32 * 8;
            //Padding = new Rectangle(4, 4, 4, 4);
        }

        public override void Attack(GameTime gameTime, Level level, bool attackButton, Vector2 direction)
        {
            var isOnMap = level.ContainsEntity(this);

            if (!isOnMap)
            {
                if (attackButton)
                {
                    if (direction == Vector2.Zero)
                        direction = Parent.CurrentDirection;

                    _maxFlyTime = TimeSpan.FromSeconds(0.5);

                    SoundManager.PlaySound("boomerang");
                    _flyingBack = false;
                    _direction = direction;
                    _startTime = gameTime.TotalGameTime;
                    Position = Parent.CenterPosition + _direction;
                    level.AddEntity(this);
                }
            }
            else if (Parent != null && direction != Vector2.Zero)
            {
                if (!_flyingBack && _startTime + _maxFlyTime < gameTime.TotalGameTime && Parent.Magic > 1)
                {
                    _maxFlyTime = TimeSpan.FromSeconds(0.2);
                    Parent.Magic.Quantity--;
                    _startTime = gameTime.TotalGameTime;
                }

                if (_startTime + _maxFlyTime >= gameTime.TotalGameTime)
                    _direction = direction;
            }
        }

        public override void Update(GameTime gameTime, Level level)
        {
            base.Update(gameTime, level);
            if (Parent == null)
                return;

            var timeFactor = gameTime.ElapsedGameTime.TotalMilliseconds / 1000;

            _flyingBack = gameTime.TotalGameTime > _startTime + _maxFlyTime;

            if (_flyingBack)
            {
                _direction = new Vector2(Parent.CenterPosition.X - Position.X, Parent.CenterPosition.Y - Position.Y);
                _direction.Normalize();
            }

            Position += _direction * (float)timeFactor * Speed;

            Angle += _spinSpeed;

            if (level.Map.Collides(CollisionRect, false, true))
            {
                if (_flyingBack)
                {
                    Parent.RemoveWeapon(this);
                    OverlapEntities = true;
                    return;
                }
                else
                {
                    _startTime = TimeSpan.MinValue;
                    _flyingBack = true;
                }
            }

            foreach (var ent in level.CollidesWith(CollisionRect))
            {
                if (ent == this)
                    continue;

                if (ent == Parent)
                {
                    if (_flyingBack)
                    {
                        level.RemoveEntity(this);
                        return;
                    }
                }
                else
                    ent.Hit(this, level, _direction);
            }
        }

        public override void Unequiped(Level level) { }
    }
}
