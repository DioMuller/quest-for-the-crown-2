using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels.Mapping;
using System;

namespace QuestForTheCrown2.Entities.Behaviors
{
    /// <summary>
    /// Base behavior for walking.
    /// Contains helper methods to make one entity walk based on the GameTime.
    /// </summary>
    abstract class WalkBehavior : EntityUpdateBehavior
    {
        #region Attributes
        int _stoppedFrameCount;
        #endregion

        #region Constructors
        protected WalkBehavior()
        {
            Group = "movement";
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Make the Entity walk into the desired direction.
        /// The entity will use its animations accordingly.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="map">Current entity map.</param>
        /// <param name="direction">Desired walk direction.</param>
        protected void Walk(GameTime gameTime, Map map, Vector2 direction)
        {
            if (Math.Abs(direction.X) >= Math.Abs(direction.Y))
            {
                if (direction.X > 0)
                    Entity.CurrentView = "right";
                else if (direction.X < 0)
                    Entity.CurrentView = "left";
            }
            else
            {
                if (direction.Y < 0)
                    Entity.CurrentView = "up";
                else if (direction.Y > 0)
                    Entity.CurrentView = "down";
            }

            if (direction.X != 0 || direction.Y != 0)
            {
                Entity.CurrentAnimation = "walking";
                _stoppedFrameCount = 0;
            }
            else
            {
                if (_stoppedFrameCount++ > 1)
                    Entity.CurrentAnimation = "stopped";
                return;
            }

            var timeFactor = gameTime.ElapsedGameTime.TotalMilliseconds / 1000;

            float newX = (float)(Entity.Position.X + direction.X * Entity.Speed.X * timeFactor);
            float newY = (float)(Entity.Position.Y + direction.Y * Entity.Speed.Y * timeFactor);

            if (!map.Collides(new Microsoft.Xna.Framework.Rectangle((int)newX, (int)newY, Entity.Size.X, Entity.Size.Y)))
            {
                Entity.Position = new Microsoft.Xna.Framework.Vector2(
                    x: newX,
                    y: newY);
            }
        }
        #endregion
    }
}
