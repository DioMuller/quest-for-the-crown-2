using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using QuestForTheCrown2.Levels.Mapping;

namespace QuestForTheCrown2.Entities.Weapons
{
    public interface IWeapon
    {
        /// <summary>
        /// The entity in which this weapon is attached and will operate on.
        /// </summary>
        Entity Entity { get; set; }

        /// <summary>
        /// Perform the weapon attack.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="map">Current entity map.</param>
        /// <param name="direction">The direction of the attack.</param>
        void Attack(GameTime gameTime, Level level, Vector2 direction);
    }
}
