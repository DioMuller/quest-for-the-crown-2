﻿#region Using Statements
using System;
using System.Linq;
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
        GameOver,
        Quiting,
        NewGame
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
        GameOverScreen _gameOver;
        OptionsScreen _options;

        Levels.LevelCollection _overworld;

        Base.Input input = new Base.Input();

        public GameMain()
            : base()
        {
            _graphics = new GraphicsDeviceManager(this);

            OptionsManager.LoadOptions();

            #if OUYA
            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            // 30 FPS
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0/30.0);
            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
 
            _graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
#else
            _graphics.IsFullScreen = OptionsManager.CurrentOptions.Fullscreen;
            _graphics.PreferredBackBufferHeight = OptionsManager.CurrentOptions.ResolutionHeight;
            _graphics.PreferredBackBufferWidth = OptionsManager.CurrentOptions.ResolutionWidth;

            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0 / 30.0);
            #endif

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
            _gameOver = new GameOverScreen(this);
            _options = new OptionsScreen(this);

            System.Threading.ThreadPool.QueueUserWorkItem(LoadContentAsync);
        }

        private void LoadContentAsync(object asyncState)
        {
            _overworld = MapLoader.LoadLevels("Content/maps/QuestForTheCrown.maps");
            _overworld.Parent = this;
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
            switch (_currentState)
            {
                case GameState.MainMenu:
                    SoundManager.PlayBGM("Call to Adventure");
                    _mainMenu.Update(gameTime);
                    break;
                case GameState.NewGame:
                    var player = _overworld.Players.First();
                    GameStateManager.DeleteAllSaves();
                    GameStateManager.SelectSaveData(new Base.GameState
                    {
                        AllowWeapon = new List<string> { "Sword" },
                        DungeonsComplete = new List<string>(),
                        Player = GameStateManager.GetPlayerState(player)
                    });
                    ChangeState(GameState.Playing);
                    break;
                case GameState.Playing:
                    if( _overworld != null ) 
                    {
                        _overworld.Update(gameTime);
                    }
                    break;
                case GameState.Loading:
                    if (GameStateManager.AllStates.Any())
                    {
                        GameStateManager.SelectSaveData(GameStateManager.AllStates.OrderByDescending(s => s.LastPlayDate).First());
                        GameStateManager.LoadPlayerState(_overworld.Players.First());
                        ChangeState(GameState.Playing);
                    }
                    else
                        ChangeState(GameState.NewGame);
                    break;
                case GameState.Options:
                    _options.Update(gameTime);
                    break;
                case GameState.Credits:
                    _credits.Update(gameTime);
                    break;
                case GameState.GameOver:
                    _gameOver.Update(gameTime);
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
                    if( _overworld != null )
                    {
                        _overworld.Draw(gameTime, _spriteBatch, Window.ClientBounds);
                    }
                    break;
                case GameState.Loading:
                    break;
                case GameState.Options:
                    _options.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.Credits:
                    _credits.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.GameOver:
                    _gameOver.Draw(gameTime, _spriteBatch);
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

            if (state == GameState.Playing)
            {
                _overworld = MapLoader.LoadLevels("Content/maps/QuestForTheCrown.maps");
                _overworld.Parent = this;
            }
        }
    }
}
