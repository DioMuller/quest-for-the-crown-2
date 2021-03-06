﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels.Mapping;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Entities.Characters;
using System.Text.RegularExpressions;
using QuestForTheCrown2.Base;

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
        private bool _bgmStarted = false;
        private Map _map;
        private List<Entity> _entities;
        private int[] _neighbors;
        private Queue<Entity> _newEntities, _oldEntities;
        private Dictionary<string, List<Entity>> _entitiesByCategory;
        #endregion Attributes

        #region Properties
        public int Id { get; private set; }
        internal IEnumerable<Entity> Players
        {
            get { return GetEntities("Player"); }
        }

        internal Map Map
        {
            get { return _map; }
        }

        internal LevelCollection Parent { get; set; }

        public string BGM { get; set; }

        public string Title { get; set; }
        #endregion Properties

        #region Constructor
        public Level(int id, Map map, IEnumerable<Entity> entities = null)
        {
            //Updates collision map
            map.UpdateCollision();

            Id = id;
            _map = map;
            _entities = new List<Entity>();
            _newEntities = new Queue<Entity>();
            _oldEntities = new Queue<Entity>();
            _neighbors = new int[4];
            _entitiesByCategory = new Dictionary<string, List<Entity>>();

            if (entities != null)
                AddEntity(entities);
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

            foreach (Entity en in _entities.Where(e => !e.IsInvisible && !_oldEntities.Contains(e)).OrderBy(e => e.Position.Y + e.Size.Y))
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
            if (!_bgmStarted || LevelCollection.CurrentPlayers.Count() == 1)
            {
                SoundManager.PlayBGM(BGM);
                _bgmStarted = true;
            }

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

            if (entity.Category != null)
            {
                List<Entity> existingCategoryList;
                if (!_entitiesByCategory.TryGetValue(entity.Category, out existingCategoryList))
                {
                    existingCategoryList = new List<Entity>();
                    _entitiesByCategory.Add(entity.Category, existingCategoryList);
                }

                existingCategoryList.Add(entity);
            }

            if (entity.Category == "Player")
                _bgmStarted = false;
        }

        /// <summary>
        /// Adds an array of entities to the entity list.
        /// </summary>
        /// <param name="entities"></param>
        public void AddEntity(IEnumerable<Entity> entities)
        {
            foreach (Entity en in entities)
                if (en != null)
                    AddEntity(en);
        }

        /// <summary>
        /// Removes entity from the level.
        /// </summary>
        /// <param name="entity">Entity to be removed.</param>
        public void RemoveEntity(Entity entity)
        {
            if (_newEntities.Contains(entity))
                _newEntities = new Queue<Entity>(_newEntities.Where(e => e != entity));
            else
                _oldEntities.Enqueue(entity);

            if (entity.Category != null && _entitiesByCategory.ContainsKey(entity.Category))
                _entitiesByCategory[entity.Category].Remove(entity);

            RemoveEntities(_entities.Where(e => e.Parent == entity));
        }

        /// <summary>
        /// Removes entities from the level.
        /// </summary>
        /// <param name="entity">Entities to be removed.</param>
        public void RemoveEntities(IEnumerable<Entity> entities)
        {
            foreach (var ent in entities)
                RemoveEntity(ent);
        }

        /// <summary>
        /// Checks the entities that collide with with a given rect.
        /// </summary>
        /// <param name="rect">Collision rect.</param>
        /// <returns>Entities that collide with the rectangle.</returns>
        public IEnumerable<Entity> CollidesWith(Rectangle rect, bool includeOverlaping = false)
        {
            return (from ent in _entities
                    where (!ent.OverlapEntities || includeOverlaping) && rect.Intersects(ent.CollisionRect)
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

        /// <summary>
        /// Teleports the player to the Neighbor.
        /// </summary>
        /// <param name="player">Player to be teleported.</param>
        /// <param name="direction">Direction.</param>
        public void GoToNeighbor(Entity entity, Direction direction)
        {
            Parent.GoToNeighbor(entity, this, direction);
        }

        public void GoToDungeon(Entity en, int dungeon)
        {
            Parent.GoToDungeon(en, dungeon);
        }

        /// <summary>
        /// Tests entities matching a specific predicate.
        /// </summary>
        /// <param name="predicate">The function that checks the entity.</param>
        /// <returns>All entities where the predicate is true.</returns>
        public IEnumerable<Entity> FindEntities(Func<Entity, bool> predicate)
        {
            return _entities.Where(predicate);
        }

        /// <summary>
        /// Finds all entities matching any of the specified categories.
        /// </summary>
        /// <param name="category">Array of possible categories.</param>
        /// <returns>All entities matching any of the specified categories.</returns>
        public IEnumerable<Entity> GetEntities(params string[] category)
        {
            if (category.Length == 0)
                throw new ArgumentException("A category must be specified", "category");

            foreach (var cat in category)
            {
                if (_entitiesByCategory.ContainsKey(cat))
                {
                    foreach (var ent in _entitiesByCategory[cat].ToList())
                        yield return ent;
                }
            }
        }

        /// <summary>
        /// Find all entities of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the entity being search</typeparam>
        /// <returns>All entities matching the specified type.</returns>
        public IEnumerable<T> GetEntities<T>() where T : Entity
        {
            return _entities.OfType<T>();
        }

        /// <summary>
        /// Finds the entity of the desired category that is closer to an specified entity.
        /// </summary>
        /// <param name="relativeTo">Find the entity which is closer to the specified entity.</param>
        /// <param name="category">The category of the entity being searched.</param>
        /// <returns></returns>
        public EntityRelativePosition EntityCloserTo(Entity relativeTo, params string[] category)
        {
            return GetEntities(category).CloserTo(relativeTo);
        }

        public IEnumerable<Entity> GetEntities(Func<Entity, bool> predicate)
        {
            return _entities.Where(predicate);
        }
    }
}
