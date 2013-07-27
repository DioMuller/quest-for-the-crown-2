using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.GUI.Components
{
    class Label : Component
    {
         #region Properties
        /// <summary>
        /// Label.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Font used.
        /// </summary>
        public SpriteFont Font { get; set; }
        #endregion Properties

        #region Constructor
        public Label(string text)
            : base(text)
        {
            Font = GameContent.LoadContent<SpriteFont>("fonts/DefaultFont");
            Text = text;
            Select = null;
            Selectable = false;
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Draws the Component
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch, float transparency = 1.0f)
        {
            base.Draw(spriteBatch, transparency);
            Color fontColor = Color.White;
            Vector2 size = Font.MeasureString(Text);
            Vector2 fontPosition = new Vector2( Position.X , Position.Center.Y - size.Y/2);

            spriteBatch.DrawString(Font, Text, fontPosition, fontColor * transparency);
        }
        #endregion Methods
    }
}
