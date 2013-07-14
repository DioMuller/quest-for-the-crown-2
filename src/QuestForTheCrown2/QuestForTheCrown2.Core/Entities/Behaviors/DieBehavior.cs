using QuestForTheCrown2.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Behaviors
{
    class DieBehavior : EntityUpdateBehavior
    {
        public override bool IsActive(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            return Entity.Health != null && Entity.Health <= 0;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            // TODO: play dying animation;
            level.RemoveEntity(Entity);
        }
    }
}
