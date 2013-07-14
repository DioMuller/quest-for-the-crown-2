using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using QuestForTheCrown2.Levels.Mapping;
using System;

namespace QuestForTheCrown2.Entities.Behaviors
{
    /// <summary>
    /// A behavior that moves the entity in the direction of another entity.
    /// </summary>
    class WalkAroundBehavior : WalkBehavior
    {
        Random _random;
        bool _walking;

        public float SpeedMultiplier { get; set; }
        Vector2 _currentWalkDirection;
        TimeSpan _startWalkTime;
        TimeSpan _maxStoppedTime;
        TimeSpan _lastStopTime;
        public TimeSpan MaxWalkingTime { get; set; }

        public TimeSpan MaxStoppedTime { get; set; }

        public WalkAroundBehavior()
        {
            MaxStoppedTime = TimeSpan.MaxValue;
            SpeedMultiplier = 0.3f;
        }

        #region Behavior
        public override bool IsActive(GameTime gameTime, Level level)
        {
            if (_random == null)
            {
                _random = new Random((int)(unchecked(Environment.TickCount + Entity.Position.X * Entity.Position.Y)));
                _maxStoppedTime = TimeSpan.FromSeconds(Math.Min(2 + _random.NextDouble() * 2, MaxStoppedTime.TotalSeconds));
                MaxWalkingTime = TimeSpan.FromSeconds(4 + _random.NextDouble() * 4);
            }

            return true;
        }

        public override void Update(GameTime gameTime, Level level)
        {
            if (_walking)
            {
                if (gameTime.TotalGameTime > _startWalkTime + MaxWalkingTime ||
                    !Walk(gameTime, level, _currentWalkDirection))
                {
                    _walking = false;
                    _lastStopTime = gameTime.TotalGameTime;
                    StopWalking(gameTime, level);
                }
            }
            else
            {
                if (gameTime.TotalGameTime > _lastStopTime + _maxStoppedTime)
                {
                    _walking = true;
                    _startWalkTime = gameTime.TotalGameTime;

                    switch (_random.Next(4))
                    {
                        case 0: _currentWalkDirection = new Vector2(-SpeedMultiplier, 0); break;
                        case 1: _currentWalkDirection = new Vector2(0, -SpeedMultiplier); break;
                        case 2: _currentWalkDirection = new Vector2(SpeedMultiplier, 0); break;
                        case 3: _currentWalkDirection = new Vector2(0, SpeedMultiplier); break;
                    }
                }
            }
        }
        #endregion
    }
}
