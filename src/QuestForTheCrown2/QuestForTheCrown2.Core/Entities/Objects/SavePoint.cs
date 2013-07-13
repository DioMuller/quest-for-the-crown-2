using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;

namespace QuestForTheCrown2.Entities.Objects
{
    /// <summary>
    /// Class used to represent a Save Point.
    /// </summary>
    public class SavePoint : Entity
    {
        public SavePoint()
            : base(@"sprites/Objects/Empty.png", new Point(32, 32))
        {
            OverlapEntities = true;
            Health = null;
            SpriteSheet.AddAnimation("stopped", "down", line: 0, count: 1, frameDuration: TimeSpan.FromDays(1)); 
        }
    }
}
