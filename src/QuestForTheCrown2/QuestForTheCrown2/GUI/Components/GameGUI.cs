using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Characters;

namespace QuestForTheCrown2.GUI.Components
{
    class GameGUI
    {
        private Rectangle _position;

        private Texture2D _background;
        private Texture2D _fullHealth;
        private Texture2D _emptyHealth;
        private Texture2D _partHealth;
        private SpriteFont _font;

        public GameGUI()
        {
            _background = GameContent.LoadContent<Texture2D>("gui/gui_background.png");
            _fullHealth = GameContent.LoadContent<Texture2D>("gui/health_full.png");
            _emptyHealth = GameContent.LoadContent<Texture2D>("gui/health_empty.png");
            _partHealth = GameContent.LoadContent<Texture2D>("gui/health_part.png");

            _font = GameContent.LoadContent<SpriteFont>("fonts/DefaultFont");
        }

        public void Draw(SpriteBatch spritebatch, Rectangle rectangle, IEnumerable<Entity> players)
        {
            List<Entity> list = players.ToList<Entity>();
            int width = rectangle.X / 4;

            //spritebatch.Draw(_background, rectangle, Color.White);

            for( int i = 0; i < list.Count(); i++ )
            {
                int maxhealth =  list[i].MaxHealth.GetValueOrDefault();
                int health = list[i].Health.GetValueOrDefault();
                int difference = Convert.ToInt32(_font.MeasureString( "Player " + (i+1) ).X) + 35;

                spritebatch.DrawString(_font, "Player " + (i+1), new Vector2( 20 + (i * width), 20), Color.White);

                #region Draw Health
                while( maxhealth > 0 )
                {
                    Rectangle rect = new Rectangle( difference, 25, 20, 20);

                        if( health >= 4 )
                        {
                            spritebatch.Draw( _fullHealth, rect, Color.White );
                        }
                        else
                        {
                            //TODO: Find a more elegant way to do this.
                            spritebatch.Draw(_emptyHealth, rect, Color.White);
                            if (health > 0) spritebatch.Draw(_partHealth, new Rectangle( rect.X, rect.Y, 10, 10), Color.White);
                            if (health > 1) spritebatch.Draw(_partHealth, new Rectangle(rect.X + 10, rect.Y, 10, 10), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f );
                            if (health > 2) spritebatch.Draw(_partHealth, new Rectangle(rect.X, rect.Y + 10, 10, 10), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
                        }
                        

                    difference += 30;
                    maxhealth -= 4;
                    health -= 4;
                }
                #endregion Draw Health
            }
            
        }
    }
}
