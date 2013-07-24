using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Characters;
using QuestForTheCrown2.Entities.Weapons;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace QuestForTheCrown2.Base
{
    [Serializable]
    public class PlayerState
    {
        /// <summary>
        /// Current Level Number.
        /// </summary>
        public int CurrentLevel { get; set; }

        /// <summary>
        /// Player Position
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// Containers
        /// </summary>
        public Dictionary<string, Container> Containers { get; set; }

        /// <summary>
        /// Player Weapons
        /// </summary>
        public List<string> Weapons { get; set; }

        /// <summary>
        /// Player's health.
        /// </summary>
        public Container Health
        {
            get { return Containers.GetOrDefault("Health"); }
        }

        /// <summary>
        /// Player's magic.
        /// </summary>
        public Container Magic
        {
            get { return Containers.GetOrDefault("Magic"); }
        }
    }

    [Serializable]
    public class GameState
    {
        /// <summary>
        /// Creation Date
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Last Play Date
        /// </summary>
        public DateTime LastPlayDate { get; set; }

        /// <summary>
        /// Current player state.
        /// </summary>
        public PlayerState Player { get; set; }
        
        /// <summary>
        /// Completed dungeons. 
        /// </summary>
        public List<string> DungeonsComplete { get; set; }

        /// <summary>
        /// Weapons allowed.
        /// </summary>
        public List<string> AllowWeapon { get; set; }

        /// <summary>
        /// Creates Game State.
        /// </summary>
        public GameState()
        {
            CreationDate = DateTime.Now;
            DungeonsComplete = new List<string>();
            AllowWeapon = new List<string>();
        }
    }

    public static class GameStateManager
    {
        #region Constants
        /// <summary>
        /// Save file.
        /// </summary>
        private const string SaveFile = "SavedGames.xml";
        #endregion Constants

        #region Constructors
        /// <summary>
        /// Creates Game State Manager and loads data.
        /// </summary>
        static GameStateManager()
        {
            LoadData();
        }
        #endregion Constructors

        /// <summary>
        /// Main game instance
        /// </summary>
        public static GameMain Parent { private get; set; }

        /// <summary>
        /// Gets current state instance.
        /// </summary>
        public static GameState CurrentState { get; private set; }

        /// <summary>
        /// Loads game data.
        /// </summary>
        public static List<GameState> LoadData()
        {
            List<GameState> allStates = new List<GameState>();

            try { allStates = Serialization.Load<List<GameState>>(SaveFile); }
            catch { }

            if (allStates == null)
                allStates = new List<GameState>();

            return allStates;
        }

        /// <summary>
        /// Save game data.
        /// </summary>
        public static void SaveData(int slot)
        {
            List<GameState> allStates = LoadData();
            
            if( slot == -1 ) allStates.Insert(0, CurrentState);
            else if( allStates.Count > slot )
            {
                allStates.Insert(slot, CurrentState);
            }

            allStates.Save(SaveFile);
        }

        public static void SaveDataOverwriting(GameState state)
        {
            List<GameState> allStates = LoadData();
            allStates.Remove( allStates.Where((s) => s.LastPlayDate == state.LastPlayDate).First());
            allStates.Add(CurrentState);

            allStates.Save(SaveFile);
        }

        /// <summary>
        /// Deletes save data.
        /// </summary>
        public static void DeleteAllSaves()
        {
            List<GameState> allStates = new List<GameState>();

            allStates.Save(SaveFile);
        }

        /// <summary>
        /// Selects Save Data
        /// </summary>
        /// <param name="state">Data instance</param>
        public static void SelectSaveData(GameState state)
        {
            CurrentState = state;
            state.LastPlayDate = DateTime.Now;
        }

        #region Save Data Creation Auxiliary Methods
        /// <summary>
        /// Gets player state.
        /// </summary>
        /// <param name="player">Player entity</param>
        /// <returns>The player state.</returns>
        public static PlayerState GetPlayerState(Entity player)
        {
            return new PlayerState
                        {
                            CurrentLevel = player.CurrentLevel,
                            Position = player.Position,
                            Containers = player.Containers,
                            Weapons = (player.Weapons?? Enumerable.Empty<Weapon>()).Select(w => w.GetType().Name).ToList()
                        };
        }

        /// <summary>
        /// Loads player state
        /// </summary>
        /// <param name="player">Player to be loaded.</param>
        public static void LoadPlayerState(Entity player)
        {
            var weaponFactory = new Dictionary<string, Func<Weapon>>
                            {
                                { "Sword", () => new Sword() },
                                { "Bow", () => new Bow() },
                                { "Boomerang", () => new Boomerang() },
                                { "FireWand", () => new FireWand() },
                            };

            var playerStatus = GameStateManager.CurrentState.Player;
            player.Position = playerStatus.Position;
            player.CurrentLevel = playerStatus.CurrentLevel;
            player.Containers = playerStatus.Containers;
            player.Weapons = playerStatus.Weapons.Distinct().Select(name =>
                {
                    var weapon = CreateWeapon(weaponFactory, name);
                    weapon.Parent = player;
                    return weapon;
                }).ToList();
        }

        /// <summary>
        /// Creates a weapon using a factory.
        /// </summary>
        /// <param name="entityFactory">Entity factory.</param>
        /// <param name="type">Entity type (string)</param>
        /// <returns></returns>
        static Weapon CreateWeapon(Dictionary<string, Func<Weapon>> entityFactory, string type)
        {
            if (!entityFactory.ContainsKey(type))
                return null;

            return entityFactory[type]();
        }

        public static void CallSaveScreen()
        {
            if( Parent != null )
            {
                Parent.ChangeState(QuestForTheCrown2.GameState.Saving);
            }
        }
        #endregion Save Data Creation Auxiliary Methods
    }
}
