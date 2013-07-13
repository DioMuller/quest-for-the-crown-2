using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Levels.Mapping
{
    /// <summary>
    /// Collision position enumeration
    /// </summary>
    public enum CollisionPosition
    {
        UpperLeft = 0,
        UpperRight = 1,
        DownLeft = 2,
        DownRight = 3
    }

    /// <summary>
    /// Tile info.
    /// </summary>
    public class Tile
    {
        #region Attributes
        /// <summary>
        /// Tile terrain.
        /// </summary>
        private int[] _terrain;
        #endregion Attributes

        #region Properties
        /// <summary>
        /// Tile id.
        /// </summary>
        public int Id { get; private set; }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Creates instance from the data.
        /// </summary>
        /// <param name="id">Tile id.</param>
        /// <param name="terrain">Terrain id.</param>
        public Tile( int id)
        {
            Id = id;
            _terrain = new int[4]{0,0,0,0};
        }
        #endregion Constructor

        #region Methods
        public int GetCollision(CollisionPosition position)
        {
            return _terrain[(int) position];
        }

        public void SetCollision(CollisionPosition position, int value)
        {
            _terrain[(int)position] = value;
        }
        #endregion Methods
    }
}
