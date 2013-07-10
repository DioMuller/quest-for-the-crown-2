using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Weapons
{
    public abstract class Weapon : Entity
    {
        public Weapon(string spriteSheetPath, Point? frameSize)
            : base(spriteSheetPath, frameSize)
        {
        }

        /// <summary>
        /// Perform the weapon attack.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="level">Current entity level.</param>
        /// <param name="attackButton">True when the attack button is pressed.</param>
        /// <param name="direction">The direction of the attack.</param>
        public abstract void Attack(GameTime gameTime, Level level, bool attackButton, Vector2 direction);
    }
}
