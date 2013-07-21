using QuestForTheCrown2.Entities.Base;
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
            foreach (var ent in level.CollidesWith(CollisionRect).Where(e => !e.IsDead).Distinct())
            {
                if (Parent == null)
                {
                    if (ent.Health != null)
                    {
                        ent.Health.Maximum += PickupCount;
                        ent.Health.Fill();
                        level.RemoveEntity(this);
                    }
                    return;
                }
            }
            base.Update(gameTime, level);
        }
    }
}
