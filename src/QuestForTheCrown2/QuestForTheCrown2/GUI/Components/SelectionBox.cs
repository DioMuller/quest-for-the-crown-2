using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.GUI.Components
{
    class SelectionBox : Component
    {
        #region Attributes
        private List<string> _options { get; set; }
        #endregion Attributes

        #region Properties
        /// <summary>
        /// Label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Font used.
        /// </summary>
        public SpriteFont Font { get; set; }
        #endregion Properties

        #region Constructor
        public SelectionBox(string label)
            : base()
        {
            Font = GameContent.LoadContent<SpriteFont>("fonts/DefaultFont");
            Label = label;
            SelectionChanged += new SelectionChangeDelegate((value) =>
            {
                if (value < -0.8)
                {
                    SelectPrevious();
                }
                else if (value > 0.8)
                {
                    SelectNext();
                }
            });
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
            Vector2 size = Font.MeasureString(Label);
            Vector2 fontPosition = new Vector2( Position.Center.X - size.X/2 , Position.Center.Y - size.Y/2);

            spriteBatch.DrawString(Font, Label, fontPosition, fontColor);
        }
        #endregion Methods
    }
}
