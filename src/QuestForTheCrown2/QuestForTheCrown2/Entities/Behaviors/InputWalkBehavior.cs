using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Behaviors
{
    class InputWalkBehavior : EntityUpdateBehavior
    {
        Input _input;

        public InputWalkBehavior(Input input)
        {
            Group = "movement";
            _input = input;
        }

        public override bool Active
        {
            get { return true; }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (_input.Movement.X > 0)
                Entity.CurrentView = "right";
            else if (_input.Movement.X < 0)
                Entity.CurrentView = "left";
            else if(_input.Movement.Y < 0)
                Entity.CurrentView = "up";
            else if (_input.Movement.Y > 0)
                Entity.CurrentView = "down";

            if (_input.Movement.X != 0 || _input.Movement.Y != 0)
                Entity.CurrentAnimation = "walking";
            else
                Entity.CurrentAnimation = "stopped";

            var timeFactor = gameTime.ElapsedGameTime.TotalMilliseconds / 1000;

            Entity.Position = new Microsoft.Xna.Framework.Vector2(
                x: (float)(Entity.Position.X + _input.Movement.X * Entity.Speed.X * timeFactor),
                y: (float)(Entity.Position.Y + _input.Movement.Y * Entity.Speed.Y * timeFactor));
        }
    }
}
