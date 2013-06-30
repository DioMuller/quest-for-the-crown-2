using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public int Width { get; private set; }

        /// <summary>
        /// Layer Height
        /// </summary>
        public int Height { get; private set; }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Creates layer from the layer data
        /// </summary>
        /// <param name="name">Layer name</param>
        /// <param name="width">Layer width</param>
        /// <param name="height">Layer height</param>
        /// <param name="csvdata">CSV Data</param>
        public Layer(string name, int width, int height, string csvdata)
        {
            Name = name;
            Width = width;
            Height = height;

            _data = new int[Width,Height];
            string[] separated = csvdata.Split(',');

            if( separated.Length != (Width * Height) ) throw new InvalidOperationException("The csv data size is different from the Layer data size");

            for( int i = 0; i < separated.Length; i++ )
            {
                _data[ i % Width, i / Width] = int.Parse(separated[i]);
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
