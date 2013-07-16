using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.GUI.GameGUI
{
    public class TitleCard
    {
        #region Constants
        /// <summary>
        /// Show time.
        /// </summary>
        public const int ShowTime = 2000;
        /// <summary>
        /// Fade time.
        /// </summary>
        public const int FadeTime = 2000;
        #endregion Constants

        #region Attributes
        /// <summary>
        /// Current Title.
        /// </summary>
        private string _currentTitle;

        /// <summary>
        /// Time where the card will be shown.
        /// </summary>
        private int _showTime;

        /// <summary>
        /// Time where the card will be fading.
        /// </summary>
        private int _fadeTime;

        /// <summary>
        /// Font used.
        /// </summary>
        private SpriteFont _font;
        #endregion Attributes

        #region Properties
        /// <summary>
        /// Current Title.
        /// </summary>
        public string CurrentTitle
        {
            get
            {
                return _currentTitle;
            }
            set
            {
                if( value != _currentTitle )
                {
                    _showTime = ShowTime;
                    _fadeTime = FadeTime;
                    _currentTitle = value;
                }
            }
        }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Creates title card with default values.
        /// </summary>
        public TitleCard()
        {
            _showTime = 0;
            _fadeTime = 0;
            _currentTitle = String.Empty;

            _font = GameContent.LoadContent<SpriteFont>("fonts/TitleFont");
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Draws the Title Card.
        /// </summary>
        /// <param name="gameTime">Game Time.</param>
        /// <param name="spriteBatch">Sprite batch.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle window)
        {
            if( _showTime <= 0 && _fadeTime <= 0 ) return; //Returns here so it won't do the rectangle calculation.
            
            Vector2 size = _font.MeasureString(_currentTitle);
            Vector2 place = new Vector2(window.Center.X - size.X/2, window.Center.Y - size.Y/2);

            if( _showTime > 0 )
            {
                spriteBatch.DrawString(_font, _currentTitle, place, Color.White);
                _showTime -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else if( _fadeTime > 0 )
            {
                spriteBatch.DrawString(_font, _currentTitle, place, Color.White * ((float)_fadeTime / (float)FadeTime) );
                _fadeTime -= gameTime.ElapsedGameTime.Milliseconds;
            }
        }
        #endregion Methods
    }
}
