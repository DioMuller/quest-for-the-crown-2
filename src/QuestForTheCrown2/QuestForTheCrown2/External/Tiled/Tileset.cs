using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuestForTheCrown2.External.Tiled
{
    /// <summary>
    /// Represents a tileset.
    /// </summary>
    public class Tileset
    {
        #region Properties
        /// <summary>
        /// First GID.
        /// </summary>
        public int FirstGID { get; private set; }
 
        /// <summary>
        /// Tileset name.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Tile size.
        /// </summary>
        public Vector2 TileSize { get; private set; }

        /// <summary>
        /// Image source path.
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// Image size.
        /// </summary>
        public Vector2 Size { get; private set; }

        /// <summary>
        /// Tiles list.
        /// </summary>
        public List<Tile> Tiles { get; private set; }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Creates a tileset with the parameters.
        /// </summary>
        /// <param name="firstgid">First GID</param>
        /// <param name="name">Tileset name</param>
        /// <param name="tileSize">Tile size</param>
        /// <param name="imageSource">Image source path</param>
        /// <param name="imageSize">Image size in pixels</param>
        public Tileset(int firstgid, string name, Vector2 tileSize, string imageSource, Vector2 imageSize)
        {
            FirstGID = firstgid;
            Name = name;
            TileSize = tileSize;
            Source = imageSource;
            Size = imageSize;

            Tiles = new List<Tile>();
        }
        #endregion Constructor
    }
}
