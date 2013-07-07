using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;

namespace QuestForTheCrown2.Entities.Objects
{
    /// <summary>
    /// Class used to represent an item pickup.
    /// </summary>
    public class Item : Entity
    {
        public Item() : base(@"sprites/Empty.png", new Point(32, 32))
        {

        }
    }
}
