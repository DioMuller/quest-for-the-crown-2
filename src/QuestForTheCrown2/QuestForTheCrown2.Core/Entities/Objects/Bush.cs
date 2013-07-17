using QuestForTheCrown2.Entities.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Objects
{
    class Bush : Destructible
    {
        public Bush()
            : base("sprites/Objects/bush.png", e => e is FireBall)
        {
        }
    }
}
