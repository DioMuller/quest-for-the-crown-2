﻿using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using QuestForTheCrown2.Levels.Mapping;

namespace QuestForTheCrown2.Entities.Behaviors
{
    /// <summary>
    /// A behavior that moves the entity in the direction of another entity.
    /// </summary>
    class FollowBehavior : WalkBehavior
    {
        #region Attributes
        Entity _following;
        #endregion

        #region Properties
        /// <summary>
        /// A desired distance to keep from the entity.
        /// </summary>
        public float Distance { get; set; }
        /// <summary>
        /// The entity category to be followed.
        /// </summary>
        public string TargetCategory { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the follow behavior with the desired follow entity and the distance to keep.
        /// </summary>
        /// <param name="following">The entity to follow.</param>
        /// <param name="distance">The desired distance to keep.</param>
        public FollowBehavior(string targetCategory, float distance = 64)
        {
            TargetCategory = targetCategory;
            Distance = distance;
        }
        #endregion

        #region Behavior
        /// <summary>
        /// Indicates if this behavior is set to follow one Entity.
        /// </summary>
        public override bool IsActive(GameTime gameTime, Level level)
        {
            _following = level.EntityCloserTo(Entity, TargetCategory);
            return _following != null;
        }

        /// <summary>
        /// Moves the entity into the direction of the currently Following entity.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="map">Current entity map.</param>
        public override void Update(GameTime gameTime, Level level)
        {
            var direction = new Vector2(
                _following.Position.X - Entity.Position.X,
                _following.Position.Y - Entity.Position.Y);

            var route = direction;
            var length = route.Length();

            if (length < Distance)
                route = Vector2.Zero;
            else if (route.Length() > 1)
                route.Normalize();

            Walk(gameTime, level, route);
        }
        #endregion
    }
}
