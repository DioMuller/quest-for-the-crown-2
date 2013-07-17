using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using QuestForTheCrown2.Levels.Mapping;
using System;
using System.Linq;
using QuestForTheCrown2.Entities.Characters;
using QuestForTheCrown2.Entities.Weapons;

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
        protected bool Walk(GameTime gameTime, Level level, Vector2 direction, Func<Entity, bool> collisionCheck = null)
        {
            if (collisionCheck == null)
                collisionCheck = e => e != Entity && !e.OverlapEntities;

            Entity.Look(direction, updateDirection: true);
            UpdateAnimation(direction);

            if (Entity.CurrentAnimation == "stopped")
                return true;

            var numArrows = level.FindEntities(e => e is Arrow && ((Arrow)e).HitEntity == Entity).Count() + 1;

            var timeFactor = gameTime.ElapsedGameTime.TotalMilliseconds / 1000;

            float newX = (float)(Entity.Position.X + direction.X * Entity.Speed / numArrows * timeFactor);
            float newY = (float)(Entity.Position.Y + direction.Y * Entity.Speed / numArrows * timeFactor);
            Rectangle newRect = new Rectangle(
                    x: (int)newX + Entity.Padding.X,
                    y: (int)newY + Entity.Padding.Y,
                    width: Entity.Size.X - Entity.Padding.X - Entity.Padding.Width,
                    height: Entity.Size.Y - Entity.Padding.Y - Entity.Padding.Height);

            if (!level.Map.Collides(newRect) && !(level.CollidesWith(newRect).Any(collisionCheck)))
            {
                Entity.Position = new Vector2(newX, newY);
                return true;
            }
            else if (Entity.Category == "Player" && level.Map.IsOutsideBorders(newRect))
            {
                Direction teleportDirection = Direction.None;

                if (newRect.X < 0) teleportDirection = Direction.West;
                else if (newRect.X + newRect.Width > level.Map.PixelSize.X) teleportDirection = Direction.East;
                else if (newRect.Y < 0) teleportDirection = Direction.North;
                else if (newRect.Y + newRect.Height > level.Map.PixelSize.Y) teleportDirection = Direction.South;

                level.GoToNeighbor(Entity, teleportDirection);
            }
            return false;
        }

        private void UpdateAnimation(Vector2 direction)
        {
            if (direction.X != 0 || direction.Y != 0)
            {
                Entity.CurrentAnimation = "walking";
                _stoppedFrameCount = 0;
            }
            else
            {
                if (_stoppedFrameCount++ > 1)
                    Entity.CurrentAnimation = "stopped";
            }
        }

        protected void StopWalking(GameTime gameTime, Level level)
        {
            Entity.CurrentAnimation = "stopped";
            _stoppedFrameCount = 2;
        }
        #endregion
    }
}
