using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Base
{
    static class VectorHelper
    {
        public static Vector2 AngleToV2(float angle, float length)
        {
            Vector2 direction = Vector2.Zero;
            direction.X = (float)Math.Cos(angle) * length;
            direction.Y = (float)Math.Sin(angle) * length;
            return direction;
        }

        public static Vector2 Rotate(this Vector2 vector, float angle)
        {
            return new Vector2(
                x: (vector.X * (float)Math.Cos(angle)) - (vector.Y * (float)Math.Sin(angle)),
                y: (vector.Y * (float)Math.Cos(angle)) + (vector.X * (float)Math.Sin(angle)));
        }

        public static float Angle(this Vector2 vector)
        {
            return (float)Math.Atan2(-vector.X, vector.Y);
        }
    }
}
