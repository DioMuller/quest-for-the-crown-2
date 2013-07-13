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
        public int CurrentLevel { get; set; }
        public Vector2 Position { get; set; }
        public Dictionary<string, Container> Containers { get; set; }
        public List<string> Weapons { get; set; }
    }

    [Serializable]
    public class GameState
    {
        public DateTime CreationDate { get; set; }
        public DateTime LastPlayDate { get; set; }

        public PlayerState Player { get; set; }
        public List<string> DungeonsComplete { get; set; }
        public List<string> AllowWeapon { get; set; }

        public GameState()
        {
            CreationDate = DateTime.Now;
            DungeonsComplete = new List<string>();
            AllowWeapon = new List<string>();
        }
    }

    public static class GameStateManager
    {
        static GameStateManager()
        {
            LoadData();
        }

        private const string SaveFile = "SavedGames.xml";

        private static int _currentState = -1;

        public static List<GameState> AllStates { get; private set; }
        public static GameState CurrentState
        {
            get
            {
                if (_currentState >= AllStates.Count) return null;
                return AllStates[_currentState];
            }
        }

        public static void LoadData()
        {
            try { AllStates = Serialization.Load<List<GameState>>(SaveFile); }
            catch { }

            if (AllStates == null)
                AllStates = new List<GameState>();
        }

        public static void SaveData()
        {
            AllStates.Save(SaveFile);
        }

        public static void DeleteAllSaves()
        {
            AllStates = new List<GameState>();
        }

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

        public static void SelectSaveData(int id)
        {
            _currentState = id;
            var state = CurrentState;
            if (state == null)
                throw new ArgumentOutOfRangeException("Specified save state does not exists");
            state.LastPlayDate = DateTime.Now;
        }

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

        static Weapon CreateWeapon(Dictionary<string, Func<Weapon>> entityFactory, string type)
        {
            if (!entityFactory.ContainsKey(type))
                return null;

            return entityFactory[type]();
        }
    }
}
