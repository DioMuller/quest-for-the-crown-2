﻿using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using QuestForTheCrown2.Levels.Mapping;

namespace QuestForTheCrown2.Entities.Behaviors
{
    /// <summary>
    /// A behavior that moves the entity in the direction of another entity.
    /// </summary>
    class AvoidBehavior : WalkBehavior
    {
        #region Properties
        public EntityRelativePosition CurrentTarget { get; private set; }

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
        public AvoidBehavior(string targetCategory, float distance = 32 * 2)
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
            CurrentTarget = level.EntityCloserTo(Entity, TargetCategory);
            if (CurrentTarget != null && CurrentTarget.Distance < Distance)
                return true;

            Walk(gameTime, level, Vector2.Zero);
            return false;
        }

        /// <summary>
        /// Moves the entity into the direction of the currently Following entity.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="map">Current entity map.</param>
        public override void Update(GameTime gameTime, Level level)
        {
            var direction = new Vector2(
                 Entity.CenterPosition.X - CurrentTarget.Entity.CenterPosition.X,
                Entity.CenterPosition.Y - CurrentTarget.Entity.CenterPosition.Y);

            var route = direction;
            var length = route.Length();

            if (route.Length() > 1)
                route.Normalize();

            Walk(gameTime, level, route);
        }
        #endregion
    }
}
