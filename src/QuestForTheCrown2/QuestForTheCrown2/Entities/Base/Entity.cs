using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestForTheCrown2.Levels.Mapping;
using QuestForTheCrown2.Base;

namespace QuestForTheCrown2.Entities.Base
{
    class Entity
    {
        #region Attributes
        int _currentFrameIndex;
        readonly int _framesPerLine;
        Animation _lastAnimation;
        TimeSpan _lastFrameStartTime;
        #endregion

        #region Properties
        public Entity Parent { get; set; }
        public Vector2 Position { get; set; }
        public SpriteSheet SpriteSheet { get; private set; }
        public Vector2 Speed { get; set; }

        /// <summary>
        /// Entity size
        /// </summary>
        public Point Size
        {
            get
            {
                return SpriteSheet.FrameSize;
            }
        }

        public Dictionary<string, List<EntityUpdateBehavior>> Behaviors { get; private set; }

        /// <summary>
        /// Indicates the current entity animation.
        /// Examples are: "walking", "stopped" and "running".
        /// </summary>
        public string CurrentAnimation { get; set; }

        /// <summary>
        /// Indicates the current animation view.
        /// Examples are: "left", "bottom", "right" and "down".
        /// </summary>
        public string CurrentView { get; set; }
        #endregion

        #region Constructors
        public Entity(string spriteSheetPath, Point frameSize)
        {
            SpriteSheet = new SpriteSheet(GameContent.LoadContent<Texture2D>(spriteSheetPath), frameSize);
            _framesPerLine = SpriteSheet.Texture.Width / SpriteSheet.FrameSize.X;

            Speed = new Vector2(frameSize.X, frameSize.Y);
        }
        #endregion

        public Frame CurrentFrame
        {
            get
            {
                Animation animation = SelectAnimation();
                var index = animation.FrameIndexes[_currentFrameIndex];

                return new Frame
                {
                    Texture = SpriteSheet.Texture,
                    Rectangle = new Rectangle(
                        (index % _framesPerLine) * SpriteSheet.FrameSize.X,
                        (index / _framesPerLine) * SpriteSheet.FrameSize.Y,
                        SpriteSheet.FrameSize.X,
                        SpriteSheet.FrameSize.Y)
                };
            }
        }

        /// <summary>
        /// Selects the best animation based on the current view and animation name.
        /// </summary>
        /// <returns>The animation matching the entity's current view and animation</returns>
        Animation SelectAnimation()
        {
            Animation bestAnimation;
            Dictionary<string, Animation> bestAnimations;

            if (CurrentAnimation == null || !SpriteSheet.Animations.ContainsKey(CurrentAnimation))
                CurrentAnimation = SpriteSheet.Animations.First().Key;
            bestAnimations = SpriteSheet.Animations[CurrentAnimation];

            if (CurrentView == null || !bestAnimations.ContainsKey(CurrentView))
                CurrentView = bestAnimations.First().Key;
            bestAnimation = bestAnimations[CurrentView];

            return bestAnimation;
        }

        /// <summary>
        /// Attach update methods to this entity.
        /// </summary>
        /// <param name="behaviors">Behaviors to be executed during this entity's update method.</param>
        public void AddBehavior(params EntityUpdateBehavior[] behaviors)
        {
            if (Behaviors == null)
                Behaviors = new Dictionary<string, List<EntityUpdateBehavior>>();

            foreach (var behavior in behaviors)
            {
                var group = behavior.Group ?? string.Empty;
                behavior.Entity = this;
                if (Behaviors.ContainsKey(group))
                    Behaviors[group].Add(behavior);
                else
                    Behaviors[group] = new List<EntityUpdateBehavior> { behavior };
            }
        }

        #region Update
        public virtual void Update(GameTime gameTime, Map map)
        {
            if (Behaviors != null)
            {
                foreach (var behaviorGroup in Behaviors)
                {
                    if (behaviorGroup.Key == string.Empty)
                        foreach (var behavior in behaviorGroup.Value.Where(b => b.Active))
                            behavior.Update(gameTime, map);
                    else
                    {
                        var bh = behaviorGroup.Value.Where(b => b.Active).FirstOrDefault();
                        if (bh != null)
                            bh.Update(gameTime, map);
                    }
                }
            }

            var curAnimation = SelectAnimation();
            if (curAnimation != _lastAnimation)
            {
                _lastFrameStartTime = gameTime.TotalGameTime;
                _lastAnimation = curAnimation;
                _currentFrameIndex = 0;
            }
            else if (gameTime.TotalGameTime > _lastFrameStartTime + curAnimation.FrameDuration)
            {
                _currentFrameIndex = (_currentFrameIndex + 1) % curAnimation.FrameIndexes.Length;
                _lastFrameStartTime = gameTime.TotalGameTime;
            }
        }
        #endregion
    }
}
