using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace QuestForTheCrown2.External.Tiled
{
    /// <summary>
    /// Map layer.
    /// </summary>
    public class Layer
    {
        #region Attributes
        /// <summary>
        /// Layer data
        /// </summary>
        private int[,] _data;
        #endregion Attributes

        #region Properties
        /// <summary>
        /// Layer Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Layer Width
        /// </summary>
        public Point Size { get; private set; }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Creates layer from the layer data
        /// </summary>
        /// <param name="name">Layer name</param>
        /// <param name="width">Layer width</param>
        /// <param name="height">Layer height</param>
        /// <param name="csvdata">CSV Data</param>
        public Layer(string name, Point size, string csvdata)
        {
            Name = name;
            Size = size;

            _data = new int[Size.X, Size.Y];
            string[] separated = csvdata.Split(',');

            if (separated.Length != (Size.X * Size.Y)) throw new InvalidOperationException("The csv data size is different from the Layer data size");

            for( int i = 0; i < separated.Length; i++ )
            {
                _data[i % Size.X, i / Size.X] = int.Parse(separated[i]);
            }
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Get data from a specific point of the layer.
        /// </summary>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <returns></returns>
        public int GetData(int x, int y)
        {
            return _data[x,y];
        }
        #endregion Methods
    }
}
