using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.GUI.Screens
{
    class GameOverScreen
    {
        #region Attributes
        /// <summary>
        /// Game over text texture.
        /// </summary>
        private Texture2D _goimage;

        /// <summary>
        /// Game over text position.
        /// </summary>
        private Rectangle _goimagePosition;

        /// <summary>
        /// Current window.
        /// </summary>
        private Rectangle _window;

        /// <summary>
        /// Screen parent.
        /// </summary>
        private GameMain _parent;

        /// <summary>
        /// Input, to exit the screen.
        /// </summary>
        private Input _input;

        /// <summary>
        /// Font used.
        /// </summary>
        public SpriteFont _font { get; set; }
        #endregion Attributes

        #region Constructor
        /// <summary>
        /// Title screen constructor.
        /// </summary>
        public GameOverScreen(GameMain parent)
        {
            _goimage = GameContent.LoadContent<Texture2D>("images/gameover.png");

            _font = GameContent.LoadContent<SpriteFont>("fonts/DefaultFont");

            _window = Rectangle.Empty;

            _parent = parent;

            _input = new Input();
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Updates component position.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public void Update(GameTime gameTime)
        {
            if( _input.CancelButton )
            {
                _parent.ChangeState(GameState.MainMenu);
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

            if( window != _window )
            {
                _goimagePosition = new Rectangle(Convert.ToInt32(window.Center.X - (_goimage.Width / 2)), Convert.ToInt32(0.2f * (window.Height - window.Y)), _goimage.Width, _goimage.Height);

                _window = window;
            }

            spriteBatch.Draw(_goimage, _goimagePosition, Color.White);

            

            PrintString(spriteBatch, "You were defeated. Evil has won.", window, _goimagePosition.Y + _goimagePosition.Height + 50);
            PrintString(spriteBatch, "Press CANCEL to return to the menu.", window, _goimagePosition.Y + _goimagePosition.Height + 100);
        }

        private void PrintString(SpriteBatch spriteBatch, string text, Rectangle window, int Y)
        {
            Vector2 size = _font.MeasureString(text);
            Vector2 textPosition = new Vector2(window.Center.X - (size.X/2), Y);
            spriteBatch.DrawString( _font, text, textPosition, Color.Red );
        }
        #endregion Methods
    }
}
