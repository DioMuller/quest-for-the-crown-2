using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Behaviors
{
    class ChainBehavior<T> : WalkBehavior where T : Entity
    {
        public Entity Following { get; private set; }
        public Entity FollowedBy { get; private set; }

        /// <summary>
        /// A desired distance to keep from the entity.
        /// </summary>
        public float Distance { get; set; }

        /// <summary>
        /// A maximum distance to start following.
        /// </summary>
        public float? MaxDistance { get; set; }

        public override bool IsActive(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            if (Entity.IsDead)
                return false;

            if (Following != null)
            {
                if(MaxDistance != null)
                {
                    var distance = new Vector2(Following.CenterPosition.X - Entity.CenterPosition.X, Following.CenterPosition.Y - Entity.CenterPosition.Y).Length();
                    if (distance > MaxDistance)
                    {
                        Following.GetBehavior<ChainBehavior<T>>().FollowedBy = null;
                        Following = null;
                    }
                }

                if (Following != null && (Following.IsDead || !level.ContainsEntity(Following)))
                {
                    Following.GetBehavior<ChainBehavior<T>>().FollowedBy = null;
                    Following = null;
                }
            }

            if (Following == null)
            {
                Following = (from ent in level.GetEntities<T>()
                              where ent != Entity && !ent.IsDead && level.ContainsEntity(ent)
                              let beh = ent.GetBehavior<ChainBehavior<T>>()
                              where beh != null && beh.FollowedBy == null && !beh.IsFollowingEntity(Entity)
                              let pos = new Vector2(Entity.CenterPosition.X - ent.CenterPosition.X,
                                                  Entity.CenterPosition.Y - ent.CenterPosition.Y)
                              let distance = pos.Length()
                              where MaxDistance == null || distance < MaxDistance
                              orderby distance
                              select ent).FirstOrDefault();
                if (Following != null)
                    Following.GetBehavior<ChainBehavior<T>>().FollowedBy = Entity;
            }

            return Following != null;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            var direction = new Vector2(
                Following.CenterPosition.X - Entity.CenterPosition.X,
                Following.CenterPosition.Y - Entity.CenterPosition.Y);

            var route = direction;
            var length = route.Length();

            if (length < Distance)
                route = Vector2.Zero;
            else if (route.Length() > 1)
                route.Normalize();

            Walk(gameTime, level, route, e =>
                {
                    if (e == Entity)
                        return false;

                    if (e.OverlapEntities)
                        return false;

                    if (!(e is T))
                        return true;

                    var beh = e.GetBehavior<ChainBehavior<T>>();
                    if (beh == null)
                        return true;

                    return IsFollowingEntity(e);
                });
        }

        /// <summary>
        /// Checks if this behavior is following an specified entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsFollowingEntity(Base.Entity entity)
        {
            var beh = this;

            while (beh != null && beh.Following != null && beh.Following != entity)
                beh = beh.Following.GetBehavior<ChainBehavior<T>>();

            return beh != null && beh.Following != null && beh.Following == entity;
        }
    }
}
