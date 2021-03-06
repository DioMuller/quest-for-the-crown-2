﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Behaviors;
using QuestForTheCrown2.Entities.Characters;
using QuestForTheCrown2.Entities.Objects;
using QuestForTheCrown2.Entities.Weapons;

namespace QuestForTheCrown2.Levels.Mapping
{
    /// <summary>
    /// Class responsible to loading the maps
    /// </summary>
    public static class MapLoader
    {
        /// <summary>
        /// Load Level Collection.
        /// </summary>
        /// <param name="path">QFC file path</param>
        /// <returns></returns>
        public static LevelCollection LoadLevels(string path)
        {
            LevelCollection collection = new LevelCollection();
            
            using( var stream = TitleContainer.OpenStream(path) )
            {    
                XDocument doc = XDocument.Load(stream);
                XElement root = doc.Element("collection");

                #region Load Levels
                foreach (XElement el in root.Element("levels").Elements("level"))
                {
                    int id = int.Parse(el.Attribute("id").Value);
                    int[] neighbors = (from string element in el.Attribute("neighbors").Value.Split(',') select int.Parse(element)).ToArray<int>();
                    Level level = LoadMap(id, el.Attribute("path").Value);
                    level.BGM = el.Attribute("music").Value;
                    level.Title = el.Attribute("title").Value;

                    for (int i = 0; i < 4; i++)
                    {
                        level.SetNeighbor((Direction)i, neighbors[i]);
                    }

                    collection.AddLevel(level);
                }
                #endregion Load Levels
            }

            return collection;
        }

        /// <summary>
        /// Loads map from the Tiled "tmx" file.
        /// </summary>
        /// <param name="tmxFile">TMX File path.</param>
        /// <returns>Loaded map.</returns>
        private static Level LoadMap(int id, string tmxFile)
        {
            Map map = null;
            List<Entity> entities = new List<Entity>();

            using( var stream = TitleContainer.OpenStream(tmxFile) )
            {   
                XDocument doc = XDocument.Load(stream);

                #region Create Map
                XElement mapElement = doc.Element("map");
                string name = Path.GetFileName(tmxFile).Replace(".tmx", "");
                Point size = new Point(int.Parse(mapElement.Attribute("width").Value), int.Parse(mapElement.Attribute("height").Value));
                Point tileSize = new Point(int.Parse(mapElement.Attribute("tilewidth").Value), int.Parse(mapElement.Attribute("tileheight").Value));

                map = new Map(name, size, tileSize);
                #endregion Create Map

                #region Tilesets
                foreach (XElement set in mapElement.Elements("tileset"))
                {
                    Tileset tileset;
                    int firstgid = int.Parse(set.Attribute("firstgid").Value.Replace("../", ""));

                    if (set.Attribute("source") == null)
                    {
                        string tilename = set.Attribute("name").Value;
                        Point tilesSize = new Point(int.Parse(set.Attribute("tilewidth").Value), int.Parse(set.Attribute("tileheight").Value));
                        XElement image = set.Element("image");
                        string imageSource = image.Attribute("source").Value.Replace("../", ""); //Removes relative path (since we'll use Content)
                        Point imageSize = new Point(int.Parse(image.Attribute("width").Value), int.Parse(image.Attribute("height").Value));

                        tileset = new Tileset(firstgid, tilename, tilesSize, imageSource, imageSize);

                        #region Tiles
                        foreach (XElement element in set.Elements("tile"))
                        {
                            int tileid = int.Parse(element.Attribute("id").Value);
                            string[] terrain = element.Attribute("terrain").Value.Split(',');

                            tileset.Tiles[tileid].SetCollision(CollisionPosition.UpperLeft, int.Parse(terrain[0]));
                            tileset.Tiles[tileid].SetCollision(CollisionPosition.UpperRight, int.Parse(terrain[1]));
                            tileset.Tiles[tileid].SetCollision(CollisionPosition.DownLeft, int.Parse(terrain[2]));
                            tileset.Tiles[tileid].SetCollision(CollisionPosition.DownRight, int.Parse(terrain[3]));
                        }
                        #endregion Tiles
                    }
                    else
                    {
                        tileset = LoadTileset(firstgid, "Content/" + set.Attribute("source").Value.Replace("../", ""));
                    }

                    map.Tilesets.Add(tileset);
                }

                map.UpdateTilesets();
                #endregion Tilesets

                #region Layers
                foreach (XElement lay in mapElement.Elements("layer"))
                {
                    string layerName = lay.Attribute("name").Value;
                    Point layersize = new Point(int.Parse(lay.Attribute("width").Value), int.Parse(lay.Attribute("height").Value));
                    string csvdata = lay.Element("data").Value;

                    Layer layer = new Layer(layerName, layersize, csvdata);
                    map.Layers.Add(layer);
                }
                #endregion Layers

                #region Objects
                var enemyFactory = new Dictionary<string, Func<Entity>>
                {
                    { "Crab", () => new Crab() },
                    { "Slime", () => new Slime() },
                    { "SlimeWorm", () => new SlimeWorm() },
                    { "FlameDragon", () => new FlameDragon() },
                    { "WaterDragon", () => new WaterDragon() },
                    { "Bat", () => new Bat() },
                    { "Zombie", () => new Zombie() },
                    { "Skelleton", () => new Skeleton() },
                    { "Goon", () => new Goon() },
                    { "MasterGoon", () => new MasterGoon() },
                    { "BoomerangSkeleton", () => new BoomerangSkeleton() },
                    { "Poltergeist", () => new Poltergeist() },
                    { "Knight", () => new Knight() },
                };
                var itemFactory = new Dictionary<string, Func<Entity>>
                {
                    { "Sword", () => new Sword() },
                    { "Bow", () => new Bow() },
                    { "Boomerang", () => new Boomerang() },
                    { "FireWand", () => new FireWand() },
                };

                var objectFactory = new Dictionary<string, Func<Entity>>
                {
                    { "Bush", () => new Bush() }
                };

                var entityFactory = new Dictionary<string, Func<string, Entity>>
                {
                    { "Player", n => new Player() },
                    { "Entrance", n => new Entrance(int.Parse(n)) },
                    { "SavePoint", n => new SavePoint() },

                    { "Item", n => itemFactory.ContainsKey(n)? itemFactory[n]() : new Item() },
                    { "Enemy", n => enemyFactory.ContainsKey(n)? enemyFactory[n]() : new Slime() },
                    { "Object", n => objectFactory.ContainsKey(n)? objectFactory[n]() : new Bush() }
                };

                entities = mapElement.Elements("objectgroup")
                                         .Elements("object")
                                         .Select(n => CreateEntity(entityFactory, id, n)).ToList();
                #endregion Objects
            }

            return new Level(id, map, entities);
        }

