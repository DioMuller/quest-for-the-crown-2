using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace QuestForTheCrown2.External.Tiled
{
    public class Map
    {
        #region Properties
        /// <summary>
        /// Map name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Map size (in tiles).
        /// </summary>
        public Point Size { get; private set; }

        /// <summary>
        /// Tile Size
        /// </summary>
        public Point TileSize { get; private set; }

        /// <summary>
        /// Tilesets.
        /// </summary>
        public List<Tileset> Tilesets { get; private set; }

        /// <summary>
        /// Map Layers
        /// </summary>
        public List<Layer> Layers { get; private set; }
        #endregion Properties

        #region Constructor
        public Map(string name, Point size, Point tileSize)
        {
            Name = name;
            Size = size;
            TileSize = tileSize;

            Tilesets = new List<Tileset>();
            Layers = new List<Layer>();
        }
        #endregion Constructor
    }
}
