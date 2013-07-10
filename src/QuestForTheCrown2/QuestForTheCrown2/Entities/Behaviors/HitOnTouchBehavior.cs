using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Behaviors
{
    class HitOnTouchBehavior : EntityUpdateBehavior
    {
        public override bool IsActive(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            return true;
        }

        public override void Update(GameTime gameTime, Level level)
        {
            var col = Entity.CollisionRect;
            var ent = level.CollidesWith(new Rectangle(
                                            x: col.X - 4,
                                            y: col.Y - 4,
                                            width: col.Width + 8,
                                            height: col.Height + 8))
                            .FirstOrDefault(e => e != Entity);

            if (ent != null)
                ent.Hit(Entity, level, Entity.CurrentDirection);
        }
    }
}
