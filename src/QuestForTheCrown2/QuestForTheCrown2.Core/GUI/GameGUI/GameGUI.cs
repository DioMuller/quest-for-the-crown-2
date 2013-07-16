using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Entities.Characters;
using QuestForTheCrown2.Entities.Weapons;

namespace QuestForTheCrown2.GUI.GameGUI
{
    class GameGUI
    {
        private Rectangle _position;

        private Texture2D _shadow;
        private Texture2D _magic;
        private Texture2D _fullHealth;
        private Texture2D _emptyHealth;
        private Texture2D _partHealth;
        private SpriteFont _font;

        public GameGUI()
        {
            _shadow = GameContent.LoadContent<Texture2D>("gui/gui_shadow.png");
            _magic = GameContent.LoadContent<Texture2D>("gui/gui_background.png");
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

            for (int i = 0; i < list.Count; i++)
            {
                //TODO: Change position depending on player.
                Rectangle bg_rect = new Rectangle(0,0, 360, 80);

                int maxhealth = list[i].Health.Maximum ?? list[i].Health;
                int health = list[i].Health;

                int maxmagic = list[i].Magic.Maximum ?? list[i].Magic;
                int magic = list[i].Magic;

                int original_difference = Convert.ToInt32(_font.MeasureString("Player " + (i + 1)).X) + 35;
                int difference = original_difference;

                spritebatch.Draw(_shadow, bg_rect, Color.White);
                spritebatch.DrawString(_font, "Player " + (i + 1), new Vector2(20 + (i * width), 20), Color.White);

                #region Draw Health
                while (maxhealth > 0)
                {
                    Rectangle rect = new Rectangle(difference, 25, 10, 10);

                    if (health >= 4)
                    {
                        spritebatch.Draw(_fullHealth, rect, Color.White);
                    }
                    else
                    {
                        //TODO: Find a more elegant way to do this.
                        spritebatch.Draw(_emptyHealth, rect, Color.Black);
                        if (health > 0) spritebatch.Draw(_partHealth, new Rectangle(rect.X, rect.Y, 5, 5), Color.White);
                        if (health > 1) spritebatch.Draw(_partHealth, new Rectangle(rect.X + 5, rect.Y, 5, 5), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
                        if (health > 2) spritebatch.Draw(_partHealth, new Rectangle(rect.X, rect.Y + 5, 5, 5), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
                    }

                    difference += 15;
                    maxhealth -= 4;
                    health -= 4;
                }
                #endregion Draw Health

                #region Draw Magic
                difference = original_difference;

                Rectangle rect_full = new Rectangle(difference, 40, 120, 10);
                Rectangle rect_life = new Rectangle(difference, 40, Convert.ToInt32(120 * ((float)magic / (float)maxmagic)), 10);

                spritebatch.Draw(_magic, rect_full, Color.Black);
                spritebatch.Draw(_magic, rect_life, Color.CornflowerBlue);

                #endregion Draw Magic

                #region Draw Weapon
                difference = original_difference + 130;
                Rectangle weapon_rect = new Rectangle( difference, 25, 30, 30);

                spritebatch.Draw(_magic, weapon_rect, Color.Black);
                if( list[i].CurrentWeapon != null )
                {
                    spritebatch.Draw(list[i].CurrentWeapon.CurrentFrame.Texture, weapon_rect, list[i].CurrentWeapon.CurrentFrame.Rectangle, Color.White);

                    if( list[i].CurrentWeapon is Bow )
                    {
                        spritebatch.DrawString(_font, list[i].Arrows.Quantity.ToString(), new Vector2(difference + 40, 20), Color.White);
                    }
                }
                #endregion Draw Weapon
            }
        }
    }
}
