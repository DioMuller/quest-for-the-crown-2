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
        public static GameMain Parent { private get; set; }

        /// <summary>
        /// Creates Game State Manager and loads data.
        /// </summary>
        static GameStateManager()
        {
            LoadData();
        }

        /// <summary>
        /// Save file.
        /// </summary>
        private const string SaveFile = "SavedGames.xml";

        /// <summary>
        /// Current state index.
        /// </summary>
        private static int _currentState = -1;

        /// <summary>
        /// All saved states.
        /// </summary>
        public static List<GameState> AllStates { get; private set; }

        /// <summary>
        /// Gets current state instance.
        /// </summary>
        public static GameState CurrentState
        {
            get
            {
                if (_currentState >= AllStates.Count) return null;
                return AllStates[_currentState];
            }
        }

        /// <summary>
        /// Loads game data.
        /// </summary>
        public static void LoadData()
        {
            try { AllStates = Serialization.Load<List<GameState>>(SaveFile); }
            catch { }

            if (AllStates == null)
                AllStates = new List<GameState>();
        }

        /// <summary>
        /// Save game data.
        /// </summary>
        public static void SaveData()
        {
            AllStates.Save(SaveFile);
        }

        /// <summary>
        /// Deletes save data.
        /// </summary>
        public static void DeleteAllSaves()
        {
            AllStates = new List<GameState>();
        }

        /// <summary>
        /// Selects Save Data
        /// </summary>
        /// <param name="state">Data instance</param>
        public static void SelectSaveData(GameState state)
        {
            if (AllStates.Contains(state))
                _currentState = AllStates.IndexOf(state);
            else
            {
                _currentState = AllStates.Count;
                AllStates.Add(state);
            }
            state.LastPlayDate = DateTime.Now;
        }

        /// <summary>
        /// Selects Save Data by index.
        /// </summary>
        /// <param name="id">Data ID</param>
        public static void SelectSaveData(int id)
        {
            _currentState = id;
            var state = CurrentState;
            if (state == null)
                throw new ArgumentOutOfRangeException("Specified save state does not exists");
            state.LastPlayDate = DateTime.Now;
        }

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
                            Weapons = player.Weapons.Select(w => w.GetType().Name).ToList()
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

            player.CurrentLevel = playerStatus.CurrentLevel;
            player.Containers = playerStatus.Containers;
            player.Weapons = playerStatus.Weapons.Select(name =>
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
    }
}
