using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Base
{
    public class EntityRelativePosition
    {
        public Entity Entity { get; set; }
        public Vector2 Position { get; set; }

        public Entity RelativeTo { get; set; }
        public float Distance { get; set; }
    }
}
