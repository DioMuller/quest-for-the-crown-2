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

        /// <summary>
        /// List component.
        /// </summary>
        private ScrollingList _list;
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
            _credits.Add("Students:");
            _credits.Add("    Diogo Muller de Miranda");
            _credits.Add("    João Vitor Pietsiaki Moraes");
            _credits.Add("");
            _credits.Add("Tecnicas de Implementacao de Jogos");
            _credits.Add("Teacher: Fabio Binder");
            _credits.Add("");
            _credits.Add("PROGRAMMING:");
            _credits.Add("      João Vitor Pietsiaki Moraes");
            _credits.Add("      Diogo Muller de Miranda");
            _credits.Add("");
            _credits.Add("ART:");
            _credits.Add("      Tilesets by David Gervais, licensed under Creative Commons 3.0");
            _credits.Add("      Website: http://pousse.rapiere.free.fr/tome/");
            _credits.Add("");
            _credits.Add("      Zombie and Skelleton - Reemax and artisticdude - http://opengameart.org/");
            _credits.Add("");
            _credits.Add("      Baldric (Main Character) and Mage - By Stephen 'Redshrike' Challener, ");
            _credits.Add("      design by Zi Ye - www.OpenGameArt.org");
            _credits.Add("");
            _credits.Add("      Slime and Slimeworm based on Slime by Xenoyia");
            _credits.Add("      http://gmc.yoyogames.com/index.php?showtopic=330208");
            _credits.Add("      Edited by Diogo Muller de Miranda.");
            _credits.Add("");
            _credits.Add("      Crab - Free for personal use");
            _credits.Add("      Found on: http://www.rpgmakervx.net/lofiversion/index.php/t28829.html");
            _credits.Add("");
            _credits.Add("      Bat by Diogo Muller de Miranda.");
            _credits.Add("");
            _credits.Add("      Goon (Male Orc)");
            _credits.Add("      http://opengameart.org/");
            _credits.Add("      Done by Matthew Krohn.");
            _credits.Add("      Walkcycle, Hurt, Slash, and Spellcast animations by Stephen Challener (AKA Redshrike), ");
            _credits.Add("      Shooting and Thrusting animations by Johannes Sjölund, Male and female orc heads by ");
            _credits.Add("      MadMarcel and based on Stephen Challener's Sinbad.");
            _credits.Add("      Master Goon edit based on this characted, made by Diogo Muller de Miranda.");
            _credits.Add("");
            _credits.Add("      Oldman:");
            _credits.Add("      Done by Tap, from OpenGameArt.Org");
            _credits.Add("");
            _credits.Add("      Water Dragon by Tana: ");
            _credits.Add("      http://www.rpgmakervx.net/index.php?showtopic=23956");
            _credits.Add("");
            _credits.Add("      Fire Dragon in Grayman Sprites:");
            _credits.Add("      http://kootation.com/grayman-sprites-rpg-maker-vx-community.html");
            _credits.Add("");
            _credits.Add("MUSIC:");
            _credits.Add("      All music by Kevin MacLeod");
            _credits.Add("      http://incompetech.com/");
            _credits.Add("");
            _credits.Add("      Brittle Rille");
            _credits.Add("      Call to Adventure");
            _credits.Add("      Easy Lemon");
            _credits.Add("      Five Armies");
            _credits.Add("      Heroic Age");
            _credits.Add("      Moonlight Hall");
            _credits.Add("      Rising Game");
            _credits.Add("      Suvaco do Cristo");
            _credits.Add("      Tenebrous Brothers Carnival - Act One");
            _credits.Add("      Tenebrous Brothers Carnival - Act Two");
            _credits.Add("      Brittle Rille");
            _credits.Add("");
            _credits.Add("SOUND EFFECTS:");
            _credits.Add("      Sword and Bow by Qat from http://www.freesound.org");
            _credits.Add("      All other sounds by Diogo Muller de Miranda.");
            _credits.Add("");
            _credits.Add("LEVEL DESIGN:");
            _credits.Add("      Diogo Muller de Miranda");
            _credits.Add("");
            _credits.Add("WEAPON DESIGN:");
            _credits.Add("      Diogo Muller de Miranda");
            _credits.Add("      João Vitor Pietsiaki Moraes");
            _credits.Add("");
            _credits.Add("ENEMY AND CHARACTER AI:");
            _credits.Add("      João Vitor Pietsiaki Moraes");
            _credits.Add("");
            _credits.Add("SPECIAL THANKS:");
            _credits.Add("      Enzo Augusto Marchiorato");
            _credits.Add("      Orlando Stein Junior");
            _credits.Add("      Luis Fernando Sobrejero Rigoni");
            _credits.Add("      Melanie Young Yee");
            _credits.Add("");
            _credits.Add("Made using MonoGame.");
            _credits.Add("http://www.monogame.net/");

            _list = new ScrollingList();

            _list.ComponentHeight = 50;

            foreach( string st in _credits )
            {
                _list.AddComponent(new Label(st));
            }
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
                _logoPosition = new Rectangle(Convert.ToInt32(window.Center.X - (_logo.Width / 2)), Convert.ToInt32(0.2f * (window.Height - window.Y)), _logo.Width, _logo.Height);

                _window = window;
            }

            spriteBatch.Draw(_logo, _logoPosition, Color.White);

            Vector2 creditsPosition = new Vector2(_logoPosition.X, _logoPosition.Y + _logoPosition.Height + 50);
            int y = _logoPosition.Y + _logoPosition.Height + 50;

            _list.Position = new Rectangle(_logoPosition.X, y, _logoPosition.Width, _window.Height - y - 20);
            _list.Draw(gameTime, spriteBatch);
            //foreach(string credit in _credits)
            //{
            //    spriteBatch.DrawString(_font, credit, creditsPosition, Color.White);
            //    creditsPosition.Y += 30;
            //}
        }
        #endregion Methods
    }
}
