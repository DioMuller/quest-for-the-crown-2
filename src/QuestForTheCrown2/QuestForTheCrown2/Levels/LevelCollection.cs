using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Characters;

namespace QuestForTheCrown2.Levels
{
    public class LevelCollection
    {
        #region Attributes
        /// <summary>
        /// This world/dungeon levels.
        /// </summary>
        List<Level> _levels;
        
        /// <summary>
        /// Current level visited by the player.
        /// </summary>
        int _currentLevel;
        
        /// <summary>
        /// Dungeons on this world/dungeon.
        /// </summary>
        List<LevelCollection> _dungeons;

        /// <summary>
        /// Current dungeon.
        /// </summary>
        int _currentDungeon;
        #endregion Attributes

        #region Properties
        /// <summary>
        /// Current Level (May be changed to CurrentLevel array)
        /// </summary>
        private Level CurrentLevel
        {
            get
            {
                if (_currentDungeon == -1)
                {
                    return _levels[_currentLevel];
                }
                else
                {
                    return _dungeons[_currentDungeon].CurrentLevel;
                }
            }
        }

        private MainCharacter Player
        {
            get
            {
                return _levels[_currentLevel].Player;
            }
        }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Creates the level collection.
        /// </summary>
        public LevelCollection()
        {
            _levels = new List<Level>();
            _dungeons = new List<LevelCollection>();

            _currentLevel = 0;
            _currentDungeon = -1;
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Changes the current level for playerNum.
        /// </summary>
        /// <param name="playerNum">Player identifier.</param>
        /// <param name="direction">Direction to teleport.</param>
        private void GoToNeighbor(int playerNum, Direction direction)
        {
            int neighbor = CurrentLevel.GetNeighbor(direction);

            if( neighbor != 0 )
            {
                _currentLevel = neighbor - 1;
            }
        }

        #region Public Methods
        /// <summary>
        /// Updates the currently active levels and their entities.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public void Update(GameTime gameTime)
        {
            CurrentLevel.Update(gameTime);
        }

        /// <summary>
        /// Draws current active levels and their entities.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle clientBounds)
        {
            Vector2 camera = (Player != null) ? GetCameraPosition(Player, CurrentLevel.Map.PixelSize, clientBounds) : Vector2.Zero;
            CurrentLevel.Draw(gameTime, spriteBatch, camera);
        }

        /// <summary>
        /// Adds a level to the collection.
        /// </summary>
        /// <param name="level">Level to be added.</param>
        public bool AddLevel(Level level)
        {
            int count = (from Level lv in _levels where lv.Id == level.Id select lv).Count();

            if( count != 0 ) return false;

            _levels.Add(level);
            return true;
        }
        #endregion Public Methods

        #region Camera Methods
        static Vector2 GetCameraPosition(Entity entity, Point mapSize, Rectangle screenSize)
        {
            var camera = entity.Position;
            var newX = entity.Position.X;
            var newY = entity.Position.Y;

            if (newX < screenSize.Width / 2)
                newX = screenSize.Width / 2;
            else if (newX > mapSize.X - screenSize.Width / 2)
                newX = mapSize.X - screenSize.Width / 2;

            if (newY < screenSize.Height / 2)
                newY = screenSize.Height / 2;
            else if (newY > mapSize.Y - screenSize.Height / 2)
                newY = mapSize.Y - screenSize.Height / 2;

            camera = new Vector2(
                newX - screenSize.Width / 2,
                newY - screenSize.Height / 2);
            return camera;
        }
        #endregion Camera Methods

        #endregion Methods
    }
}
