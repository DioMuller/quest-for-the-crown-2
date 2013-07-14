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
        EntityRelativePosition _following;
        Entity _followedBy;

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
            if (_following != null)
            {
                if(MaxDistance != null)
                {
                    var distance = new Vector2(_following.Entity.CenterPosition.X - Entity.CenterPosition.X, _following.Entity.CenterPosition.Y - Entity.CenterPosition.Y).Length();
                    if (distance > MaxDistance)
                    {
                        _following.Entity.GetBehavior<ChainBehavior<T>>()._followedBy = null;
                        _following = null;
                    }
                }

                if (_following != null && (_following.Entity.IsDead || !level.ContainsEntity(_following.Entity)))
                {
                    _following.Entity.GetBehavior<ChainBehavior<T>>()._followedBy = null;
                    _following = null;
                }
            }

            if (_following == null)
            {
                _following = (from ent in level.GetEntities<T>()
                              where ent != Entity && !ent.IsDead && level.ContainsEntity(ent)
                              let beh = ent.GetBehavior<ChainBehavior<T>>()
                              where beh != null && beh._followedBy == null && !beh.IsFollowing(Entity)
                              let pos = new Vector2(Entity.CenterPosition.X - ent.CenterPosition.X,
                                                  Entity.CenterPosition.Y - ent.CenterPosition.Y)
                              let distance = pos.Length()
                              where MaxDistance == null || distance < MaxDistance
                              orderby distance
                              select new EntityRelativePosition
                              {
                                  RelativeTo = Entity,
                                  Entity = ent,
                                  Position = pos,
                                  Distance = distance
                              }).FirstOrDefault();
                if (_following != null)
                    _following.Entity.GetBehavior<ChainBehavior<T>>()._followedBy = Entity;
            }

            return _following != null;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
            var direction = new Vector2(
                _following.Entity.CenterPosition.X - Entity.CenterPosition.X,
                _following.Entity.CenterPosition.Y - Entity.CenterPosition.Y);

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

                    return IsFollowing(e);
                });
        }

        /// <summary>
        /// Checks if this behavior is following an specified entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsFollowing(Base.Entity entity)
        {
            var beh = this;

            while (beh != null && beh._following != null && beh._following.Entity != entity)
                beh = beh._following.Entity.GetBehavior<ChainBehavior<T>>();

            return beh != null && beh._following != null && beh._following.Entity == entity;
        }
    }
}
