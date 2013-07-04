using Microsoft.Xna.Framework;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Behaviors
{
    abstract class WalkBehavior : EntityUpdateBehavior
    {
        int _stoppedFrameCount;

        public WalkBehavior()
        {
            Group = "movement";
        }

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
    }
}
