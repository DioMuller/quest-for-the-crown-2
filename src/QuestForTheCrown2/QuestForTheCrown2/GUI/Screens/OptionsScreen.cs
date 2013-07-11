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
    public class OptionsScreen
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
        public OptionsScreen(GameMain parent)
        {
            _list = new ComponentList();

            SelectionBox resolution = new SelectionBox("Resolution");
            resolution.AddOption("800x600");
            resolution.AddOption("1280x720");
            resolution.AddOption("1960x1080");
            resolution.SelectOption(OptionsManager.CurrentOptions.ResolutionWidth + "x" + OptionsManager.CurrentOptions.ResolutionHeight);
            _list.AddComponent(resolution);

            SelectionBox fullscreen = new SelectionBox("Fullscreen");
            fullscreen.AddOption("On");
            fullscreen.AddOption("Off");
            fullscreen.SelectOption(OptionsManager.CurrentOptions.Fullscreen ? "On" : "Off");
            _list.AddComponent(fullscreen);

            SelectionBox inverted = new SelectionBox("Inverted Aim");
            inverted.AddOption("On");
            inverted.AddOption("Off");
            inverted.SelectOption(OptionsManager.CurrentOptions.InvertAim ? "On" : "Off");
            _list.AddComponent(inverted);

            _list.AddComponent(new Button("Save", new SelectDelegate(() =>
            {
                OptionsManager.SaveOptions();
                _parent.ChangeState(GameState.MainMenu);
            })));

            _list.AddComponent(new Button("Cancel", new SelectDelegate(() =>
            {
                OptionsManager.LoadOptions();
                _parent.ChangeState(GameState.MainMenu);
            })));

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

            if( window != _window )
            {
                _list.Position = new Rectangle(30, 200, window.Width - 60, 500);

                _window = window;
            }

            _list.Draw(gameTime, spriteBatch);
        }
        #endregion Methods
    }
}
