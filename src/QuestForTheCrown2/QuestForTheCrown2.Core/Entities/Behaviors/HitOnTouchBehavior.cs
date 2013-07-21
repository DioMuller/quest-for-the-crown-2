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
        Func<Entity, bool> _canHit;

        public HitOnTouchBehavior(Func<Entity, bool> canHit = null)
        {
            _canHit = canHit;
            ExtraHitAreaSize = 8;
        }

        public override bool IsActive(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            return !Entity.IsDead;
        }

        public override void Update(GameTime gameTime, Level level)
        {
            var col = Entity.CollisionRect;
            var ent = level.CollidesWith(new Rectangle(
                                            x: col.X - ExtraHitAreaSize / 2,
                                            y: col.Y - ExtraHitAreaSize / 2,
                                            width: col.Width + ExtraHitAreaSize,
                                            height: col.Height + ExtraHitAreaSize))
                            .FirstOrDefault(e => e != Entity && (_canHit == null || _canHit(e)));

            if (ent != null)
                ent.Hit(Entity, gameTime, level, Entity.CurrentDirection);
        }

        public int ExtraHitAreaSize { get; set; }
    }
}
