using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Objects
{
    class HealthContainer : Entity
    {
        public int PickupCount { get; set; }

        public HealthContainer()
            : base("gui/health_full.png", null)
        {
            OverlapEntities = true;
            PickupCount = 4;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            if(level.CollidesWith(CollisionRect).Where(e => e.Category == "Player" && !e.IsDead).Any())
            {
                if (Parent == null)
                {
                    foreach (var p in LevelCollection.CurrentPlayers)
                    {
                        p.Health.Maximum += PickupCount;
                        p.Health.Fill();
                    }
                    level.RemoveEntity(this);
                    return;
                }
            }
            base.Update(gameTime, level);
        }
    }
}
