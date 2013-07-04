using System;
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
            XDocument doc = XDocument.Load(path);
            XElement root = doc.Element("collection");

            foreach (XElement el in root.Element("levels").Elements("level"))
            {
                int id = int.Parse(el.Attribute("id").Value);
                int[] neighbors = (from string element in el.Attribute("neighbors").Value.Split(',') select int.Parse(element)).ToArray<int>();
                Level level = LoadMap(id, el.Attribute("path").Value);

                for( int i = 0; i < 4; i++ )
                {
                    level.SetNeighbor((Direction)i, neighbors[i]);
                }

                collection.AddLevel(level);
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
            XDocument doc = XDocument.Load(tmxFile);
            List<Entity> entities = new List<Entity>();

            #region Create Map
            XElement mapElement = doc.Element("map");
            string name = Path.GetFileName(tmxFile).Replace(".tmx", "");
            Point size = new Point(int.Parse(mapElement.Attribute("width").Value), int.Parse(mapElement.Attribute("height").Value));
            Point tileSize = new Point(int.Parse(mapElement.Attribute("tilewidth").Value), int.Parse(mapElement.Attribute("tileheight").Value));

            map = new Map(name, size, tileSize);
            #endregion Create Map

            #region Tilesets
            foreach( XElement set in mapElement.Elements("tileset") )
            {
                int firstgid = int.Parse(set.Attribute("firstgid").Value);
                string tilename = set.Attribute("name").Value;
                Point tilesSize = new Point(int.Parse(set.Attribute("tilewidth").Value), int.Parse(set.Attribute("tileheight").Value));
                XElement image = set.Element("image");
                string imageSource = image.Attribute("source").Value.Replace("../", ""); //Removes relative path (since we'll use Content)
                Point imageSize = new Point(int.Parse(image.Attribute("width").Value), int.Parse(image.Attribute("height").Value));

                Tileset tileset = new Tileset(firstgid, name, tilesSize,  imageSource, imageSize);

                #region Tiles
                foreach( XElement element in set.Elements("tile") )
                {
                    int tileid = int.Parse(element.Attribute("id").Value);
                    string[] terrain = element.Attribute("terrain").Value.Split(',');

                    tileset.Tiles[tileid].SetCollision(CollisionPosition.UpperLeft, int.Parse(terrain[0]));
                    tileset.Tiles[tileid].SetCollision(CollisionPosition.UpperRight, int.Parse(terrain[1]));
                    tileset.Tiles[tileid].SetCollision(CollisionPosition.DownLeft, int.Parse(terrain[2]));
                    tileset.Tiles[tileid].SetCollision(CollisionPosition.DownRight, int.Parse(terrain[3]));
                }
                #endregion Tiles

                map.Tilesets.Add(tileset);
            }
            #endregion Tilesets

            #region Layers
            foreach( XElement lay in mapElement.Elements("layer") )
            {
                string layerName = lay.Attribute("name").Value;
                Point layersize = new Point( int.Parse(lay.Attribute("width").Value), int.Parse(lay.Attribute("height").Value) );
                string csvdata = lay.Element("data").Value;

                Layer layer = new Layer(layerName, layersize, csvdata);
                map.Layers.Add(layer);
            }
            #endregion Layers

            #region Objects
            MainCharacter mainChar = null;

            foreach( XElement objs in mapElement.Elements("objectgroup") )
            {
                foreach( XElement obj in objs.Elements("object") )
                {
                    Entity entity;
                    int x = int.Parse(obj.Attribute("x").Value);
                    int y = int.Parse(obj.Attribute("y").Value);
                    string type = obj.Attribute("type").Value;

                    switch( type )
                    {
                        case "MainCharacter":
                            entity = new MainCharacter { Position = new Vector2(32 * 4, 32 * 4) };
                            entity.AddBehavior(
                                new InputBehavior(Base.InputType.Controller),
                                new InputBehavior(Base.InputType.Keyboard)
                            );
                            entity.AddWeapon(new Sword());

                            mainChar = entity as MainCharacter;
                            break;
                        case "Enemy":
                            entity = new Enemy1 { Position = new Vector2(32 * 8, 32 * 8) };
                            entity.AddBehavior(
                            new FollowBehavior { Following = mainChar }
                            );
                            break;
                        default:
                            entity = null;
                            break;
                    }

                    if( entity != null) entities.Add(entity);
                }
            }
            #endregion Objects

            //Updates collision map
            map.UpdateCollision();


            Level level = new Level(id, map);
            level.AddEntity(entities);
            return level;
        }
    }
}
