using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.GUI.Screens
{
    class CreditsScreen
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
        /// Credits list
        /// </summary>
        private List<string> _credits;

        /// <summary>
        /// Font used.
        /// </summary>
        private SpriteFont _font;
        #endregion Attributes

        #region Constructor
        /// <summary>
        /// Title screen constructor.
        /// </summary>
        public CreditsScreen(GameMain parent)
        {
            _logo = GameContent.LoadContent<Texture2D>("images/title.png");

            _font = GameContent.LoadContent<SpriteFont>("fonts/DefaultFont");

            _window = Rectangle.Empty;

            _parent = parent;

            _input = new Input();

            _credits = new List<string>();
            _credits.Add("Programmers:");
            _credits.Add("    Diogo Muller de Miranda");
            _credits.Add("    Joao Vitor Pietsiaki Moraes");
            _credits.Add("");
            _credits.Add("Feito para a matéria de Técnicas de Implementacao de Jogos");
            _credits.Add("Professor: Fabio Binder");
            _credits.Add("");
            _credits.Add("Art:");
            _credits.Add("    Tilesets by David Gervais, licensed under Creative Commons 3.0");
            _credits.Add("      Website: http://pousse.rapiere.free.fr/tome/");
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
                _logoPosition = new Rectangle(Convert.ToInt32(window.Center.X - (_logo.Width / 2)), Convert.ToInt32(0.2f * (window.Height - window.Y)), _logo.Width, _logo.Height);

                _window = window;
            }

            spriteBatch.Draw(_logo, _logoPosition, Color.White);

            Vector2 creditsPosition = new Vector2(_logoPosition.X, _logoPosition.Y + _logoPosition.Height + 50);

            foreach(string credit in _credits)
            {
                spriteBatch.DrawString(_font, credit, creditsPosition, Color.White);
                creditsPosition.Y += 30;
            }
        }
        #endregion Methods
    }
}
