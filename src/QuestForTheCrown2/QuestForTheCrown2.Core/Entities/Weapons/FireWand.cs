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
    class FireWand : Weapon
    {
        bool _oldAttackButton;

        public TimeSpan MaxFireBallFlyTime { get; set; }

        public FireWand()
            : base(@"sprites\Objects\FireWand.png", null)
        {
            OverlapEntities = true;
            Padding = new Rectangle(15, 0, 15, 0);
            MaxFireBallFlyTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Attack(GameTime gameTime, Level level, bool attackButton, Vector2 direction)
        {
            if (Parent == null)
                return;

            direction = Parent.CurrentDirection;

            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                if (direction.X > 0)
                    direction = new Vector2(1, 0);
                else
                    direction = new Vector2(-1, 0);
            }
            else
            {
                if (direction.Y > 0)
                    direction = new Vector2(0, 1);
                else
                    direction = new Vector2(0, -1);
            }

            if (attackButton && !_oldAttackButton && Parent.Magic > 0)
            {
                Parent.Magic.Quantity--;
                SoundManager.PlaySound("fireball");
                level.AddEntity(new FireBall(direction)
                {
                    Position = Parent.CenterPosition,
                    Parent = Parent,
                    MaxFlyTime = MaxFireBallFlyTime
                });
            }

            _oldAttackButton = attackButton;
        }
    }
}
