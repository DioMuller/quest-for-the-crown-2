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
        public const int FlightSpeed = 32 * 8;
        public event EntityEventHandler OnHit;

        bool _hitParent;
        bool _flyingBack;
        Vector2 _direction;
        TimeSpan _startTime;
        TimeSpan _maxFlyTime;
        float _spinSpeed = (float)Math.PI / 8;

        public Boomerang()
            : base(@"sprites\Objects\Boomerang.png", null)
        {
            Origin = new Vector2(Size.X / 2, Size.Y / 2);
            Speed = FlightSpeed;
            MoveOnHit = true;
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

                    direction.Normalize();

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
                if (_startTime + _maxFlyTime < gameTime.TotalGameTime)
                {
                    if (Parent.Magic > 1)
                    {
                        _maxFlyTime = TimeSpan.FromSeconds(0.2);
                        Parent.Magic.Quantity--;
                        _startTime = gameTime.TotalGameTime;
                        _flyingBack = false;
                    }
                    else _flyingBack = true;
                }
                else _flyingBack = false;

                if (!_flyingBack)
                {
                    _direction = direction;
                    _direction.Normalize();
                }
            }
        }

        public override void Update(GameTime gameTime, Level level)
        {
            base.Update(gameTime, level);
            if (Parent == null)
                return;

            if(!_flyingBack)
                _flyingBack = gameTime.TotalGameTime > _startTime + _maxFlyTime;

            var timeFactor = gameTime.ElapsedGameTime.TotalMilliseconds / 1000;

            if (_flyingBack)
            {
                _direction = new Vector2(Parent.CenterPosition.X - Position.X, Parent.CenterPosition.Y - Position.Y);
                _direction.Normalize();
            }

            Position += _direction * (float)timeFactor * Speed;

            Angle += _spinSpeed;

            if (level.Map.Collides(CollisionRect, false, true))
            {
                Hit(null, level, _direction * -1);
                return;
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
                        if (_hitParent)
                            Parent.Hit(this, level, _direction);
                        _hitParent = false;
                        return;
                    }
                }
                else
                {
                    ent.Hit(this, level, _direction);
                    Hit(ent, level, _direction * -1);

                    if (OnHit != null)
                        OnHit(this, new EntityEventArgs(ent, gameTime, level));
                }
            }
        }

        public override void Hit(Entity attacker, Level level, Vector2 direction)
        {
            if (_flyingBack)
            {
                if (Parent != null && Parent.Category == "Player")
                {
                    Parent.RemoveWeapon(this);
                    OverlapEntities = true;
                }
            }
            else
            {
                //if (attacker is Weapon)
                //    _hitParent = true;

                _startTime = TimeSpan.MinValue;
                _flyingBack = true;
            }

            base.Hit(attacker, level, direction);
        }

        public override void Unequiped(Level level) { }
    }
}
