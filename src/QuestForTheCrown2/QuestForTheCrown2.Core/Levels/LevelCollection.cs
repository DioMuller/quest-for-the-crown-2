﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Characters;
using QuestForTheCrown2.GUI.Components;
using QuestForTheCrown2.GUI.GameGUI;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.Levels
{
    /// <summary>
    /// Class to represent a waypoint (where the player stoped).
    /// </summary>
    [Serializable]
    public class Waypoint
    {
        //public Entity Entity { get; set; }
        public int LevelId { get; set; }
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
        static List<Level> _levels;

        /// <summary>
        /// Stored waypoints: Where the player was when he quit this collection.
        /// </summary>
        public static Dictionary<Entity, List<Waypoint>> StoredWaypoints { get; set; }

        public static IEnumerable<Entity> CurrentPlayers { get { return CurrentLevels.SelectMany(l => l.Players); } }

        /// <summary>
        /// Game GUI.
        /// </summary>
        GameGUI _gui;

        /// <summary>
        /// Title Card.
        /// </summary>
        Dictionary<Entity, TitleCard> _cards;
        #endregion Attributes

        #region Properties
        /// <summary>
        /// Dungeon id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Current Level (May be changed to CurrentLevel array)
        /// </summary>
        private static IEnumerable<Level> CurrentLevels
        {
            get
            {
                return _levels.Where(l => l.Players.Any());
            }
        }

        public IEnumerable<Entity> Players
        {
            get
            {
                return CurrentLevels.SelectMany(lv => lv.Players);
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

            _gui = new GameGUI();
            _cards = new Dictionary<Entity, TitleCard>();
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
            if (entity.Weapons != null)
            {
                foreach (var weapon in entity.Weapons)
                    level.RemoveEntity(weapon);
            }

            int neighbor = level.GetNeighbor(direction);

            if (neighbor == -1)
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
            StoredWaypoints[entity].Add(new Waypoint { LevelId = GetLevelByEntity(entity).Id, Position = entity.Position });

            GetLevelByEntity(entity).RemoveEntity(entity);

            entity.CurrentLevel = map;

            Level newLevel = GetLevelByEntity(entity);
            newLevel.AddEntity(entity);

            if (!_cards.ContainsKey(entity))
                _cards[entity] = new TitleCard { CurrentTitle = newLevel.Title };
            else
                _cards[entity].CurrentTitle = newLevel.Title;

            //TODO: Load this from an XML file, maybe?
            entity.Position = new Vector2(newLevel.Map.PixelSize.X / 2 - 20, newLevel.Map.PixelSize.Y - entity.Size.Y - 1);
        }

        /// <summary>
        /// Returns to the waypoint
        /// </summary>
        /// <param name="entity">The entity that will return to the waypoint</param>
        internal void BackToWaypoint(Entity entity)
        {
            Waypoint wp = StoredWaypoints[entity].Last();

            GetLevelByEntity(entity).RemoveEntity(entity);

            entity.CurrentLevel = wp.LevelId;

            GetLevel(wp.LevelId).AddEntity(entity);

            entity.Position = new Vector2(wp.Position.X, wp.Position.Y + 5);

            StoredWaypoints[entity].Remove(wp);
        }

        /// <summary>
        /// Get the level.
        /// </summary>
        /// <param name="index">Level index.</param>
        /// <returns>The level.</returns>
        internal Level GetLevel(int id)
        {
            return _levels.Where(l => l.Id == id).First();
        }

        /// <summary>
        /// Get the level in which the entity is at.
        /// </summary>
        /// <param name="entity">Entity reference</param>
        /// <returns>The level the entity is in.</returns>
        internal Level GetLevelByEntity(Entity entity)
        {
            return _levels.Where(l => l.Id == entity.CurrentLevel).First();
        }

        #region Public Methods
        /// <summary>
        /// Updates the currently active levels and their entities.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public void Update(GameTime gameTime)
        {
            if (CurrentLevels.Count() == 0) Parent.ChangeState(GameState.GameOver);

            foreach (Level lv in CurrentLevels)
            {
                lv.Update(gameTime);
            }
        }

        /// <summary>
        /// Draws current active levels and their entities.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public void Draw(GraphicsDevice graphicsDevice, GameTime gameTime, SpriteBatch spriteBatch, Rectangle clientBounds)
        {
            Vector2? cameraPos = null;
            if (!CurrentLevels.Any())
                return;

            var split = CurrentLevels.Count() > 1;
            if (!split)
            {
                var level = CurrentLevels.First();
                if (level.Players.Count() > 1)
                {
                    var p1 = level.Players.First();
                    var p2 = level.Players.Last();
                    var p1Pos = p1.CenterPosition;
                    var p2Pos = p2.CenterPosition;
                    var dist = p1Pos - p2Pos;
                    if (dist.X > clientBounds.Width - p1.CollisionRect.Width - p2.CollisionRect.Width || dist.Y > clientBounds.Height - p1.CollisionRect.Height - p2.CollisionRect.Height || p1.TransitioningToLevel != 0 || p2.TransitioningToLevel != 0)
                        split = true;
                    else cameraPos = (p2Pos + p1Pos) / 2;
                }
            }

            if (split)
            {
                var oldViewport = graphicsDevice.Viewport;
                Rectangle? pBounds = null;
                foreach (var pInfo in CurrentLevels.SelectMany(l => l.Players.Select(p => new { Player = p, Level = l })).OrderBy(p => p.Player.DisplayName))
                {
                    if (pBounds == null)
                        pBounds = new Rectangle(0, 0, clientBounds.Width / 2, clientBounds.Height);
                    else
                        pBounds = new Rectangle(clientBounds.Width / 2, 0, clientBounds.Width / 2, clientBounds.Height);

                    graphicsDevice.Viewport = new Viewport(pBounds.Value);

                    var transformMatrix = Matrix.CreateScale(new Vector3(1, 1, 0));
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, transformMatrix);

                    var lv = pInfo.Level;

                    if (pInfo.Player.TransitioningToLevel != 0)
                        SlideScreen(gameTime, spriteBatch, pBounds.Value, lv, pInfo.Player);
                    else
                    {
                        Vector2 camera = GetCameraPosition(pInfo.Player.CenterPosition, lv.Map.PixelSize, pBounds.Value);
                        lv.Draw(gameTime, spriteBatch, camera);
                    }

                    #region GUI Drawing
                    int guiSize = 100;
                    Rectangle GUIRect = new Rectangle(pBounds.Value.X, pBounds.Value.Y, pBounds.Value.Width, guiSize);
                    _gui.Draw(spriteBatch, GUIRect, new[] { pInfo.Player }, clientBounds);
                    #endregion GUI Drawing

                    //Title Card
                    if (_cards.ContainsKey(pInfo.Player))
                        _cards[pInfo.Player].Draw(gameTime, spriteBatch, pBounds.Value);

                    spriteBatch.End();
                }

                graphicsDevice.Viewport = oldViewport;
            }
            else
            {
                spriteBatch.Begin();
                var lv = CurrentLevels.First();
                var player = lv.Players.First();

                if (player.TransitioningToLevel == 0)
                {
                    Vector2 camera = GetCameraPosition(cameraPos ?? lv.Players.First().CenterPosition, lv.Map.PixelSize, clientBounds);
                    lv.Draw(gameTime, spriteBatch, camera);
                }
                else
                {
                    SlideScreen(gameTime, spriteBatch, clientBounds, lv, player);
                }

                #region GUI Drawing
                int guiSize = 100;
                Rectangle GUIRect = new Rectangle(clientBounds.X, clientBounds.Y, clientBounds.Width, guiSize);
                _gui.Draw(spriteBatch, GUIRect, Players, clientBounds);
                #endregion GUI Drawing

                //Title Card
                if(_cards.ContainsKey(player))
                    _cards[player].Draw(gameTime, spriteBatch, clientBounds);

                spriteBatch.End();
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
        static Vector2 GetCameraPosition(Vector2 basePosition, Point mapSize, Rectangle screenSize)
        {
            var camera = basePosition;
            var newX = basePosition.X;
            var newY = basePosition.Y;

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
            var newLevel = GetLevel(player.TransitioningToLevel);
            Vector2 camera = GetCameraPosition(player.CenterPosition, lv.Map.PixelSize, clientBounds);
            Vector2 camera2 = GetCameraPosition(player.CenterPosition, newLevel.Map.PixelSize, clientBounds);

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

                if (!_cards.ContainsKey(player))
                    _cards[player] = new TitleCard { CurrentTitle = newLevel.Title };
                else
                    _cards[player].CurrentTitle = newLevel.Title;

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

        internal static void CloneWaypoints(Entity p1, Entity p2)
        {
            StoredWaypoints.Add(p2, StoredWaypoints[p1].ToList());
        }
    }
}
