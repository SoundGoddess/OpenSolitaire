/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)

 */
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Ruge.DragonDrop;

namespace MonoGame.Ruge.CardEngine {

    public class Table {

        // Z-Index constants
        protected const int ON_TOP = 1000;
        
        protected int stackOffsetHorizontal, stackOffsetVertical;
        protected Texture2D cardBack, slotTex;
        protected SpriteBatch spriteBatch;
        protected DragonDropRef<IDragonDropItem> dragonDrop;
        
        public List<Slot> slots = new List<Slot>();

        public Table(DragonDropRef<IDragonDropItem> dragonDrop, Texture2D cardBack, Texture2D slotTex, int stackOffsetH, int stackOffsetV) {
            spriteBatch = dragonDrop.spriteBatch;
            this.dragonDrop = dragonDrop;
            stackOffsetHorizontal = stackOffsetH;
            stackOffsetVertical = stackOffsetV;
            this.cardBack = cardBack;
            this.slotTex = slotTex;
        }

        
        public void AddSlot(Slot slot) {
            slot.stack.SetOffset(stackOffsetHorizontal, stackOffsetVertical);
            slots.Add(slot);
            dragonDrop.Add(slot);
        }


        /// <summary>
        /// override this to set up your table
        /// </summary>
        public void SetTable() { }

        /// <summary>
        /// override this to clear the table
        /// </summary>
        public void Clear() { }


        /// <summary>
        /// override this to update the board
        /// </summary>
        public void Update() { }

    }

}
