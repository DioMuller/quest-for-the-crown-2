using Microsoft.Xna.Framework;
using QuestForTheCrown2.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Base
{
    public class EntityEventArgs : GameEventArgs
    {
        public EntityEventArgs(Entity entity, GameTime gameTime, Level level)
            : base(gameTime, level)
        {
            Entity = entity;
        }

        public Entity Entity { get; private set; }
        
    }

    public delegate void EntityEventHandler(object sender, EntityEventArgs e);
}
