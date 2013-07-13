using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.GUI.Components
{
    /// <summary>
    /// "Button"/Menu item component. Can be selected.
    /// </summary>
    public class Button : Component
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
        public Button(string text, SelectDelegate select) : base(text)
        {
            Font = GameContent.LoadContent<SpriteFont>("fonts/DefaultFont");
            Text = text;
            Select += select;
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Draws the Component
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Color fontColor = Selected ? Color.Black : Color.White;
            Vector2 size = Font.MeasureString(Text);
            Vector2 fontPosition = new Vector2( Position.Center.X - size.X/2 , Position.Center.Y - size.Y/2);

            spriteBatch.DrawString(Font, Text, fontPosition, fontColor);
        }
        #endregion Methods
    }
}
