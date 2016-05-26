/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)
*/

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;


namespace MonoGame.Ruge.Sprites {

    public class SpriteRPG {

        public List<SpriteRPGPlayer> player = new List<SpriteRPGPlayer>();

        // assuming 4 rows because this is designed for RPG Maker tile sets
        private const int rows = 4;
        
        public SpriteRPG(int numPlayers, int playersPerRow, int cols, int width, int height) {

            for (int i = 0; i < numPlayers; i++) {

                // meta means the grid of player sprites comprising a single player
                int metaRow = (int)Math.Floor((float)i / playersPerRow);
                int y = metaRow * height * rows;

                int metaCol = i - (playersPerRow * metaRow);
                int x = metaCol * width * cols;

                player.Add(new SpriteRPGPlayer(cols, width, height, new Vector2(x, y), i));

            }

        }

        public int Count { get { return player.Count; } }

    }

}
