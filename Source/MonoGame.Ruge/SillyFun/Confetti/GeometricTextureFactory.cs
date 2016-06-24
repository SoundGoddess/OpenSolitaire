// Attribution (a) Jon Wilson https://github.com/jonashw

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Ruge.SillyFun.Confetti {
    public static class GeometricTextureFactory {
        public static Texture2D Rectangle(GraphicsDevice graphicsDevice, int width, int height, Color color) {
            var rect = new Texture2D(graphicsDevice, width, height);
            var data = new Color[width*height];
            for (var i = 0; i < data.Length; ++i) {
                data[i] = color;
            }
            rect.SetData(data);
            return rect;
        }
    }
}