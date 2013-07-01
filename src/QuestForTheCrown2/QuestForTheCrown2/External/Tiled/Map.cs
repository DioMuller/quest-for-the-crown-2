using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace QuestForTheCrown2.External.Tiled
{
    public class Map
    {
        #region Attributes
        private int[,] _collisionMap;
        #endregion Attributes

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

            _collisionMap = new int[Size.X * 2, Size.Y * 2];

            Tilesets = new List<Tileset>();
            Layers = new List<Layer>();
        }
        #endregion Constructor

        #region Methods

        #region Private Methods
        private Tile GetTile(int tileId)
        {
            Tileset set = (from Tileset ts in Tilesets
                where ts.FirstTileId >= tileId && ts.LastTileId <= tileId
                select ts).FirstOrDefault();

            if( set == null ) return null;

            return set.GetTileById(tileId);
        }
        #endregion Private Methods

        #region Internal Methods
        internal void UpdateCollision()
        {
            for (int y = 0; y < Size.Y; y+=2)
            {
                for (int x = 0; x < Size.X; x+=2)
                {
                    foreach (Layer layer in Layers)
                    {
                        int tileId = layer.GetData(x/2, y/2);
                        Tile tile = GetTile(tileId);

                        _collisionMap[x, y] = Math.Max(tile.GetCollision(CollisionPosition.UpperLeft), _collisionMap[x, y]); ;
                        _collisionMap[x+1,y] = Math.Max(tile.GetCollision(CollisionPosition.UpperRight), _collisionMap[x+1,y]);;
                        _collisionMap[x,y+1] = Math.Max(tile.GetCollision(CollisionPosition.DownLeft), _collisionMap[x,y+1]);;
                        _collisionMap[x+1,y+1] = Math.Max(tile.GetCollision(CollisionPosition.DownRight), _collisionMap[x+1,y+1]);
                    }
                }
            }
        }
        #endregion Internal Methods
        #endregion Methods
    }
}
