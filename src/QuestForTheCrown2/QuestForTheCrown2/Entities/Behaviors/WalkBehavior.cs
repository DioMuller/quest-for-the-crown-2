using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using QuestForTheCrown2.Levels.Mapping;
using System;
using System.Linq;
using QuestForTheCrown2.Entities.Characters;

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
        protected bool Walk(GameTime gameTime, Level level, Vector2 direction)
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
                return true;
            }

            var timeFactor = gameTime.ElapsedGameTime.TotalMilliseconds / 1000;

            float newX = (float)(Entity.Position.X + direction.X * Entity.Speed.X * timeFactor);
            float newY = (float)(Entity.Position.Y + direction.Y * Entity.Speed.Y * timeFactor);
            Rectangle newRect = new Rectangle(
                    x: (int)newX + Entity.Padding.X,
                    y: (int)newY + Entity.Padding.Y,
                    width: Entity.Size.X - Entity.Padding.X - Entity.Padding.Width,
                    height: Entity.Size.Y - Entity.Padding.Y - Entity.Padding.Height);

            if (!level.Map.Collides(newRect) && !(level.CollidesWith(newRect).Any((e) => e != Entity && !e.OverlapEntities)))
            {
                Entity.Position = new Microsoft.Xna.Framework.Vector2(
                    x: newX,
                    y: newY);
                return true;
            }
            else if( Entity is Player && level.Map.IsOutsideBorders(newRect) )
            {
                Direction teleportDirection = Direction.None;

                if (newRect.X < 0) teleportDirection = Direction.West;
                else if (newRect.X + newRect.Width > level.Map.PixelSize.X) teleportDirection = Direction.East;
                else if (newRect.Y < 0) teleportDirection = Direction.North;
                else if (newRect.Y + newRect.Height > level.Map.PixelSize.Y) teleportDirection = Direction.South;

                level.GoToNeighbor(Entity as Player, teleportDirection);
            }
            return false;
        }

        protected void StopWalking(GameTime gameTime, Level level)
        {
            Entity.CurrentAnimation = "stopped";
            _stoppedFrameCount = 2;
        }
        #endregion
    }
}