        static Entity CreateEntity(Dictionary<string, Func<string, Entity>> entityFactory, int levelId, XElement node)
        {
            var type = node.Attribute("type").Value;

            if (!entityFactory.ContainsKey(type))
                return null;

            var entity = entityFactory[type](node.Attribute("name").Value);
            entity.CurrentLevel = levelId;
            entity.Category = type;
            entity.Position = new Vector2(x: int.Parse(node.Attribute("x").Value),
                                          y: int.Parse(node.Attribute("y").Value));
            return entity;
        }

        private static Tileset LoadTileset(int firstgid, string tsxFile)
        {
            Tileset tileset;

            using (var stream = TitleContainer.OpenStream(tsxFile))
            {
                XDocument doc = XDocument.Load(stream);
                XElement set = doc.Element("tileset");

                string tilename = set.Attribute("name").Value;
                Point tilesSize = new Point(int.Parse(set.Attribute("tilewidth").Value), int.Parse(set.Attribute("tileheight").Value));
                XElement image = set.Element("image");
                string imageSource = image.Attribute("source").Value.Replace("../", ""); //Removes relative path (since we'll use Content)
                Point imageSize = new Point(int.Parse(image.Attribute("width").Value), int.Parse(image.Attribute("height").Value));
                tileset = new Tileset(firstgid, tilename, tilesSize, imageSource, imageSize);

                #region Tiles
                foreach (XElement element in set.Elements("tile"))
                {
                    int tileid = int.Parse(element.Attribute("id").Value);
                    string[] terrain = element.Attribute("terrain").Value.Split(',');

                    tileset.Tiles[tileid].SetCollision(CollisionPosition.UpperLeft, int.Parse(terrain[0]));
                    tileset.Tiles[tileid].SetCollision(CollisionPosition.UpperRight, int.Parse(terrain[1]));
                    tileset.Tiles[tileid].SetCollision(CollisionPosition.DownLeft, int.Parse(terrain[2]));
                    tileset.Tiles[tileid].SetCollision(CollisionPosition.DownRight, int.Parse(terrain[3]));
                }
                #endregion Tiles
            }

            return tileset;
        }
    }
}
