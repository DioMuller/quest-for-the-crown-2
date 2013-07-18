using Microsoft.Xna.Framework;
using System;

namespace QuestForTheCrown2.Entities.Base
{
    class EntitySavedPosition
    {
        public TimeSpan TotalGameTime { get; set; }
        public Vector2 Position { get; set; }
        public int Level { get; set; }
    }
}
