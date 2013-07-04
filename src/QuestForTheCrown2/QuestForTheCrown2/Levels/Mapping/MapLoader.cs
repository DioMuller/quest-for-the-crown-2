using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace QuestForTheCrown2.Levels.Mapping
{
    /// <summary>
    /// Class responsible to loading the maps
    /// </summary>
    public static class MapLoader
    {
        /// <summary>
        /// Loads map from the Tiled "tmx" file.
        /// </summary>
        /// <param name="tmxFile">TMX File path.</param>
        /// <returns>Loaded map.</returns>
        public static Map LoadMap(string tmxFile)
        {
            Map map = null;
            XDocument doc = XDocument.Load(tmxFile);

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
                    int id = int.Parse(element.Attribute("id").Value);
                    string[] terrain = element.Attribute("terrain").Value.Split(',');
                    
                    tileset.Tiles[id].SetCollision(CollisionPosition.UpperLeft, int.Parse(terrain[0]));
                    tileset.Tiles[id].SetCollision(CollisionPosition.UpperRight, int.Parse(terrain[1]));
                    tileset.Tiles[id].SetCollision(CollisionPosition.DownLeft, int.Parse(terrain[2]));
                    tileset.Tiles[id].SetCollision(CollisionPosition.DownRight, int.Parse(terrain[3]));
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

            //Updates collision map
            map.UpdateCollision();

            return map;
        }
    }
}
