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
        /// Dungeons on this world/dungeon.
        /// </summary>
        List<LevelCollection> _dungeons;

        /// <summary>
        /// Players list.
        /// </summary>
        List<Player> _players;

        #endregion Attributes

        #region Properties
        /// <summary>
        /// Current Level (May be changed to CurrentLevel array)
        /// </summary>
        private IEnumerable<Level> CurrentLevels
        {
            get
            {
                HashSet<Level> list = new HashSet<Level>();

                foreach( Player player in _players )
                {
                    list.Add(GetLevelByPlayer(player));
                }

                return list;
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
            
            _players = new List<Player>();
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Changes the current level for playerNum.
        /// </summary>
        /// <param name="playerNum">Player identifier.</param>
        /// <param name="direction">Direction to teleport.</param>
        internal void GoToNeighbor(Player player, Level level, Direction direction)
        {

            int neighbor = level.GetNeighbor(direction);

            if( neighbor != 0 )
            {
                level.RemoveEntity(player);
                player.CurrentLevel = neighbor;
                Level newLevel = GetLevelByPlayer(player);
                newLevel.AddEntity(player);

                //TODO: Add fancy transition drawing/update logic?

                switch( direction )
                {
                    case Direction.East:
                        player.Position = new Vector2(0, player.Position.Y);
                        break;
                    case Direction.North:
                        player.Position = new Vector2(player.Position.X, level.Map.PixelSize.Y - player.Size.Y);
                        break;
                    case Direction.South:
                        player.Position = new Vector2(player.Position.X, 0);
                        break;
                    case Direction.West:
                        player.Position = new Vector2(level.Map.PixelSize.X - player.Size.X, player.Position.Y);
                        break;
                }
            }
        }

        /// <summary>
        /// Get the level.
        /// </summary>
        /// <param name="index">Level index.</param>
        /// <returns>The level.</returns>
        internal Level GetLevel(int index)
        {
            return _levels[index];
        }

        /// <summary>
        /// Get player by Level.
        /// </summary>
        /// <param name="player">Player reference</param>
        /// <returns>The level the player is in.</returns>
        internal Level GetLevelByPlayer(Player player)
        {
            if( player.CurrentDungeon == -1 ) return _levels[player.CurrentLevel - 1];

            return _dungeons[player.CurrentDungeon].GetLevel(player.CurrentLevel - 1);
        }

        #region Public Methods
        /// <summary>
        /// Updates the currently active levels and their entities.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public void Update(GameTime gameTime)
        {
            foreach( Level lv in CurrentLevels )
            {
                lv.Update(gameTime);
            }
        }

        /// <summary>
        /// Draws current active levels and their entities.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle clientBounds)
        {
            foreach( Level lv in CurrentLevels )
            {
                //TODO: Calculate camera for split screen?
                Vector2 camera = GetCameraPosition(lv.Player, lv.Map.PixelSize, clientBounds);
                lv.Draw(gameTime, spriteBatch, camera);
            }
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

            level.Parent = this;

            if( level.Players.Count > 0) _players.AddRange(level.Players);

            return true;
        }

        public int AddPlayer(Player player)
        {
            if( _players.Count < 4 )
            {
                _players.Add(player);
                return _players.Count;
            }

            return 0;
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
