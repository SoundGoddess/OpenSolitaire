// Attribution (a) Jon Wilson https://github.com/jonashw

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Ruge.SillyFun.Confetti {
    public class ConfettiPiece {
        private readonly Texture2D _texture;
        public Vector2 Position;
        public float Rotation;
        public float Scale = 1;
        public int TicksToLive = 200;
        public Vector2 Velocity;

        public ConfettiPiece(Texture2D texture) {
            _texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch) {
            if (TicksToLive <= 0) {
                return;
            }
            spriteBatch.Draw(
                _texture,
                Position,
                scale: new Vector2(Scale, Scale),
                rotation: (float) Math.Atan2(Velocity.X, -Velocity.Y),
                origin: new Vector2(_texture.Width / 2f, _texture.Height / 2f));
        }

        public void Update() {
            if (TicksToLive <= 0) {
                return;
            }
            Position += Velocity;
            TicksToLive--;
            Velocity.Y += 0.05f;
        }
    }
}