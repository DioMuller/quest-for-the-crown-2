using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.External.Tiled
{
    class Layer
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
        string Name { get; private set; }
        /// <summary>
        /// Layer Width
        /// </summary>
        int Width { get; private set; }
        /// <summary>
        /// Layer Height
        /// </summary>
        int Height { get; private set; }
        #endregion Properties

        #region Constructor
        public Layer(string name, int width, int height, string csvdata)
        {
            Name = name;
            Width = width;
            Height = height;

            _data = new int[Width,Height];
            string[] separated = csvdata.Split(',');

            if( separated.Length != (Width * Height) ) throw new InvalidOperationException("The csv data size is different from the Layer data size");
        }
        #endregion Constructor
    }
}
