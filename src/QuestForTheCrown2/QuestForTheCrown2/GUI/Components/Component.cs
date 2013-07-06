using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.GUI.Components
{
    public class Component
    {
        #region Properties
        /// <summary>
        /// Is the component currently selected?
        /// </summary>
        public bool Selected { get; set; }
        /// <summary>
        /// Component Position.
        /// </summary>
        public Rectangle Position { get; set; }
        /// <summary>
        /// Selected texture.
        /// </summary>
        public Texture2D SelectedTexture { get; set; }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Creates component and initializes the texture with a default value.
        /// </summary>
        public Component()
        {
            SelectedTexture = GameContent.LoadContent<Texture2D>("images/menuselected.png");
        }
        #endregion Constructor
        
        #region Methods
        /// <summary>
        /// Draw the component
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if( Selected ) spriteBatch.Draw(SelectedTexture, Position, Color.White);
        }
        #endregion Methods
    }
}
