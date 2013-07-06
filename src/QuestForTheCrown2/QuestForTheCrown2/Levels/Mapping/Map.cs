﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuestForTheCrown2.Levels.Mapping
{
    /// <summary>
    /// Represents the map, with all the layers.
    /// </summary>
    public class Map
    {
        #region Attributes
        /// <summary>
        /// Collision map.
        /// </summary>
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
        /// Map size in pixels
        /// </summary>
        public Point PixelSize { get; private set; }

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
            PixelSize = new Point(TileSize.X * Size.X, TileSize.Y * size.Y);

            _collisionMap = new int[Size.X * 2, Size.Y * 2];

            Tilesets = new List<Tileset>();
            Layers = new List<Layer>();
        }
        #endregion Constructor

        #region Methods

        #region Public Methods
        /// <summary>
        /// Tests collision with the map.
        /// </summary>
        /// <param name="rect">Collision rectangle.</param>
        /// <returns>Is colliding?</returns>
        public bool Collides(Rectangle rect, bool allowOutside = false)
        {
            if( !allowOutside && IsOutsideBorders(rect) ) return true;

            int result = 0;
            int mod_x = (TileSize.X / 2);
            int mod_y = (TileSize.Y / 2);
            int min_x = rect.X / mod_x;
            int min_y = rect.Y / mod_y;
            int max_x = rect.X / mod_x + ( rect.X % mod_x == 0 ? 1 : 2 );
            int max_y = rect.Y / mod_y + (rect.Y % mod_y == 0 ? 1 : 2);

            for( int x = min_x; x < max_x; x++ )
            {
                for( int y = min_y; y < max_y; y++ )
                {
                    if( x < Size.X * 2 && y < Size.Y * 2 && x >= 0 && y >= 0 )
                    {
                        result += _collisionMap[x,y];
                    }
                }
            }

            return (result != 0); //If everything is 0; it won't collide.
        }

        /// <summary>
        /// Checks if rectangle is outside borders
        /// </summary>
        /// <param name="rect">Collision rectangle</param>
        /// <returns>Is the rectangle outside borders?</returns>
        public bool IsOutsideBorders(Rectangle rect)
        {
            return (rect.X < 0 || rect.Y < 0 || rect.X + rect.Width > PixelSize.X || rect.Y + rect.Height > PixelSize.Y );
        }

        /// <summary>
        /// Draws the map.
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="spriteBatch">Sprite batch.</param>
        /// <param name="camera">Game camera.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 camera)
        {
            foreach (Layer layer in Layers)
            {
                for (int y = 0; y < layer.Size.Y; y++)
                {
                    for (int x = 0; x < layer.Size.X; x++)
                    {
                        int tileId = layer.GetData(x, y);
                        if (tileId != 0)
                        {
                            Tileset set = GetTileset(tileId);
                            spriteBatch.Draw(set.Texture,
                                new Rectangle(
                                    (int)(x * TileSize.X - camera.X),
                                    (int)(y * TileSize.Y - camera.Y),
                                    TileSize.X,
                                    TileSize.Y),
                                set.GetRect(tileId),
                                Color.White);
                        }
                    }
                }
            }
        }
        #endregion Public Methods

        #region Private Methods
        /// <summary>
        /// Gets tile referent to the ID.
        /// </summary>
        /// <param name="tileId">Tile id.</param>
        /// <returns>Desired tile.</returns>
        private Tile GetTile(int tileId)
        {
            if (tileId == 0) return null;

            Tileset set = GetTileset(tileId);

            if( set == null ) return null;

            return set.GetTileById(tileId);
        }

        private Tileset GetTileset(int tileId)
        {
            return (from Tileset ts in Tilesets
                where ts.FirstTileId <= tileId && ts.LastTileId >= tileId
                select ts).FirstOrDefault();
        }
        #endregion Private Methods

        #region Internal Methods
        /// <summary>
        /// Updates collision map.
        /// </summary>
        internal void UpdateCollision()
        {
            for (int y = 0; y < Size.Y * 2; y+=2)
            {
                for (int x = 0; x < Size.X * 2; x+=2)
                {
                    foreach (Layer layer in Layers)
                    {
                        int tileId = layer.GetData(x/2, y/2);

                        if (tileId != 0)
                        {
                            Tile tile = GetTile(tileId);

                            _collisionMap[x, y] = Math.Max(tile.GetCollision(CollisionPosition.UpperLeft), _collisionMap[x, y]); ;
                            _collisionMap[x + 1, y] = Math.Max(tile.GetCollision(CollisionPosition.UpperRight), _collisionMap[x + 1, y]); ;
                            _collisionMap[x, y + 1] = Math.Max(tile.GetCollision(CollisionPosition.DownLeft), _collisionMap[x, y + 1]); ;
                            _collisionMap[x + 1, y + 1] = Math.Max(tile.GetCollision(CollisionPosition.DownRight), _collisionMap[x + 1, y + 1]);
                        }
                    }
                }
            }
        }
        #endregion Internal Methods
        #endregion Methods
    }
}
