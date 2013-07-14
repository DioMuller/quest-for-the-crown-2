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
    class Bow : Weapon
    {
        double _spriteAngle = (Math.PI / 8) * 6;

        //Vector2 _currentAttackDirection;
        bool _currentAttackButton;
        TimeSpan _lastShootDate;
        TimeSpan _shootDisabledDelay = TimeSpan.FromSeconds(0.5);

        public Arrow LastShotArrow { get; private set; }

        bool CanShoot(GameTime gameTime)
        {
            return _lastShootDate + _shootDisabledDelay < gameTime.TotalGameTime;
        }

        public Bow()
            : base(@"sprites\Objects\Bow.png", new Point(30, 30))
        {
            OverlapEntities = true;
            Origin = new Vector2(Size.X / 2, Size.Y / 2);
            SpriteSheet.AddAnimation("unloaded", "default", line: 0, frameDuration: TimeSpan.Zero);
            SpriteSheet.AddAnimation("loaded", "default", line: 1, frameDuration: TimeSpan.Zero);
        }

        public override void Attack(GameTime gameTime, Level level, bool attackButton, Vector2 direction)
        {
            if (!attackButton && direction == Vector2.Zero)
            {
                _currentAttackButton = attackButton;
                level.RemoveEntity(this);
                return;
            }

            if (!level.ContainsEntity(this))
                level.AddEntity(this);

            Position = Parent.CenterPosition;

            direction *= (OptionsManager.CurrentOptions.InvertAim ? 1 : -1);

            if (direction == Vector2.Zero)
                direction = Parent.CurrentDirection * -1;

            Angle = (float)(Math.Atan2(direction.X, -direction.Y) + _spriteAngle);

            if (attackButton && !_currentAttackButton && Parent.Arrows > 0)
            {
                Parent.Arrows--;
                LastShotArrow = new Arrow(direction.Normalized() * -1) { Position = Parent.CenterPosition, Parent = Parent };
                level.AddEntity(LastShotArrow);
                _lastShootDate = gameTime.TotalGameTime;
            }
            _currentAttackButton = attackButton;
        }

        public override void Update(GameTime gameTime, Level level)
        {
            if (Parent != null && Parent.Arrows > 0 && CanShoot(gameTime))
                CurrentAnimation = "loaded";
            else
                CurrentAnimation = "unloaded";

            base.Update(gameTime, level);
        }
    }
}
