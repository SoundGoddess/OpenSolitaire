/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Ruge.Sprites {

    class SpriteGrid {

        private int rows, cols, width, height;
        private Vector2 offset;

        public SpriteGrid(int rows, int cols, int width, int height, Vector2 offset) {

            this.rows = rows;
            this.cols = cols;
            this.width = width;
            this.height = height;
            this.offset = offset;

        }

        public SpriteGrid(int rows, int cols, int width, int height) : this(rows, cols, width, height, Vector2.Zero) {}

            
        public Rectangle getRectangle(Vector2 vector) {

            int x = (int)vector.X * width;
            int y = (int)vector.Y * height;

            x += (int)offset.X;
            y += (int)offset.Y;

            Rectangle rect = new Rectangle(x, y, width, height);
            return rect;

        }

        public Rectangle getRectangle(int col, int row) {

            Vector2 vect = new Vector2(col, row);
            Rectangle rect = getRectangle(vect);
            return rect;

        }

    }
}
