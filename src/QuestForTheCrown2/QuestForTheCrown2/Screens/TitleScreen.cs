using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.Screens
{
    class TitleScreen
    {
        private Texture2D _logo;
        private Rectangle _logoPosition;

        public TitleScreen(Rectangle window)
        {
            _logo = GameContent.LoadContent<Texture2D>("images/logo.png");
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_logo, _logoPosition, Color.White);
        }

        private void UpdatePositions(Rectangle window)
        {
            _logoPosition = new Rectangle(Convert.ToInt32((window.Width - window.X) / 2f - (_logo.Width / 2)), Convert.ToInt32(0.3f * (window.Height - window.Y)), _logo.Width, _logo.Height);
        }
    }
}
