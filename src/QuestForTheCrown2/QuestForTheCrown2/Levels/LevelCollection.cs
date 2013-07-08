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
    /// <summary>
    /// Class to represent a waypoint (where the player stoped).
    /// </summary>
    internal class Waypoint
    {
        public Entity Entity { get; set; }
        public Level Level { get; set; }
        public Vector2 Position { get; set; }
    }

    /// <summary>
    /// Dungeon/Overworld/Other types of level collection.
    /// </summary>
    public class LevelCollection
    {
        #region Attributes
        /// <summary>
        /// This world/dungeon levels.
        /// </summary>
        List<Level> _levels;

        /// <summary>
        /// Stored waypoints: Where the player was when he quit this collection.
        /// </summary>
        List<Waypoint> _storedWaypoints;
        #endregion Attributes

        #region Properties
        /// <summary>
        /// Dungeon id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Current Level (May be changed to CurrentLevel array)
        /// </summary>
        private IEnumerable<Level> CurrentLevels
        {
            get
            {
                return _levels.Where(l => l.Players.Any());
            }
        }

        public GameMain Parent { get; set; }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Creates the level collection.
        /// </summary>
        public LevelCollection()
        {
            _levels = new List<Level>();
            _storedWaypoints = new List<Waypoint>();
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Changes the current level for playerNum.
        /// </summary>
        /// <param name="playerNum">Player identifier.</param>
        /// <param name="direction">Direction to teleport.</param>
        internal void GoToNeighbor(Entity entity, Level level, Direction direction)
        {
            int neighbor = level.GetNeighbor(direction);

            if( neighbor == -1 )
            {
                BackToWaypoint(entity);
            }
            else if (neighbor != 0)
            {
                entity.TransitioningToLevel = neighbor;
                entity.LevelTransitionPercent = 0;
                entity.LevelTransitionDirection = direction;
            }
        }

        /// <summary>
        /// Teleports player to dungeon
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="dungeon">Dungeon ID</param>
        internal void GoToDungeon(Entity entity, int map)
        {
            _storedWaypoints.Add(new Waypoint { Entity = entity, Level = GetLevelByEntity(entity), Position = entity.Position } );

            GetLevelByEntity(entity).RemoveEntity(entity);

            entity.CurrentLevel = map;

            Level newLevel = GetLevelByEntity(entity);
            newLevel.AddEntity(entity);

            //TODO: Load this from an XML file, maybe?
            entity.Position = new Vector2(newLevel.Map.PixelSize.X / 2, newLevel.Map.PixelSize.Y - entity.Size.Y - 1);
        }

        /// <summary>
        /// Returns to the waypoint
        /// </summary>
        /// <param name="entity">The entity that will return to the waypoint</param>
        internal void BackToWaypoint(Entity entity)
        {
            Waypoint wp = _storedWaypoints.Where(w => w.Entity == entity).Last();

            GetLevelByEntity(entity).RemoveEntity(entity);

            entity.CurrentLevel = wp.Level.Id;

            wp.Level.AddEntity(entity);

            entity.Position = new Vector2(wp.Position.X, wp.Position.Y + 5);

            _storedWaypoints.Remove(wp);
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
        /// Get the level in which the entity is at.
        /// </summary>
        /// <param name="entity">Entity reference</param>
        /// <returns>The level the entity is in.</returns>
        internal Level GetLevelByEntity(Entity entity)
        {
            return _levels[entity.CurrentLevel - 1];
        }

        #region Public Methods
        /// <summary>
        /// Updates the currently active levels and their entities.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public void Update(GameTime gameTime)
        {
            if( CurrentLevels.Count() == 0 ) Parent.ChangeState(GameState.GameOver);

            foreach (Level lv in CurrentLevels)
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
            foreach (Level lv in CurrentLevels)
            {
                var player = lv.Players.FirstOrDefault();
                               
                if (player.TransitioningToLevel != 0)
                {
                    SlideScreen(gameTime, spriteBatch, clientBounds, lv, player);
                }
                else
                {
                    //TODO: Calculate camera for split screen?
                    Vector2 camera = GetCameraPosition(player, lv.Map.PixelSize, clientBounds);
                    lv.Draw(gameTime, spriteBatch, camera);
                }
            }
        }
        #endregion Public Methods

        #region Collection Methods
        /// <summary>
        /// Adds a level to the collection.
        /// </summary>
        /// <param name="level">Level to be added.</param>
        public bool AddLevel(Level level)
        {
            if (_levels.Any(l => l.Id == level.Id))
                return false;

            _levels.Add(level);
            level.Parent = this;
            return true;
        }
        #endregion Collection Methods

        #region Camera Methods
        /// <summary>
        /// Gets camera position relative to an Entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="mapSize">Complete map size.</param>
        /// <param name="screenSize">Screen total size.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Screen sliding effect
        /// </summary>
        /// <param name="gameTime">Current game time</param>
        /// <param name="spriteBatch">Sprite batch for drawing</param>
        /// <param name="clientBounds">Window bounds</param>
        /// <param name="lv">Previous level</param>
        /// <param name="player">Player.</param>
        private void SlideScreen(GameTime gameTime, SpriteBatch spriteBatch, Rectangle clientBounds, Level lv, Entity player)
        {
            var newLevel = GetLevel(player.TransitioningToLevel - 1);
            Vector2 camera = GetCameraPosition(player, lv.Map.PixelSize, clientBounds);
            Vector2 camera2 = GetCameraPosition(player, newLevel.Map.PixelSize, clientBounds);

            var cameraXTranslation = clientBounds.Width * player.LevelTransitionPercent;
            var cameraYTranslation = clientBounds.Height * player.LevelTransitionPercent;

            player.LevelTransitionPercent += (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000);

            switch (player.LevelTransitionDirection)
            {
                case Direction.West:
                    newLevel.Draw(gameTime, spriteBatch, new Vector2(newLevel.Map.PixelSize.X - clientBounds.Width + (clientBounds.Width - cameraXTranslation), camera2.Y));
                    lv.Draw(gameTime, spriteBatch, new Vector2(-cameraXTranslation, camera.Y));
                    player.Position = new Vector2(-(player.Size.X * player.LevelTransitionPercent), player.Position.Y);
                    break;
                case Direction.East:
                    newLevel.Draw(gameTime, spriteBatch, new Vector2(-clientBounds.Width + cameraXTranslation, camera2.Y));
                    lv.Draw(gameTime, spriteBatch, new Vector2(camera.X + cameraXTranslation, camera.Y));
                    player.Position = new Vector2(lv.Map.PixelSize.X - (player.Size.X * (1 - player.LevelTransitionPercent)), player.Position.Y);
                    break;
                case Direction.South:
                    newLevel.Draw(gameTime, spriteBatch, new Vector2(camera2.X, -clientBounds.Height + cameraYTranslation));
                    lv.Draw(gameTime, spriteBatch, new Vector2(camera.X, camera.Y + cameraYTranslation));
                    player.Position = new Vector2(player.Position.X, lv.Map.PixelSize.Y - (player.Size.Y * (1 - player.LevelTransitionPercent)));
                    break;
                case Direction.North:
                    newLevel.Draw(gameTime, spriteBatch, new Vector2(camera2.X, newLevel.Map.PixelSize.Y - clientBounds.Height + (clientBounds.Height - cameraYTranslation)));
                    lv.Draw(gameTime, spriteBatch, new Vector2(camera.X, -cameraYTranslation));
                    player.Position = new Vector2(player.Position.X, -(player.Size.Y * player.LevelTransitionPercent));
                    break;
                default:
                    player.LevelTransitionPercent = 1;
                    break;
            }

            if (player.LevelTransitionPercent >= 1)
            {
                newLevel.AddEntity(player);
                lv.RemoveEntity(player);

                player.CurrentLevel = player.TransitioningToLevel;
                player.TransitioningToLevel = 0;
                switch (player.LevelTransitionDirection)
                {
                    case Direction.West:
                        player.Position = new Vector2(newLevel.Map.PixelSize.X + player.Position.X, player.Position.Y);
                        break;
                    case Direction.East:
                        player.Position = new Vector2(player.Position.X - lv.Map.PixelSize.X, player.Position.Y);
                        break;
                    case Direction.North:
                        player.Position = new Vector2(player.Position.X, newLevel.Map.PixelSize.Y + player.Position.Y);
                        break;
                    case Direction.South:
                        player.Position = new Vector2(player.Position.X, player.Position.Y - lv.Map.PixelSize.Y);
                        break;
                }

                player.LevelTransitionDirection = Direction.None;
                player.LevelTransitionPercent = 0;
            }
        }
        #endregion Camera Methods

        #endregion Methods
    }
}
