using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using QuestForTheCrown2.Levels.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Weapons
{
    class Sword : Entity, IWeapon
    {
        bool _attacking;

        public Sword() : base(@"sprites\Sword.png", null)
        {
            OverlapEntities = true;
            Origin = new Vector2(Size.X / 2, Size.Y * 0.2f);
        }

        public void Attack(GameTime gameTime, Level level, Vector2 direction)
        {
            Position = new Vector2(
                x: Entity.Position.X + Entity.Size.X / 2,
                y: Entity.Position.Y + Entity.Size.Y / 2);

            Angle = (float)Math.Atan2(-direction.X, direction.Y);

            level.AddEntity(this);
            _attacking = true;
        }

        public override void Update(GameTime gameTime, Level level)
        {
            if (!_attacking)
                level.RemoveEntity(this);

            _attacking = false;
        }

        public new Entity Entity { get; set; }
    }
}
