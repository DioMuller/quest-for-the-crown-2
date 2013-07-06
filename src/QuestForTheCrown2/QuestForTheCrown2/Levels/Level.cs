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
        South = 3,
        None = -1
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
        private Queue<Entity> _newEntities, _oldEntities;
        private Queue<Player> _oldPlayers;
        #endregion Attributes

        #region Properties
        public int Id { get; private set; }
        internal List<Player> Players { get; private set; }

        public Player Player
        {
            get
            {
                if (Players.Count != 0) return Players.First();
                return null;
            }
        }

        internal Map Map
        {
            get
            {
                return _map;
            }
        }

        internal LevelCollection Parent { get; set; }
        #endregion Properties

        #region Constructor
        public Level(int id, Map map)
        {
            Id = id;
            _map = map;
            _entities = new List<Entity>();
            _newEntities = new Queue<Entity>();
            _oldEntities = new Queue<Entity>();
            _oldPlayers = new Queue<Entities.Characters.Player>();
            _neighbors = new int[4];

            Players = new List<Player>();
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

            foreach (Entity en in _entities.OrderBy(e => e.Position.Y + e.Size.Y))
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
            if (Players.Any(p => p.LevelTransitionPercent != 0))
                return;

            while (_oldEntities.Count > 0)
            {
                _entities.Remove(_oldEntities.Dequeue());
            }

            while (_newEntities.Count > 0)
            {
                _entities.Add(_newEntities.Dequeue());
            }

            foreach (Entity e in _entities)
            {
                e.Update(gameTime, this);
            }
        }

        /// <summary>
        /// Gets neighbor ID
        /// </summary>
        /// <param name="direction">Desired direction</param>
        /// <returns>Neigbhbor id</returns>
        public int GetNeighbor(Direction direction)
        {
            return _neighbors[(int)direction];
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

        /// <summary>
        /// Adds entity to the entities list
        /// </summary>
        /// <param name="entity"></param>
        public void AddEntity(Entity entity)
        {
            _newEntities.Enqueue(entity);
            if (entity is Player)
                Players.Add((Player)entity);
        }

        /// <summary>
        /// Adds an array of entities to the entity list.
        /// </summary>
        /// <param name="entities"></param>
        public void AddEntity(IEnumerable<Entity> entities)
        {
            foreach (Entity en in entities)
                AddEntity(en);
        }

        /// <summary>
        /// Removes entity from the level.
        /// </summary>
        /// <param name="entity">Entity to be removed.</param>
        public void RemoveEntity(Entity entity)
        {
            _oldEntities.Enqueue(entity);
            if (entity is Player)
                Players.Remove((Player)entity);
        }

        /// <summary>
        /// Checks the entities that collide with with a given rect.
        /// </summary>
        /// <param name="rect">Collision rect.</param>
        /// <returns>Entities that collide with the rectangle.</returns>
        public IEnumerable<Entity> CollidesWith(Rectangle rect)
        {
            return (from ent in _entities
                    where !ent.OverlapEntities && rect.Intersects(ent.CollisionRect)
                    select ent);
        }
        #endregion Methods

        /// <summary>
        /// Checks if the entity is currently in the map.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool ContainsEntity(Entity entity)
        {
            return _entities.Contains(entity) || _newEntities.Contains(entity);
        }

        public void GoToNeighbor(Player player, Direction direction)
        {
            Parent.GoToNeighbor(player, this, direction);
        }

        /// <summary>
        /// Tests entities matching a specific predicate.
        /// </summary>
        /// <param name="predicate">The function that checks the entity.</param>
        /// <returns>All entities where the predicate is true.</returns>
        public IEnumerable<object> FindEntities(Func<Entity, bool> predicate)
        {
            return _entities.Where(predicate);
        }
    }
}
