using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuestForTheCrown2.GUI.Components
{
    public class MenuItem : Component
    {
        public string Value { get; set; }

        public MenuItem(string value) : base()
        {
        }

        /// <summary>
        /// Draws the Component
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
