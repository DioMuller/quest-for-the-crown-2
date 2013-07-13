using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.Levels.Mapping
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
        public Point TileSize { get; private set; }

        /// <summary>
        /// Image source path.
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// Image size.
        /// </summary>
        public Point Size { get; private set; }

        /// <summary>
        /// Tiles list.
        /// </summary>
        public Tile[] Tiles { get; private set; }

        public int NumRows { get; private set; }
        public int NumCols { get; private set; }

        internal Texture2D Texture { get; private set; }

        /// <summary>
        /// First tile id
        /// </summary>
        public int FirstTileId
        {
            get
            {
                return FirstGID;
            }
        }

        public int LastTileId
        {
            get
            {
                return FirstGID + (NumCols * NumRows);
            }
        }
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
        public Tileset(int firstgid, string name, Point tileSize, string imageSource, Point imageSize)
        {
            FirstGID = firstgid;
            Name = name;
            TileSize = tileSize;
            Source = imageSource;
            Size = imageSize;

            Texture = GameContent.LoadContent<Texture2D>(Source);

            NumRows = Size.Y / TileSize.Y;
            NumCols = Size.X / TileSize.X;

            Tiles = new Tile[NumRows*NumCols];

            for( int i = 0; i < NumRows * NumCols; i++ )
            {
                Tiles[i] = new Tile(i);
            }
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Get rectangle for the specified tile.
        /// </summary>
        /// <param name="tileId">Tile id.</param>
        /// <returns></returns>
        public Rectangle GetRect(int tileId)
        {
            tileId -= FirstTileId;

            if (tileId < 0)
                return Rectangle.Empty;

            int row = tileId / NumCols;
            

            if (row >= NumRows)
                return Rectangle.Empty;

            int col = (tileId % NumCols);

            Rectangle rect = new Rectangle(col * TileSize.X, row * TileSize.Y, TileSize.X, TileSize.Y);
            return rect;
        }

        /// <summary>
        /// Gets the tile by the Id value.
        /// </summary>
        /// <param name="tileId">Desired tile Id.</param>
        /// <returns>Tile instance.</returns>
        public Tile GetTileById(int tileId)
        {
            tileId = tileId - FirstGID;

            return (from Tile tile in Tiles where tile.Id == tileId select tile).FirstOrDefault();
        }
        #endregion Methods
    }
}
