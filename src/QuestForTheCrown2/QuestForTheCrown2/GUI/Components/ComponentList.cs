using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.GUI.Components
{
    public class ComponentList
    {
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
        /// If the player can move.
        /// </summary>
        private int _timeout;
        /// <summary>
        /// Current selected option.
        /// </summary>
        private int _selectedOption;
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
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Creates the Component List.
        /// </summary>
        public ComponentList()
        {
            _components = new List<Component>();
            _input = new Input();

            _timeout = 0;
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Component update method.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        public void Update(GameTime gameTime)
        {
            if( _input.ConfirmButton )
            {
                if( _components.Count > 0 )
                {
                    _components[_selectedOption].Select();
                }
            }

            float y = _input.Movement.Y;
            if (y != 0 && _timeout <= 0)
            {
                _timeout = 200;

                if( y < 0f ) OptionUp();
                else OptionDown();
            }
            else
            {
                _timeout -= gameTime.ElapsedGameTime.Milliseconds;
            }
        }

        /// <summary>
        /// Draws the component list.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="spriteBatch">Sprite batch for drawing.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach( Component cp in _components )
            {
                cp.Draw(spriteBatch);
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
                _selectedOption = 0;
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
                int y = (_position.Height - _position.Y) / _components.Count;

                for (int i = 0; i < _components.Count; i++)
                {
                    _components[i].Position = new Rectangle(_position.X, _position.Y + (i * y), _position.Width, y);
                }
            }
        }

        /// <summary>
        /// Goes up an option.
        /// </summary>
        private void OptionUp()
        {
            if( _components.Count > 0 && _selectedOption > 0 )
            {
                _components[_selectedOption].Selected = false;
                _selectedOption--;
                _components[_selectedOption].Selected = true;
            }
        }

        /// <summary>
        /// Goes down an option.
        /// </summary>
        private void OptionDown()
        {
            if (_components.Count > 0 && _selectedOption < (_components.Count - 1) )
            {
                _components[_selectedOption].Selected = false;
                _selectedOption++;
                _components[_selectedOption].Selected = true;
            }
        }
        #endregion Methods
    }
}
