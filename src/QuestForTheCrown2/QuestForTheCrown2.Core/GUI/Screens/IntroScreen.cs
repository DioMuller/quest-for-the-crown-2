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
    class IntroScreen
    {
        #region Attributes
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

        /// <summary>
        /// List component.
        /// </summary>
        private ScrollingList _list;
        #endregion Attributes

        #region Constructor
        /// <summary>
        /// Title screen constructor.
        /// </summary>
        public IntroScreen(GameMain parent)
        {
            _font = GameContent.LoadContent<SpriteFont>("fonts/DefaultFont");

            _parent = parent;

            _input = new Input();

            _credits = new List<string>();
            _credits.Add("A long time ago, an distant kingdom lived in peace");
            _credits.Add("thanks to a magic crown. The peace was kept until ");
            _credits.Add("one day, when a Evil Wizard stole the crown.");
            _credits.Add("The Prince couldn't let the worst happen,");
            _credits.Add("and saved the day, killing the villain.");
            _credits.Add("");
            _credits.Add("Two hundred years have passed since then.");
            _credits.Add("A group of cultists stole the crown and");
            _credits.Add("captured the king, so they could");
            _credits.Add("get enough power to ressurrect the wizard.");
            _credits.Add("");
            _credits.Add("Now, all of the kingdom's hopes rest on");
            _credits.Add("the shoulders of the king's bodyguard.");
            _credits.Add("He needs to get the sacred weapons to");
            _credits.Add("destroy the evil cult, saving the king,");
            _credits.Add("the kingdom and the crown.");
            _credits.Add("");
            _credits.Add("But first... he would need a sword.");


            _list = new ScrollingList();

            _list.ComponentHeight = 50;

            foreach( string st in _credits )
            {
                _list.AddComponent(new Label(st));
            }

            _list.Position = new Rectangle(20, 20, parent.Window.ClientBounds.Width - 40, parent.Window.ClientBounds.Height - 40);
            _list.ResetPosition();
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Updates component position.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public void Update(GameTime gameTime)
        {
            SoundManager.PlayBGM("Rising Game");

            if( _input.CancelButton || _input.ConfirmButton || _input.PauseButton )
            {
                _parent.ChangeState(GameState.MainMenu);
            }

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

            _list.Position = new Rectangle(20, 20, window.Width - 40, window.Height - 40);
            _list.Draw(gameTime, spriteBatch);
        }
        #endregion Methods
    }
}
