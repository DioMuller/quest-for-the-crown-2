using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.GUI.Components
{
    class ScrollingList
    {
        #region Constants
        private const int SlideTime = 32;
        #endregion Constants

        #region Attributes
        /// <summary>
        /// Component size and position.
        /// </summary>
        private Rectangle _position;
        /// <summary>
        /// Controller/Keyboard input.
        /// </summary>
        private Input _input;
        /// <summary>
        /// Components on the list.
        /// </summary>
        private List<Component> _components;
        /// <summary>
        /// Total number of components
        /// </summary>
        private int _componentNumber;
        /// <summary>
        /// Number of components on screen (floored)
        /// </summary>
        private int _componentsOnScreen;
        /// <summary>
        /// Current initial component.
        /// </summary>
        private int _currentStart;

        private int _offset;
        private int _timeToChange;
        #endregion Attributes

        #region Properties

        /// <summary>
        /// Component position and size.
        /// </summary>
        public Rectangle Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;

                RecalculateSizes();
            }
        }

        public int? ComponentHeight { get; set; }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Creates the Component List.
        /// </summary>
        public ScrollingList()
        {
            _components = new List<Component>();
            _input = new Input();
            ComponentHeight = null;

            _timeToChange = SlideTime;
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Component update method.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        public void Update(GameTime gameTime)
        {
            _timeToChange -= gameTime.ElapsedGameTime.Milliseconds;

            if( _timeToChange <= 0 )
            {
                _offset += 1;
                _timeToChange = SlideTime;
                RecalculateSizes();
            }

            if( _offset >= ComponentHeight )
            {
                _offset = 0;
                _currentStart++;
                RecalculateSizes();
            }

            if( _currentStart >= _components.Count )
            {
                _currentStart = 0;
                RecalculateSizes();
            }
        }

        /// <summary>
        /// Draws the component list.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="spriteBatch">Sprite batch for drawing.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = _currentStart; i < (_currentStart + _componentsOnScreen); i++ )
            {
                _components[i].Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Adds component to the list.
        /// </summary>
        /// <param name="component"></param>
        public void AddComponent(Component component)
        {
            _components.Add(component);

            if( _components.Count == 1 )
            {
                _currentStart = 0;
                component.Selected = true;
            }
            
            RecalculateSizes();
        }

        /// <summary>
        /// Recalculates the component sizes.
        /// </summary>
        private void RecalculateSizes()
        {
            if (_components.Count > 0)
            {
                int y = ComponentHeight != null ? ComponentHeight.GetValueOrDefault() : (_position.Height - _position.Y) / _components.Count;
                int diff = ComponentHeight != null ? 32 : 0;

                //_selectedOption = 0;
                _componentNumber = _components.Count;
                _componentsOnScreen = y != 0 ? Math.Min( ((this.Position.Height - (2*diff))/ y), _componentNumber) : 0;
                //_currentStart = 0;



                for (int i = _currentStart; i < (_currentStart + _componentsOnScreen); i++)
                {
                    _components[i].Position = new Rectangle(_position.X, diff + _position.Y + ((i - _currentStart) * y), _position.Width, y);
                }
            }
            else
            {
                _componentNumber = 0;
                _componentsOnScreen = 0;
                _currentStart = 0;
            }
        }
        #endregion Methods
    }
}
