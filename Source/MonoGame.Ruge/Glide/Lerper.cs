using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Ruge.Glide {
    public abstract class Lerper {
        [Flags]
        public enum Behavior {
            None = 0,
            Reflect = 1,
            Rotation = 2,
            RotationRadians = 4,
            RotationDegrees = 8,
            Round = 16
        }

        protected const float DEG = 180f / (float) Math.PI;
        protected const float RAD = (float) Math.PI / 180f;

        public abstract void Initialize(object fromValue, object toValue, Behavior behavior);
        public abstract object Interpolate(float t, object currentValue, Behavior behavior);
    }


    public class Vector2Lerper : Lerper {
        private Vector2 from, to, range;

        public override void Initialize(object fromValue, object toValue, Lerper.Behavior behavior) {
            from = (Vector2)fromValue;
            to = (Vector2)toValue;
            range = to - from;
        }

        public override object Interpolate(float t, object currentValue, Lerper.Behavior behavior) {
            var x = from.X + range.X * t;
            var y = from.Y + range.Y * t;

            if (behavior.HasFlag(Behavior.Round)) {
                x = (float)Math.Round(x);
                y = (float)Math.Round(y);
            }

            var current = (Vector2)currentValue;
            if (range.X != 0) current.X = x;
            if (range.Y != 0) current.Y = y;
            return current;
        }
    }

}