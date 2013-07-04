using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        Level _currentLevel;
        
        /// <summary>
        /// Dungeons on this world/dungeon.
        /// </summary>
        List<LevelCollection> _dungeons;

        /// <summary>
        /// Current dungeon.
        /// </summary>
        LevelCollection _currentDungeon;
        #endregion Attributes

        #region Properties
        /// <summary>
        /// Current Level (May be changed to CurrentLevel array)
        /// </summary>
        private Level CurrentLevel
        {
            get
            {
                if (_currentDungeon == null)
                {
                    return _currentLevel;
                }
                else
                {
                    return _currentDungeon.CurrentLevel;
                }
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
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 camera)
        {
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

        #endregion Methods
    }
}
