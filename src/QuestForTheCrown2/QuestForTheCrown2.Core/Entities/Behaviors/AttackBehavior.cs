using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Behaviors
{
    abstract class AttackBehavior : WalkBehavior
    {
        public float? MinDistance { get; set; }
        public float? MaxDistance { get; set; }
        public string TargetCategory { get; set; }
        public Entity OverrideTarget { get; set; }
        public Entity OverrideRelativeTo { get; set; }
        public float SpeedToEnemy { get; private set; }

        public EntityRelativePosition CurrentTarget { get; private set; }

        public AttackBehavior(string targetCategory, float speedToEnemy, float? minDistance = null, float? maxDistance = null)
        {
            if (speedToEnemy == 0)
                throw new ArgumentOutOfRangeException("speedToEnemy", "The speed to hit the enemy must not be zero.");

            if (string.IsNullOrEmpty(targetCategory))
                throw new ArgumentException("Target category must be non-empty", "targetCategory");

            TargetCategory = targetCategory;
            MinDistance = minDistance;
            MaxDistance = maxDistance;
            SpeedToEnemy = speedToEnemy;
        }

        public override bool IsActive(GameTime gameTime, Level level)
        {
            if (OverrideRelativeTo == null || OverrideRelativeTo.IsDead || !level.ContainsEntity(OverrideRelativeTo))
                OverrideRelativeTo = null;

            var relativeTo = OverrideRelativeTo ?? Entity;

            if (relativeTo == null)
                return false;

            if (OverrideTarget == null || OverrideTarget.IsDead || !level.ContainsEntity(OverrideTarget))
                OverrideTarget = null;

            if (OverrideTarget != null)
                CurrentTarget = relativeTo.PreviewEnemyLocation(gameTime, level, OverrideTarget, SpeedToEnemy);
            else
            {
                CurrentTarget = (from ent in level.GetEntities(TargetCategory)
                                 let position = ent.CenterPosition - relativeTo.CenterPosition
                                 let distance = position.Length()
                                 where MinDistance == null || distance > MinDistance
                                 where MaxDistance == null || distance < MaxDistance
                                 orderby distance
                                 select relativeTo.PreviewEnemyLocation(gameTime, level, ent, SpeedToEnemy))
                                .FirstOrDefault();
            }
            if (CurrentTarget == null || CurrentTarget.Entity.IsDead || !level.ContainsEntity(CurrentTarget.Entity))
                CurrentTarget = null;

            return CurrentTarget != null;
        }
    }
}
