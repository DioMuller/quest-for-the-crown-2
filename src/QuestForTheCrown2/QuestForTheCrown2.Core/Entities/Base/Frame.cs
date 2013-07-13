using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Base
{
    /// <summary>
    /// Represents a single image inside a sprite sheet.
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// The sprite sheet's texture.
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// The frame's position inside the sprite sheet.
        /// </summary>
        public Rectangle Rectangle { get; set; }
    }
}
