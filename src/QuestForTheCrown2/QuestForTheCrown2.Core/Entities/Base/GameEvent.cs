using Microsoft.Xna.Framework;
using QuestForTheCrown2.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Base
{
    class GameEventArgs : EventArgs
    {
        public GameEventArgs(GameTime gameTime, Level level)
        {
            GameTime = gameTime;
            Level = level;
        }

        public GameTime GameTime { get; private set; }
        public Level Level { get; private set; }
    }

    delegate void GameEventHandler(object sender, GameEventArgs e);
}
