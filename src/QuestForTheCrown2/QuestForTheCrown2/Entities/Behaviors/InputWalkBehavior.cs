using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Behaviors
{
    class InputWalkBehavior : WalkBehavior
    {
        Input _input;

        public InputWalkBehavior(InputType input, int index = 0)
        {
            Group = "movement";
            _input = new Input(input, index);
        }

        public override bool Active
        {
            get { return _input.IsConnected; }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Levels.Mapping.Map map)
        {
            Walk(gameTime, map, _input.Movement);
        }
    }
}
