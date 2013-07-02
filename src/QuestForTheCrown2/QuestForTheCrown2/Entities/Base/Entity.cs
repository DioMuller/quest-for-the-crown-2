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
        int currentFrameIndex;
        #endregion

        #region Properties
        public Entity Parent { get; set; }
        public Vector2 Location { get; set; }
        public SpriteSheet SpriteSheet { get; private set; }

        public string CurrentView { get; set; }
        public string CurrentAnimation { get; set; }
        #endregion

        #region Constructors
        public Entity(SpriteSheet spriteSheet)
        {
            if (spriteSheet == null || !spriteSheet.Animations.Any() || !spriteSheet.Animations.First().Value.Any())
                throw new InvalidOperationException("The SpriteSheet content was not loaded");

            SpriteSheet = spriteSheet;

            CurrentAnimation = spriteSheet.Animations.First().Key;
            CurrentView = spriteSheet.Animations.First().Value[0].View;
        }
        #endregion

        public Frame CurrentFrame
        {
            get
            {
                var bestAnimation = SpriteSheet.Animations[CurrentAnimation].FirstOrDefault(a => a.View == CurrentView);

                var framesPerLine = SpriteSheet.Texture.Width / SpriteSheet.FrameSize.X;

                var index = bestAnimation.FrameIndexes[currentFrameIndex];

                return new Frame
                {
                    Texture = SpriteSheet.Texture,
                    Rectangle = new Rectangle(
                        (index % framesPerLine) * SpriteSheet.FrameSize.X,
                        (index / framesPerLine) * SpriteSheet.FrameSize.Y,
                        SpriteSheet.FrameSize.X,
                        SpriteSheet.FrameSize.Y)
                };
            }
        }

        #region Update
        public virtual void Update(GameTime gameTime)
        {

        }
        #endregion
    }
}
