using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.GUI.Screens
{
    class HowToPlay
    {
         #region Attributes
        /// <summary>
        /// Logo texture.
        /// </summary>
        private Texture2D _logo;

        /// <summary>
        /// Logo position.
        /// </summary>
        private Rectangle _logoPosition;


        /// <summary>
        /// Screen parent.
        /// </summary>
        private GameMain _parent;

        /// <summary>
        /// Input, to exit the screen.
        /// </summary>
        private Input _input;
        #endregion Attributes

        #region Constructor
        /// <summary>
        /// Title screen constructor.
        /// </summary>
        public HowToPlay(GameMain parent)
        {
            _logo = GameContent.LoadContent<Texture2D>("images/controls.png");

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

            _logoPosition = new Rectangle(Convert.ToInt32(window.Center.X - (_logo.Width / 2)), Convert.ToInt32(0.2f * (window.Height - window.Y)), _logo.Width, _logo.Height);

            spriteBatch.Draw(_logo, _logoPosition, Color.White);
        }
        #endregion Methods
    }
}
