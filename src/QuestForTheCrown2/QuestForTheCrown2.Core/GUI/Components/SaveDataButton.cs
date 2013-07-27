using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.GUI.Components
{
    /// <summary>
    /// "Button"/Menu item component. Can be selected.
    /// </summary>
    public class SaveDataButton : Component
    {
        #region Attributes
        /// <summary>
        /// GUI white/transparent shadow.
        /// </summary>
        private Texture2D _shadow;
        /// <summary>
        /// Magic texture.
        /// </summary>
        private Texture2D _magic;
        /// <summary>
        /// Full health texture.
        /// </summary>
        private Texture2D _fullHealth;
        /// <summary>
        /// Empty health texture.
        /// </summary>
        private Texture2D _emptyHealth;
        /// <summary>
        /// Part health texture.
        /// </summary>
        private Texture2D _partHealth;
        #endregion Attributes

        #region Properties
        /// <summary>
        /// Label.
        /// </summary>
        public Base.GameState Data { get; set; }

        /// <summary>
        /// Font used.
        /// </summary>
        public SpriteFont Font { get; set; }
        #endregion Properties

        #region Constructor
        public SaveDataButton(string name, Base.GameState state, SelectDelegate select)
            : base(name)
        {
            _shadow = GameContent.LoadContent<Texture2D>("gui/gui_shadow.png");
            _magic = GameContent.LoadContent<Texture2D>("gui/gui_background.png");
            _fullHealth = GameContent.LoadContent<Texture2D>("gui/health_full.png");
            _emptyHealth = GameContent.LoadContent<Texture2D>("gui/health_empty.png");
            _partHealth = GameContent.LoadContent<Texture2D>("gui/health_part.png");

            Font = GameContent.LoadContent<SpriteFont>("fonts/DefaultFont");
            Data = state;
            Select += select;
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Draws the Component
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch, float transparency = 1.0f)
        {
            base.Draw(spriteBatch);
            Color fontColor = Selected ? Color.Black : Color.White;

            Rectangle bg_rect = new Rectangle(Position.X, Position.Y, 360, 80);

            int maxhealth = Data.Player.Health.Maximum ?? Data.Player.Health;
            int health = Data.Player.Health;

            int maxmagic = Data.Player.Magic.Maximum ?? Data.Player.Magic;
            int magic = Data.Player.Magic;

            int original_difference = Convert.ToInt32(Font.MeasureString("Player").X) + 35 + Position.X;
            int difference = original_difference;

            spriteBatch.Draw(_shadow, bg_rect, Color.White);
            spriteBatch.DrawString(Font, "Player", new Vector2(20 + Position.X, 20 + Position.Y), Color.White);

            #region Draw Health
            while (maxhealth > 0)
            {
                Rectangle rect = new Rectangle(difference, 25 + Position.Y, 10, 10);

                if (health >= 4)
                {
                    spriteBatch.Draw(_fullHealth, rect, Color.White);
                }
                else
                {
                    //TODO: Find a more elegant way to do this.
                    spriteBatch.Draw(_emptyHealth, rect, Color.Black);
                    if (health > 0) spriteBatch.Draw(_partHealth, new Rectangle(rect.X, rect.Y, 5, 5), Color.White);
                    if (health > 1) spriteBatch.Draw(_partHealth, new Rectangle(rect.X + 5, rect.Y, 5, 5), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
                    if (health > 2) spriteBatch.Draw(_partHealth, new Rectangle(rect.X, rect.Y + 5, 5, 5), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
                }

                difference += 15;
                maxhealth -= 4;
                health -= 4;
            }
            #endregion Draw Health

            #region Draw Magic
            if( magic != 0 )
            {
                difference = original_difference;

                Rectangle rect_full = new Rectangle(difference, 40 + Position.Y, 120, 10);
                Rectangle rect_life = new Rectangle(difference, 40 + Position.Y, Convert.ToInt32(120 * ((float)magic / (float)maxmagic)), 10);

                spriteBatch.Draw(_magic, rect_full, Color.Black);
                spriteBatch.Draw(_magic, rect_life, Color.CornflowerBlue);
            }

            #endregion Draw Magic

            #region Draw Weapons
            //TODO: Draw Weapons
            #endregion Draw Weapons

            string date = Data.LastPlayDate.ToString("MMM dd yyyy, hh:mm:ss").ToUpperInvariant();
            Vector2 fontSize = Font.MeasureString(date);
            Vector2 fontPosition = new Vector2(Position.X + Position.Width - fontSize.X, Position.Y + Position.Height - fontSize.Y); 
            spriteBatch.DrawString(Font, date, fontPosition, fontColor);
        }
        #endregion Methods
    }
}
