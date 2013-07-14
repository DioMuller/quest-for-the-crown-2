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
            : base(@"sprites/Objects/SavePoint.png", new Point(32, 32))
        {
            OverlapEntities = true;
            Health = null;
            SpriteSheet.AddAnimation("stopped", "down", line: 0, frameDuration: TimeSpan.FromMilliseconds(33)); 
        }
    }
}
