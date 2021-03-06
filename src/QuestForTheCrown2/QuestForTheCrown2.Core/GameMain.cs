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
using QuestForTheCrown2.Levels;

namespace QuestForTheCrown2
{
    /// <summary>
    /// Game current state.
    /// </summary>
    public enum GameState
    {
        IntroScreen,
        MainMenu,
        Playing,
        LoadGame,
        Options,
        Credits,
        GameOver,
        Quiting,
        NewGame,
        LoadingGame,
        Saving,
        HowToPlay,
        DemoMode
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameMain : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        GameState _currentState;

        IntroScreen _introScreen;
        TitleScreen _mainMenu;
        CreditsScreen _credits;
        GameOverScreen _gameOver;
        OptionsScreen _options;
        LoadScreen _loadscreen;
        LoadingScreen _loadingScreen;
        SaveScreen _savescreen;
        HowToPlay _howToPlay;

        int _inputDelay;

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
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0/60.0);
            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
 
            _graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
#else
            _graphics.IsFullScreen = OptionsManager.CurrentOptions.Fullscreen;
            _graphics.PreferredBackBufferHeight = OptionsManager.CurrentOptions.ResolutionHeight;
            _graphics.PreferredBackBufferWidth = OptionsManager.CurrentOptions.ResolutionWidth;
#endif

            _currentState = GameState.IntroScreen;

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

            _introScreen = new IntroScreen(this);
            _mainMenu = new TitleScreen(this);
            _credits = new CreditsScreen(this);
            _gameOver = new GameOverScreen(this);
            _options = new OptionsScreen(this);
            _loadscreen = new LoadScreen(this);
            _savescreen = new SaveScreen(this);
            _loadingScreen = new LoadingScreen(this);
            _howToPlay = new HowToPlay(this);

            GameStateManager.Parent = this;

            //System.Threading.ThreadPool.QueueUserWorkItem(LoadContentAsync);
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
                case GameState.IntroScreen:
                    _introScreen.Update(gameTime);
                    break;
                case GameState.MainMenu:
                    SoundManager.PlayBGM("Call to Adventure");
                    _mainMenu.Update(gameTime);
                    break;
                case GameState.NewGame:
                case GameState.DemoMode:
                    if (_overworld == null || _overworld.Parent == null)
                    {
                        _loadingScreen.Update(gameTime);
                        break;
                    }

                    var player = _overworld.Players.First();
                    //GameStateManager.DeleteAllSaves();
                    LevelCollection.StoredWaypoints = new Dictionary<Entity, List<Waypoint>>
                    {
                        { player, new List<Waypoint>() }
                    };

                    if( _currentState == GameState.DemoMode )
                    {
                        GameStateManager.SelectSaveData(new Base.GameState
                        {
                            AllowWeapon = new List<string> { "Sword", "Bow", "Boomerang", "FireWand" },
                            DungeonsComplete = new List<string>(),
                            Player = GameStateManager.GetPlayerState(player),
                            StoredWaypoints = LevelCollection.StoredWaypoints[player]
                        });  
                        player.AddWeapon(new Sword(), new Bow(), new Boomerang(), new FireWand());
                    }
                    else
                    {
                        GameStateManager.SelectSaveData(new Base.GameState
                        {
                            AllowWeapon = new List<string> { "Sword" },
                            DungeonsComplete = new List<string>(),
                            Player = GameStateManager.GetPlayerState(player),
                            StoredWaypoints = LevelCollection.StoredWaypoints[player]
                        });
                    }

                    ChangeState(GameState.Playing);
                    break;
                case GameState.Playing:
                    if (_overworld != null)
                    {
                        _overworld.Update(gameTime);
                    }
                    break;
                case GameState.LoadGame:
                    _loadscreen.Update(gameTime);
                    break;
                case GameState.LoadingGame:
                    if (_overworld != null && _overworld.Parent != null)
                    {
                        var loadingPlayer = _overworld.Players.First();
                        var playerState = GameStateManager.CurrentState.Player;
                        var oldLevel = loadingPlayer.CurrentLevel;

                        GameStateManager.LoadPlayerState(loadingPlayer);
                        if (oldLevel != playerState.CurrentLevel)
                        {
                            _overworld.GetLevel(oldLevel).RemoveEntity(loadingPlayer);
                            _overworld.GetLevel(loadingPlayer.CurrentLevel).AddEntity(loadingPlayer);
                        }
                        LevelCollection.StoredWaypoints = new Dictionary<Entity, List<Waypoint>> { { loadingPlayer, GameStateManager.CurrentState.StoredWaypoints } };
                        ChangeState(GameState.Playing);
                    }
                    else
                    {
                        _loadingScreen.Update(gameTime);
                    }
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
                case GameState.HowToPlay:
                    _howToPlay.Update(gameTime);
                    break;
                case GameState.Quiting:
                    Exit();
                    break;
                case GameState.Saving:
                    _savescreen.Update(gameTime);
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

            if (_currentState != GameState.Playing)
                _spriteBatch.Begin();

            switch (_currentState)
            {
                case GameState.IntroScreen:
                    _introScreen.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.MainMenu:
                    _mainMenu.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.Playing:
                    if (_overworld != null)
                    {
                        _overworld.Draw(GraphicsDevice, gameTime, _spriteBatch, Window.ClientBounds);
                    }
                    break;
                case GameState.LoadGame:
                    _loadscreen.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.Options:
                    _options.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.Credits:
                    _credits.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.LoadingGame:
                case GameState.NewGame:
                    _loadingScreen.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.HowToPlay:
                    _howToPlay.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.GameOver:
                    _gameOver.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.Saving:
                    _savescreen.Draw(gameTime, _spriteBatch);
                    break;
            }

            if (_currentState != GameState.Playing)
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
            if (state == GameState.Saving)
            {
                GameStateManager.CurrentState.Player = GameStateManager.GetPlayerState(_overworld.Players.First());
                _savescreen = new SaveScreen(this);
            }
            if (state == GameState.LoadGame)
            {
                _loadscreen = new LoadScreen(this);
            }
            if (state == GameState.NewGame || state == GameState.LoadingGame || state == GameState.DemoMode)
            {
                _overworld = null;
                System.Threading.ThreadPool.QueueUserWorkItem(LoadContentAsync);
            }
        }
    }
}
