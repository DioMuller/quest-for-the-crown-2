using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.GUI.Components
{
    public delegate void ValueChangeDelegate();

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

        /// <summary>
        /// Scroll direction arrow.
        /// </summary>
        private Texture2D _arrow;
        #endregion Attributes

        #region Delegates
        public ValueChangeDelegate ValueChanged;
        #endregion Delegates

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
        public ComponentList()
        {
            _components = new List<Component>();
            _input = new Input();
            ComponentHeight = null;

            _timeout = 0;

            _arrow = GameContent.LoadContent<Texture2D>("images/arrow.png");
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
                    if( _components[_selectedOption].Select != null )
                    {
                        _components[_selectedOption].Select();
                    }
                }
            }

            float x = _input.Movement.X;
            float y = _input.Movement.Y;

            if (y != 0 && _timeout <= 0)
            {
                _timeout = 200;

                if( y < 0f ) OptionUp();
                else OptionDown();
                SoundManager.PlaySound("select");
            }
            else if (x != 0 && _timeout <= 0)
            {
                _timeout = 200;

                if( _components[_selectedOption].SelectionChanged != null )
                {
                    _components[_selectedOption].SelectionChanged( x );
                    if( ValueChanged != null ) ValueChanged();
                }

                SoundManager.PlaySound("select");
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
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, float transparency = 1.0f)
        {
            if( _currentStart != 0 ) spriteBatch.Draw(_arrow, new Rectangle(Position.Center.X, Position.Y + 16, 32, 32), null, Color.White, (float)Math.PI/2, new Vector2(16,16), SpriteEffects.None, 0f);
            if (_currentStart + _componentsOnScreen != _componentNumber) spriteBatch.Draw(_arrow, new Rectangle(Position.Center.X, Position.Y + Position.Height - 32, 32, 32), null, Color.White, (float)(-Math.PI/2), new Vector2(16, 16), SpriteEffects.None, 0f);

            for (int i = _currentStart; i < (_currentStart + _componentsOnScreen); i++ )
            {
                _components[i].Draw(spriteBatch, transparency);
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
                _selectedOption = 0;
                _componentNumber = 0;
                _componentsOnScreen = 0;
                _currentStart = 0;
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

            if (_selectedOption <= _currentStart)
            {
                _currentStart = _selectedOption;
                RecalculateSizes();
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

            if (_selectedOption >= (_currentStart + _componentsOnScreen) )
            {
                _currentStart = _selectedOption - _componentsOnScreen +1;
                RecalculateSizes();
            }
        }

        /// <summary>
        /// Gets component value.
        /// </summary>
        /// <param name="name">Component name</param>
        /// <returns>value</returns>
        public string GetValue(string name)
        {
            Component comp = _components.Where((c) => c.Name == name).FirstOrDefault();

            if( comp == null ) return String.Empty;
            if( comp is SelectionBox ) return ((SelectionBox) comp).SelectedOption; 
            else return comp.Name;
        }
        #endregion Methods
    }
}
