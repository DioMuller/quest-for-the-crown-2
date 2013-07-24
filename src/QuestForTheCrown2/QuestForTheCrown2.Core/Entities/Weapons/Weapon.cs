using Microsoft.Xna.Framework;
using QuestForTheCrown2.Base;
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

        public override void Update(GameTime gameTime, Level level)
        {
            // If Weapon has no owner!
            if (Parent == null)
            {
                var allowedWeapons = GameStateManager.CurrentState.AllowWeapon;
                var currentWeapons = GameStateManager.CurrentState.Player.Weapons;

                var weaponName = GetType().Name;

                // Check if weapon is not allowed
                if (!allowedWeapons.Contains(weaponName) || currentWeapons.Contains(weaponName))
                    IsInvisible = true;
                else
                {
                    IsInvisible = false;
                    Parent = level.CollidesWith(CollisionRect).FirstOrDefault(e => e.Category == "Player");
                    if (Parent != null)
                    {
                        currentWeapons.Add(weaponName);

                        Parent.AddWeapon(this);
                        level.RemoveEntity(this);
                        OverlapEntities = true;
                    }
                    return;
                }
            }

            base.Update(gameTime, level);
        }

        /// <summary>
        /// Called when the weapon is equiped.
        /// </summary>
        public virtual void Equiped(Level level) { }

        /// <summary>
        /// Called when the weapon is unequiped.
        /// </summary>
        public virtual void Unequiped(Level level)
        {
            level.RemoveEntity(this);
        }
    }
}
