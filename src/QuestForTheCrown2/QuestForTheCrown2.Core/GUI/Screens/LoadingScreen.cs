using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.GUI.Screens
{
    class LoadingScreen
    {
        #region Attributes
        /// <summary>
        /// Screen parent.
        /// </summary>
        private GameMain _parent;

        /// <summary>
        /// Font used.
        /// </summary>
        private SpriteFont _font;

        /// <summary>
        /// Loading text.
        /// </summary>
        private string _text;
        #endregion Attributes

        #region Constructor
        /// <summary>
        /// Title screen constructor.
        /// </summary>
        public LoadingScreen(GameMain parent)
        {
            _font = GameContent.LoadContent<SpriteFont>("fonts/DefaultFont");

            _parent = parent;
            _text = "Loading";
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Updates component position.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public void Update(GameTime gameTime)
        {
            int pointCount = (gameTime.TotalGameTime.Milliseconds/333) % 3;

            _text = "Loading";

            int i;

            for (i = 0; i <= pointCount; i++)
            {
                _text += ".";
            }
            for( ; i <= 3; i++ )
            {
                _text += " ";
            }
        }

        /// <summary>
        /// Draws components and items.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="spriteBatch">Sprite batch.</param>
        /// <param name="window">Window position.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle window = _parent.Window.ClientBounds;
            Vector2 textSize = _font.MeasureString(_text);
            Vector2 textPosition = new Vector2(window.Width - window.X - textSize.X - 40, window.Height - window.Y - textSize.Y - 20 );

            spriteBatch.DrawString(_font, _text, textPosition, Color.White);
        }
        #endregion Methods
    }
}
