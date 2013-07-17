using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Base
{
    public class SpriteSheet
    {
        public Texture2D Texture { get; private set; }
        public Dictionary<string, Dictionary<string, Animation>> Animations { get; private set; }
        public Point FrameSize { get; private set; }

        public SpriteSheet(Texture2D texture, Point? frameSize)
        {
            if (frameSize != null)
            {
                if (texture.Width % frameSize.Value.X != 0 ||
                    texture.Height % frameSize.Value.Y != 0)
                    throw new InvalidOperationException("Texture size is not multiple of the frame size");
            }

            Texture = texture;
            FrameSize = frameSize ?? new Point(texture.Width, texture.Height);

            Animations = new Dictionary<string, Dictionary<string, Animation>>();

            if (frameSize == null)
                AddAnimation("default", "default", new int[] { 0 }, TimeSpan.Zero);
        }

        public void AddAnimation(string name, string view, int[] frameIndexes, TimeSpan frameDuration, bool repeat = true)
        {
            AddAnimation(name, view, new Animation(frameIndexes, frameDuration, repeat ? 0 : frameIndexes.Length - 1));
        }

        public void AddAnimation(string name, string view, int line, TimeSpan frameDuration, bool repeat = true)
        {
            AddAnimation(name, view, line, Texture.Width / FrameSize.X, frameDuration, repeat);
        }

        public void AddAnimation(string name, string view, int line, int count, TimeSpan frameDuration, bool repeat = true)
        {
            var startIndex = (Texture.Width / FrameSize.X) * line;
            var indexes = Enumerable.Range(startIndex, count).ToArray();

            AddAnimation(name, view, indexes, frameDuration, repeat);
        }

        public void AddAnimation(string name, string view, Animation animation)
        {
            Dictionary<string, Animation> existingAnimations;
            if (!Animations.TryGetValue(name, out existingAnimations))
            {
                existingAnimations = new Dictionary<string, Animation>();
                Animations.Add(name, existingAnimations);
            }

            existingAnimations.Add(view, animation);
        }
    }

    public class Animation
    {
        public int[] FrameIndexes { get; private set; }
        public TimeSpan FrameDuration { get; private set; }
        public int ResetToFrame { get; private set; }

        public Animation(int[] frameIndexes, TimeSpan frameDuration, int resetToFrame = 0)
        {
            FrameIndexes = frameIndexes;
            FrameDuration = frameDuration;
            ResetToFrame = resetToFrame;
        }
    }
}
