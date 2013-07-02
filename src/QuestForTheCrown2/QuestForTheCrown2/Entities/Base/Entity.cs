using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Base
{
    class Frame
    {
        public Texture2D Texture { get; set; }
        public Rectangle Rectangle { get; set; }
    }

    class Entity
    {
        #region Attributes
        int _currentFrameIndex;
        readonly int _framesPerLine;
        #endregion

        #region Properties
        public Entity Parent { get; set; }
        public Vector2 Location { get; set; }
        public SpriteSheet SpriteSheet { get; private set; }

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

        #region Update
        public virtual void Update(GameTime gameTime)
        {

        }
        #endregion
    }
}
