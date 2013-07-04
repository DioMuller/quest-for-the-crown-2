using Microsoft.Xna.Framework;
using QuestForTheCrown2.Base;
using QuestForTheCrown2.Levels.Mapping;

namespace QuestForTheCrown2.Entities.Behaviors
{
    /// <summary>
    /// A behavior that moves the entity based on the current input status.
    /// </summary>
    class InputWalkBehavior : WalkBehavior
    {
        #region Attributes
        Input _input;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the walk behavior.
        /// </summary>
        /// <param name="input">Input that will control the entity's movement.</param>
        /// <param name="index">Player's input index.</param>
        public InputWalkBehavior(InputType input, int index = 0)
        {
            _input = new Input(input, index);
        }
        #endregion

        #region Behavior
        /// <summary>
        /// Indicates if the current input is connected and active.
        /// </summary>
        public override bool Active
        {
            get { return _input.IsConnected; }
        }

        /// <summary>
        /// Moves the entity based on the current input status.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="map">Current entity map.</param>
        public override void Update(GameTime gameTime, Map map)
        {
            Walk(gameTime, map, _input.Movement);
        }
        #endregion
    }
}
