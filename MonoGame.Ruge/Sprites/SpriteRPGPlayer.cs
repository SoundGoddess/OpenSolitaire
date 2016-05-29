/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)
*/

using Microsoft.Xna.Framework;

namespace MonoGame.Ruge.Sprites {

    public class SpriteRPGPlayer {
        
        private int cols, width, height;
        private SpriteAnimator animateLeft, animateRight, animateUp, animateDown;
        public Rectangle rect;
        public Vector2 position, origin;
        public int index;

        // this code assumes you're using a 3x4 or 4x4 type of grid layout
        // common with RPG Maker character tile sets.  
        
        public SpriteRPGPlayer(int cols, int width, int height, Vector2 offset, int playerNum = 0) {

            this.cols = cols;
            this.width = width;
            this.height = height;
            index = playerNum;

            Vector2 idle;
            

            SpriteGrid grid = new SpriteGrid(4, cols, width, height, offset);



            idle = new Vector2(cols - 1, 0);
            animateDown = new SpriteAnimator(idle, grid);
            idle = new Vector2(cols - 1, 1);
            animateLeft = new SpriteAnimator(idle, grid);
            idle = new Vector2(cols - 1, 2);
            animateRight = new SpriteAnimator(idle, grid);
            idle = new Vector2(cols - 1, 3);
            animateUp = new SpriteAnimator(idle, grid);


            for (int i = 0; i < cols; i++) {

                animateDown.Add(new Vector2(i, 0));
                animateLeft.Add(new Vector2(i, 1));
                animateRight.Add(new Vector2(i, 2));
                animateUp.Add(new Vector2(i, 3));

            }

            rect = grid.getRectangle(new Vector2(cols - 1, 0));

            origin = new Vector2(width / 2.0f, height / 2.0f);
            
        }

        public SpriteRPGPlayer(int cols, int width, int height) : this(cols, width, height, Vector2.Zero) { }
        public SpriteRPGPlayer(int cols, int width, int height, int playerNum) : this(cols, width, height, Vector2.Zero, playerNum) { }

        public void moveDown()  { rect = animateDown.play(); }
        public void moveUp()    { rect = animateUp.play(); }
        public void moveLeft()  { rect = animateLeft.play(); }
        public void moveRight() { rect = animateRight.play(); }

    }

}
