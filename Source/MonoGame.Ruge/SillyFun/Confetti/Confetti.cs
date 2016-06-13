// Attribution (a) Jon Wilson https://github.com/jonashw

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Ruge.SillyFun.Confetti {
    public class Confetti {
        private readonly List<ConfettiPiece> _pieces = new List<ConfettiPiece>();
        private readonly Random _random = new Random();
        private readonly ReadOnlyCollection<Texture2D> _textures;

        public Confetti(ReadOnlyCollection<Texture2D> textures) {
            _textures = textures;
        }

        public void Update(GameTime gameTime) {
            foreach (var piece in _pieces) {
                piece.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            foreach (var piece in _pieces) {
                piece.Draw(spriteBatch);
            }
        }

        public void Sprinkle(int n, Rectangle withinRectangle) {
            _pieces.AddRange(
                Enumerable.Range(0, Math.Abs(n)).Select(i =>
                    new ConfettiPiece(randomTexture()) {
                        Position = new Vector2(
                            _random.Next(0, withinRectangle.Width),
                            _random.Next(0, withinRectangle.Height)),
                        Scale = randomScale(),
                        Velocity = new Vector2(_random.Next(-10, 10) / 5f, 3f)
                    }));
        }

        public void Explode(int n, Vector2 atPosition) {
            const float velocity = 3;
            _pieces.AddRange(
                Enumerable.Range(1, Math.Abs(n)).Select(theta =>
                    new ConfettiPiece(randomTexture()) {
                        Position = atPosition,
                        Scale = randomScale(),
                        Velocity = new Vector2(
                            velocity * (float) Math.Cos(theta),
                            velocity * (float) Math.Sin(theta))
                    }));
        }

        private float randomScale() {
            return _random.Next(5, 10) / 10f;
        }

        private Texture2D randomTexture() {
            return _textures[_random.Next(0, 100) % 3];
        }
    }
}