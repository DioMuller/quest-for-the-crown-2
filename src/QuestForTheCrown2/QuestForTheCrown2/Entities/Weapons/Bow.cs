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
    class Bow : Entity, IWeapon
    {
        double _spriteAngle = (Math.PI / 8) * 6;

        //Vector2 _currentAttackDirection;
        bool _currentAttackButton;

        public Bow()
            : base(@"sprites\Objects\Bow.png", null)
        {
            OverlapEntities = true;
            Origin = new Vector2(Size.X / 2, Size.Y / 2);
        }

        public void Attack(GameTime gameTime, Level level, bool attackButton, Vector2 direction)
        {
            if (!attackButton && direction == Vector2.Zero)
            {
                _currentAttackButton = attackButton;
                level.RemoveEntity(this);
                return;
            }

            if (!level.ContainsEntity(this))
                level.AddEntity(this);

            Position = Entity.CenterPosition;

            if (direction == Vector2.Zero)
                direction = Entity.CurrentDirection * -1;

            Angle = (float)(Math.Atan2(direction.X, -direction.Y) + _spriteAngle);

            if (attackButton && !_currentAttackButton)
                level.AddEntity(new Arrow(direction * -1) { Position = Entity.CenterPosition, Parent = Entity });
            _currentAttackButton = attackButton;
        }

        public override void Update(GameTime gameTime, Level level)
        {

        }

        public new Entity Entity { get; set; }
    }
}
