#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

using QuestForTheCrown2.Levels.Mapping;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Characters;
using QuestForTheCrown2.Entities.Behaviors;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Weapons;
using QuestForTheCrown2.GUI.Screens;

namespace QuestForTheCrown2
{
    /// <summary>
    /// Game current state.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        Loading,
        Options,
        Credits,
        Quiting
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameMain : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        GameState _currentState;

        TitleScreen _mainMenu;
        CreditsScreen _credits;

        Levels.LevelCollection _overworld;

        Base.Input input = new Base.Input();

        public GameMain()
            : base()
        {
            _graphics = new GraphicsDeviceManager(this);

            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.PreferredBackBufferWidth = 1280;

            _currentState = GameState.MainMenu;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            GameContent.Initialize(Content);

            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _mainMenu = new TitleScreen(this);
            _credits = new CreditsScreen(this);

            _overworld = MapLoader.LoadLevels("Content/maps/QuestForTheCrown.maps");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (input.PauseButton)
                Exit();

            switch( _currentState )
            {
                case GameState.MainMenu:
                    _mainMenu.Update(gameTime);
                    break;
                case GameState.Playing:
                    _overworld.Update(gameTime);
                    break;
                case GameState.Loading:
                    break;
                case GameState.Options:
                    break;
                case GameState.Credits:
                    _credits.Update(gameTime);
                    break;
                case GameState.Quiting:
                    Exit();
                    break;
            }            
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            switch (_currentState)
            {
                case GameState.MainMenu:
                    _mainMenu.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.Playing:
                    _overworld.Draw(gameTime, _spriteBatch, Window.ClientBounds);
                    break;
                case GameState.Loading:
                    break;
                case GameState.Options:
                    break;
                case GameState.Credits:
                    _credits.Draw(gameTime, _spriteBatch);
                    break;
            }    
 
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Changes game state.
        /// </summary>
        /// <param name="state">Desired game state.</param>
        public void ChangeState(GameState state)
        {
            _currentState = state;
        }
    }
}
