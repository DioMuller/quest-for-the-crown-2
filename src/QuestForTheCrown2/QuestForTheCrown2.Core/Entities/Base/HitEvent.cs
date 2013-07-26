using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Weapons;
using QuestForTheCrown2.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Base
{
    public class HitEventArgs : EntityEventArgs
    {
        public HitEventArgs(Entity entity, Entity attacker, GameTime gameTime, Level level)
            : base(entity, gameTime, level)
        {
            Attacker = attacker;
        }

        public Entity Attacker { get; private set; }
    }

    public delegate void HitEventHandler(object sender, HitEventArgs e);
}
