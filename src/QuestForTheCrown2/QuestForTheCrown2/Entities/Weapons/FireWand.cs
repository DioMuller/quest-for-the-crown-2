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
    class FireWand : Entity, IWeapon
    {
        bool _oldAttackButton;
        int _timeFromCreation;

        public FireWand()
            : base(@"sprites\Sword.png", null)
        {
        }

        public new Entity Entity { get; set; }

        public void Attack(GameTime gameTime, Level level, bool attackButton, Vector2 direction)
        {
            Parent = Entity;
            direction = Entity.CurrentDirection;
            
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

            if (attackButton && !_oldAttackButton)
                level.AddEntity(new FireBall(direction) { Position = Entity.CenterPosition, Speed = new Vector2(5 * 32), Parent = Entity });
            _oldAttackButton = attackButton;
        }
    }
}
