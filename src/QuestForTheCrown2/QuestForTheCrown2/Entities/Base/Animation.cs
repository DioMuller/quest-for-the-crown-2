using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Entities.Base
{
    class SpriteSheet
    {
        public Texture2D Texture { get; private set; }
        public Dictionary<string, List<Animation>> Animations { get; private set; }
        public Point FrameSize { get; private set; }

        public SpriteSheet(Texture2D texture, Point frameSize)
        {
            if (texture.Width % frameSize.X != 0 ||
                texture.Height % frameSize.Y != 0)
                throw new InvalidOperationException("Texture size is not multiple of the frame size");

            Texture = texture;
            FrameSize = frameSize;

            Animations = new Dictionary<string, List<Animation>>();
        }

        public void AddAnimation(string name, string view, int[] frameIndexes, TimeSpan frameDuration)
        {
            AddAnimation(name, new Animation(view, frameIndexes, frameDuration));
        }

        public void AddAnimation(string name, Animation animation)
        {
            List<Animation> existingAnimations;
            if (!Animations.TryGetValue(name, out existingAnimations))
            {
                existingAnimations = new List<Animation>();
                Animations.Add(name, existingAnimations);
            }

            existingAnimations.Add(animation);
        }

        public void AddAnimation(string name, string view, int line, TimeSpan frameDuration)
        {
            AddAnimation(name, view, line, Texture.Width / FrameSize.X, frameDuration);
        }

        public void AddAnimation(string name, string view, int line, int count, TimeSpan frameDuration)
        {
            var startIndex = (Texture.Width / FrameSize.X) * line;
            var indexes = Enumerable.Range(startIndex, count).ToArray();

            AddAnimation(name, view, indexes, frameDuration);
        }

        public void AddAnimations(string name, IEnumerable<Animation> animations)
        {
            foreach (var animation in animations)
                AddAnimation(name, animation);
        }
    }

    [DebuggerDisplay("{View}")]
    class Animation
    {
        public string View { get; private set; }

        public int[] FrameIndexes { get; private set; }

        public TimeSpan FrameDuration { get; private set; }

        public Animation(/*string name, */string view, int[] frameIndexes, TimeSpan frameDuration)
        {
            View = view;
            FrameIndexes = frameIndexes;
            FrameDuration = frameDuration;
        }
    }
}
