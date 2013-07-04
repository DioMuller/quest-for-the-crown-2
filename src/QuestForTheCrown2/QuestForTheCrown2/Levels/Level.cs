using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels.Mapping;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Entities.Characters;

namespace QuestForTheCrown2.Levels
{
    /// <summary>
    /// Map directions, used for neighbors.
    /// </summary>
    public enum Direction
    {
        West = 0,
        North = 1,
        East = 2,
        South = 3
    }

    /// <summary>
    /// Level information
    /// </summary>
    public class Level
    {
        #region Attributes
        private Map _map;
        private List<Entity> _entities;
        private int[] _neighbors;
        #endregion Attributes

        #region Properties
        public int Id { get; private set; }

        public MainCharacter Player
        {
            get
            {
                return (from Entity entity in _entities where entity is MainCharacter select entity).First() as MainCharacter;
            }
        }

        internal Map Map
        {
            get
            {
                return _map;
            }
        }
        #endregion Properties

        #region Constructor
        public Level(int id, Map map)
        {
            Id = id;
            _map = map;
            _entities = new List<Entity>();
            _neighbors = new int[4];
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Draws the level (map and entities).
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="camera"></param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 camera)
        {
            _map.Draw(gameTime, spriteBatch, camera);

            foreach( Entity en in _entities )
            {
                en.Draw(gameTime, spriteBatch, camera);
            }
        }

        /// <summary>
        /// Updates the level.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public void Update(GameTime gameTime)
        {
            foreach (Entity e in _entities)
            {
                e.Update(gameTime, _map);
            }
        }

        /// <summary>
        /// Gets neighbor ID
        /// </summary>
        /// <param name="direction">Desired direction</param>
        /// <returns>Neigbhbor id</returns>
        public int GetNeighbor(Direction direction)
        {
            return _neighbors[(int) direction];
        }

        /// <summary>
        /// Sets neighbor
        /// </summary>
        /// <param name="direction">Neighbor direction</param>
        /// <param name="value">Neighbor id</param>
        public void SetNeighbor(Direction direction, int value)
        {
            _neighbors[(int)direction] = value;
        }

        public void AddEntity(Entity entity)
        {
            _entities.Add(entity);
        }

        public void AddEntity(IEnumerable<Entity> entities)
        {
            _entities.AddRange(entities);
        }
        #endregion Methods
    }
}
