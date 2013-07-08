using System;
using Microsoft.Xna.Framework;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Levels.Mapping;
using QuestForTheCrown2.Levels;

namespace QuestForTheCrown2.Entities.Behaviors
{
    /// <summary>
    /// A behavior that controls the entity based on the current input status.
    /// </summary>
    class InputBehavior : WalkBehavior
    {
        #region Attributes
        Input _input;
        int _currentWeapon;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the walk behavior.
        /// </summary>
        /// <param name="input">Input that will control the entity's movement.</param>
        /// <param name="index">Player's input index.</param>
        public InputBehavior(InputType input, int index = 0)
        {
            _input = new Input(input, index);
        }
        #endregion

        #region Behavior
        /// <summary>
        /// Indicates if the current input is connected and active.
        /// </summary>
        public override bool IsActive(GameTime gameTime, Level level)
        {
            return _input.IsConnected;
        }

        /// <summary>
        /// Moves the entity based on the current input status.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="map">Current entity map.</param>
        public override void Update(GameTime gameTime, Level level)
        {
            Walk(gameTime, level, _input.Movement);
            Attack(gameTime, level, _input.Attack, _input.AttackDirection);
        }

        /// <summary>
        /// Tries to attack with the current weapon.
        /// </summary>
        /// <param name="direction">The attack direction.</param>
        void Attack(GameTime gameTime, Level level, float attackForce, Vector2 direction)
        {
            Entity.Weapons[_currentWeapon].Attack(gameTime, level, attackForce, direction);
        }
        #endregion
    }
}
