using Microsoft.Xna.Framework;
using QuestForTheCrown2.Entities.Base;
using QuestForTheCrown2.Levels;
using QuestForTheCrown2.Levels.Mapping;
using System;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.Entities.Behaviors
{
    /// <summary>
    /// A behavior that moves the entity in the direction of another entity.
    /// </summary>
    class TouchAndDodgeBehavior : WalkBehavior
    {
        double _safeDistanceRange = 5;
        bool _dodging = true;
        TimeSpan _startDodge;
        int? _forcedAngle;

        #region Properties
        public EntityRelativePosition CurrentTarget { get; private set; }

        /// <summary>
        /// A desired distance to keep from the entity.
        /// </summary>
        public float Distance { get; set; }

        public float? MaxDistance { get; set; }
        /// <summary>
        /// The entity category to be followed.
        /// </summary>
        public string TargetCategory { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the follow behavior with the desired follow entity and the distance to keep.
        /// </summary>
        /// <param name="following">The entity to follow.</param>
        /// <param name="distance">The desired distance to keep.</param>
        public TouchAndDodgeBehavior(string targetCategory, float distance = 32 * 3)
        {
            TargetCategory = targetCategory;
            Distance = distance;
        }
        #endregion

        #region Behavior
        /// <summary>
        /// Indicates if this behavior is set to follow one Entity.
        /// </summary>
        public override bool IsActive(GameTime gameTime, Level level)
        {
            if (Entity.IsDead)
                return false;

            CurrentTarget = level.EntityCloserTo(Entity, TargetCategory);
            if (CurrentTarget != null && (MaxDistance == null || CurrentTarget.Distance < MaxDistance))
                return true;

            Walk(gameTime, level, Vector2.Zero);
            return false;
        }

        /// <summary>
        /// Moves the entity into the direction of the currently Following entity.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="map">Current entity map.</param>
        public override void Update(GameTime gameTime, Level level)
        {
            Entity.ChangeWeapon(null, level);

            const float safeXMult = 3;

            if (_dodging && gameTime.TotalGameTime > _startDodge + TimeSpan.FromSeconds(4))
                _dodging = false;

            var entLocation = Entity.CenterPosition;
            var directRoute = CurrentTarget.Position;

            if (directRoute.X < 0)
                Entity.CurrentView = "left";
            else
                Entity.CurrentView = "right";
            Entity.CurrentAnimation = "walking";

            var safeVector = new Vector2(directRoute.X, directRoute.Y * safeXMult);
            var normalized = new Vector2(safeVector.X, safeVector.Y);
            normalized.Normalize();

            if (_dodging)
            {
                if (safeVector.Length() > Distance + _safeDistanceRange)
                {
                    _safeDistanceRange = 0.5;
                    Walk(gameTime, level, normalized);
                }
                else if (safeVector.Length() < Distance - _safeDistanceRange)
                {
                    _safeDistanceRange = 0.5;
                    Walk(gameTime, level, normalized * -1);
                }
                else if (Math.Abs(directRoute.Y) > 0.3 || (directRoute.X > 0 == (CurrentTarget.Entity.CurrentView == "left")))
                {
                    _safeDistanceRange = 1;
                    int angle;

                    if (CurrentTarget.Entity.CurrentView == "right")
                    {
                        if (CurrentTarget.Position.Y > entLocation.Y)
                            angle = -1;
                        else
                            angle = 1;
                    }
                    else
                    {
                        if (CurrentTarget.Position.Y > entLocation.Y)
                            angle = 1;
                        else
                            angle = -1;
                    }
                    if (_forcedAngle != null)
                        angle = _forcedAngle.Value;

                    var targetRoute = new Vector2(-normalized.X, -normalized.Y);
                    var rotated = targetRoute.Rotate(angle * MathHelper.ToRadians(30));
                    var rotatedSafe = new Vector2(rotated.X * Distance, rotated.Y * Distance);
                    var walkRoute = new Vector2(entLocation.X + rotatedSafe.X * safeXMult * 1.05f, entLocation.Y + rotatedSafe.Y * 1.05f) - entLocation;
                    normalized = new Vector2(walkRoute.X, walkRoute.Y);
                    normalized.Normalize();
                    if (!Walk(gameTime, level, normalized * -1))
                    {
                        _forcedAngle = -angle;
                        _dodging = false;
                    }
                }
                else
                {
                    _dodging = false;
                    _forcedAngle = null;
                }
            }
            else
            {
                if (!Walk(gameTime, level, normalized) && Math.Abs(directRoute.X) <= Distance)
                {
                    _dodging = true;
                    _startDodge = gameTime.TotalGameTime;
                }
            }
        }
        #endregion
    }
}
