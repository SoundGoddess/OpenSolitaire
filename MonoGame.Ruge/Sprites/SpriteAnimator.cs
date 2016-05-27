/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGame.Ruge.Sprites {

    class SpriteAnimator {

        SpriteGrid spriteGrid;
        List<Vector2> queue;
        Vector2 idle;
        int playIndex = 0;
        bool loop;

        public SpriteAnimator(Vector2 idle, SpriteGrid spriteGrid, bool loop = true) {

            this.idle = idle;
            this.spriteGrid = spriteGrid;
            this.loop = loop;
            queue = new List<Vector2>();

        }

        public void Add(Vector2 vector) { queue.Add(vector); }

        public void Clear() { queue.Clear();  }


        public Rectangle play() {

            Rectangle rect = spriteGrid.getRectangle(idle);

            if (queue.Count > 0) { 

                // check if it is out of range
                if (playIndex == queue.Count) {

                    if (!loop) { 
                        queue.Clear();
                        return rect;
                    }
                    else {
                        playIndex = 0;
                    }

                }
            
                Vector2 current = queue[playIndex];
                rect = spriteGrid.getRectangle(current);
            
                playIndex++;
            }

            return rect;

        }

        public Rectangle stop() {

            return spriteGrid.getRectangle(idle);

        }
        
    }

}
