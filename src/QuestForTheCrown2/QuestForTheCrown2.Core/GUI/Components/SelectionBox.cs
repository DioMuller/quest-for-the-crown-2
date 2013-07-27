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
        /// <summary>
        /// Options
        /// </summary>
        private List<string> _options;

        /// <summary>
        /// Current Selection
        /// </summary>
        private int _current;

        private Texture2D _arrow;
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

        public string SelectedOption
        {
            get
            {
                if( _current == -1 ) return String.Empty;

                return _options[_current];
            }
        }
        #endregion Properties

        #region Constructor
        public SelectionBox(string label)
            : base(label)
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

            _options = new List<string>();
            _current = -1;

            _arrow = GameContent.LoadContent<Texture2D>("images/arrow.png");
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Draws the Component
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch, float transparency = 1.0f)
        {
            base.Draw(spriteBatch);

            #region Draw Label
            Color fontColor = Selected ? Color.Black : Color.White;
            Vector2 size = Font.MeasureString(Label);
            Vector2 fontPosition = new Vector2( Position.X + 20 , Position.Center.Y - size.Y/2);
            spriteBatch.DrawString(Font, Label, fontPosition, fontColor);
            #endregion Draw Label

            #region Draw Option
            Rectangle position = new Rectangle(Position.Center.X, Position.Y, Position.Width/2, Position.Height);

            if( _current != 0 ) spriteBatch.Draw( _arrow, new Rectangle( position.X, position.Y, 32, 32 ), fontColor);
            if (_current != _options.Count - 1) spriteBatch.Draw(_arrow, new Rectangle(position.X + position.Width - 32, position.Y, 32, 32), null, fontColor, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f );


            if(  _current >= 0 )
            {
                int textSize = Convert.ToInt32(Font.MeasureString( _options[_current] ).X);
                Vector2 textPosition = new Vector2(position.Center.X - (textSize / 2), position.Y);
                spriteBatch.DrawString(Font, _options[_current], textPosition, fontColor);
            }
            #endregion Draw Option
        }

        public void AddOption(string option)
        {
            _options.Add(option);

            if( _options.Count == 1 ) _current = 0;
        }

        public void SelectOption(string option)
        {
            _current = _options.IndexOf(option);
        }

        private void SelectPrevious()
        {
            if( _current > 0 )
            {
                _current--;
            }
        }

        private void SelectNext()
        {
            if( _current < _options.Count - 1 )
            {
                _current++;
            }
        }
        #endregion Methods
    }
}
