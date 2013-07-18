using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Characters;
using QuestForTheCrown2.GUI.Components;

namespace QuestForTheCrown2.GUI.Screens
{
    public class SaveScreen
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
        public SaveScreen(GameMain parent)
        {
            _window = Rectangle.Empty;
            _parent = parent;

            _list = new ComponentList();

            _list.AddComponent(new Button("Save Game", new SelectDelegate(() =>
           {
                GameStateManager.SaveData(0);
                _parent.ChangeState(GameState.Playing);
            })));

            _list.AddComponent(new Button("Cancel", new SelectDelegate(() =>
            {
                OptionsManager.LoadOptions();
                _parent.ChangeState(GameState.Playing);
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
                _list.Position = new Rectangle(30, window.Center.Y-200, window.Width - 60, 400);

                _window = window;
            }

            _list.Draw(gameTime, spriteBatch);
        }
        #endregion Methods
    }
}
