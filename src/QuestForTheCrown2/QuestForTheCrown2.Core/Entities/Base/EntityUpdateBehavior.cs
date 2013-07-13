using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestForTheCrown2.Levels.Mapping;
using QuestForTheCrown2.Levels;

namespace QuestForTheCrown2.Entities.Base
{
    /// <summary>
    /// An update method, that can be attached to entities.
    /// </summary>
    public abstract class EntityUpdateBehavior
    {
        /// <summary>
        /// The group in which this update operates.
        /// Only one update per group is executed (except for an empty group).
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// The entity in which this behavior is attached and will operate on.
        /// </summary>
        public Entity Entity { get; set; }

        /// <summary>
        /// Checks if the current behavior is active.
        /// </summary>
        public abstract bool IsActive(GameTime gameTime, Level level);

        /// <summary>
        /// Executes an update logic on the attached entity.
        /// </summary>
        /// <param name="deltaTime"></param>
        public abstract void Update(GameTime gameTime, Level level);

        public virtual void Deactivated(Microsoft.Xna.Framework.GameTime gameTime, Levels.Level level)
        {
        }
    }
}
