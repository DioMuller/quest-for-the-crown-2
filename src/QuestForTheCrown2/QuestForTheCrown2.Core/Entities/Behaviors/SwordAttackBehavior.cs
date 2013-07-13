using System.Linq;
using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using QuestForTheCrown2.Levels.Mapping;
using QuestForTheCrown2.Entities.Weapons;
using System;

namespace QuestForTheCrown2.Entities.Behaviors
{
    /// <summary>
    /// A behavior that moves the entity in the direction of another entity.
    /// </summary>
    class SwordAttackBehavior : WalkBehavior
    {
        #region Attributes
        Sword _sword;
        FollowBehavior _followBehavior;
        TimeSpan _lastAttackTime, _timeBetweenAttacks = TimeSpan.FromSeconds(2);

        public float? MaxDistance
        {
            get { return _followBehavior.MaxDistance; }
            set { _followBehavior.MaxDistance = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the follow behavior with the desired follow entity and the distance to keep.
        /// </summary>
        /// <param name="following">The entity to follow.</param>
        /// <param name="distance">The desired distance to keep.</param>
        public SwordAttackBehavior(string targetCategory)
        {
            _followBehavior = new FollowBehavior(targetCategory, distance: 24);
        }
        #endregion

        #region Behavior
        /// <summary>
        /// Indicates if this behavior is set to follow one Entity.
        /// </summary>
        public override bool IsActive(GameTime gameTime, Level level)
        {
            _sword = Entity.Weapons.OfType<Sword>().FirstOrDefault();
            _followBehavior.Entity = Entity;

            return _sword != null &&
                   _followBehavior.IsActive(gameTime, level);
        }

        /// <summary>
        /// Moves the entity into the direction of the currently Following entity.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="map">Current entity map.</param>
        public override void Update(GameTime gameTime, Level level)
        {
            Entity.ChangeWeapon(_sword, level);

            if (_followBehavior.CurrentTarget.Distance < 36 && _lastAttackTime + _timeBetweenAttacks < gameTime.TotalGameTime)
            {
                _sword.Attack(gameTime, level, true, _followBehavior.CurrentTarget.Position);
                _sword.Attack(gameTime, level, false, Vector2.Zero);
                _lastAttackTime = gameTime.TotalGameTime;
            }
            else
                _followBehavior.Update(gameTime, level);
        }
        #endregion
    }
}
