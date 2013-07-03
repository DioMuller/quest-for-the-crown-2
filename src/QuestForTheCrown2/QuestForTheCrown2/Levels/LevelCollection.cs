using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Levels
{
    class LevelCollection
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
        #endregion Attributes

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
    }
}
