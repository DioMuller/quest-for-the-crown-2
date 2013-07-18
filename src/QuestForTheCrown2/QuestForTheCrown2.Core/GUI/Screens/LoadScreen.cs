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
    public class LoadScreen
    {
        #region Attributes
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
        public LoadScreen(GameMain parent)
        {
            _window = Rectangle.Empty;
            _parent = parent;
            int x = 0;

            _list = new ComponentList();
            _list.ComponentHeight = 80;

            foreach( Base.GameState state in GameStateManager.LoadData() )
            {
                
                //TODO: Create specific component.
                _list.AddComponent(new SaveDataButton("saveButton" + (x++), state, new SelectDelegate(() =>
                {
                    GameStateManager.SelectSaveData(state);
                    _parent.ChangeState(GameState.LoadingGame);
                })));
            }

            _list.AddComponent(new Button("Cancel", new SelectDelegate(() =>
            {
                OptionsManager.LoadOptions();
                _parent.ChangeState(GameState.MainMenu);
            })));
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

            if( window != _window )
            {
                _list.Position = new Rectangle(30, 100, window.Width - 60, window.Height - 200);

                _window = window;
            }

            _list.Draw(gameTime, spriteBatch);
        }
        #endregion Methods
    }
}
