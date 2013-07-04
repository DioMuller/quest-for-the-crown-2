using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels.Mapping;

namespace QuestForTheCrown2.Entities.Weapons
{
    abstract class Weapon
    {
        /// <summary>
        /// The entity in which this weapon is attached and will operate on.
        /// </summary>
        public Entity Entity { get; set; }

        /// <summary>
        /// Perform the weapon attack.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="map">Current entity map.</param>
        /// <param name="direction">The direction of the attack.</param>
        public abstract void Attack(GameTime gameTime, Map map, Vector2 direction);
    }
}
