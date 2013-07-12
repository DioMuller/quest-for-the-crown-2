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
    public class GameState
    {
        public Player Player { get; set; }
        public List<string> DungeonsComplete { get; private set; }
        public List<string> AllowWeapon { get; private set; }

        public GameState()
        {
            DungeonsComplete = new List<string>();
            AllowWeapon = new List<string>();
        }
    }

    public class GameStateManager
    {
        private const string SaveFile = "SavedGames.xml";

        private static int _currentState;

        public static List<GameState> AllStates { get; private set; }
        public static GameState CurrentState 
        {
            get
            {
                if (_currentState < 0) return null;
                return AllStates[_currentState];
            }
        }

        public static void LoadData()
        {
            AllStates = new List<GameState>();
            _currentState = 0;

            using (IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            {
                if (!store.FileExists(SaveFile))
                {
                    store.CreateFile(SaveFile);
                }
                else
                {
                    try
                    {
                        string content;

                        using (StreamReader sr = new StreamReader(store.OpenFile(SaveFile, FileMode.Open)))
                        {
                            content = sr.ReadToEnd();
                        }

                        XDocument doc = XDocument.Parse(content);
                        XElement root = doc.Element("saves");
                        
                        foreach (XElement save in root.Elements("save"))
                        {
                            #region Load Player
                            XElement playerElement = save.Element("Player");
                            Player player = new Player()
                            {
                                Position = new Vector2(float.Parse(playerElement.Attribute("X").Value),float.Parse(save.Element("Player").Attribute("Y").Value)),
                                Health = new Container(
                                    quantity: int.Parse(playerElement.Attribute("Health").Value),
                                    maximum: int.Parse(playerElement.Attribute("MaxHealth").Value)),
                            };
                            #endregion Load Player

                            #region Load Player Weapons
                            var weaponFactory = new Dictionary<string, Func<Weapon>>
                            {
                                { "Sword", () => new Sword() },
                                { "Bow", () => new Bow() },
                                { "Boomerang", () => new Boomerang() },
                                { "FireWand", () => new FireWand() },
                            };

                            var entities = playerElement.Element("Weapons")
                                .Elements("Weapon")
                                .Select(n => CreateWeapon(weaponFactory, n));

                            player.Weapons.AddRange(entities);
                            #endregion Load Player Weapons

                            GameState state = new GameState();
                        }
                    }
                    catch
                    {
                        store.CreateFile(SaveFile);
                        AllStates = new List<GameState>();
                        _currentState = 0;
                    }
                }
            }
        }

        static Weapon CreateWeapon(Dictionary<string, Func<Weapon>> entityFactory, XElement node)
        {
            string type = node.Attribute("Name").Value;

            if (!entityFactory.ContainsKey(type))
                return null;

            return entityFactory[type]();
        }
    }
}
