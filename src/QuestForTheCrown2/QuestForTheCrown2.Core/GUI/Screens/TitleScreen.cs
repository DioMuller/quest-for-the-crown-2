using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.GUI.Components;

namespace QuestForTheCrown2.GUI.Screens
{
    public class TitleScreen
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
        /// List component.
        /// </summary>
        private ComponentList _list;

        private GameMain _parent;
        #endregion Attributes

        #region Constructor
        /// <summary>
        /// Title screen constructor.
        /// </summary>
        public TitleScreen(GameMain parent)
        {
            _logo = GameContent.LoadContent<Texture2D>("images/title.png");

            _list = new ComponentList();

            _list.ComponentHeight = 50;

            _list.AddComponent(new Button("New Game", () => _parent.ChangeState(GameState.NewGame)));
            _list.AddComponent(new Button("Load Game", () => _parent.ChangeState(GameState.LoadGame)));
            #if DEBUG
            _list.AddComponent(new Button("Demo Mode", () => _parent.ChangeState(GameState.DemoMode)));
            #endif
            _list.AddComponent(new Button("Options", () => _parent.ChangeState(GameState.Options)));
            _list.AddComponent(new Button("How to Play", () => _parent.ChangeState(GameState.HowToPlay)));
            _list.AddComponent(new Button("Credits", () => _parent.ChangeState(GameState.Credits)));
            _list.AddComponent(new Button("Quit Game", () => _parent.ChangeState(GameState.Quiting)));

            _window = Rectangle.Empty;

            _parent = parent;
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Updates component position.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public void Update(GameTime gameTime)
        {
            _list.Update(gameTime);
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

            if (window != _window)
            {
                _logoPosition = new Rectangle(Convert.ToInt32(window.Center.X - (_logo.Width / 2)), Convert.ToInt32(0.2f * (window.Height - window.Y)), _logo.Width, _logo.Height);
                _list.Position = new Rectangle(window.Center.X - 150, Convert.ToInt32(0.4f * (window.Height - window.Y)), 300, 360);

                _window = window;
            }

            spriteBatch.Draw(_logo, _logoPosition, Color.White);
            _list.Draw(gameTime, spriteBatch);
        }
        #endregion Methods
    }
}
