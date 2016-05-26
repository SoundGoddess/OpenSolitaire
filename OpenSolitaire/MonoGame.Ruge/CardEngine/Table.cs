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
        
        protected int _stackOffsetHorizontal, _stackOffsetVertical;
        protected Texture2D _cardBack, _slotTex;
        protected SpriteBatch _spriteBatch;
        protected DragonDrop<IDragonDropItem> _dragonDrop;
        
        public List<Slot> slots = new List<Slot>();

        public Table(DragonDrop<IDragonDropItem> dd, Texture2D cardBack, Texture2D slotTex, int stackOffsetH, int stackOffsetV) {
            _spriteBatch = dd.spriteBatch;
            _dragonDrop = dd;
            _stackOffsetHorizontal = stackOffsetH;
            _stackOffsetVertical = stackOffsetV;
            _cardBack = cardBack;
            _slotTex = slotTex;
        }

        
        public void AddSlot(Slot slot) {
            slot.stack.SetOffset(_stackOffsetHorizontal, _stackOffsetVertical);
            slots.Add(slot);
            _dragonDrop.Add(slot);
        }


        /// <summary>
        /// override this to set up your table
        /// </summary>
        public void SetTable() { }

        /// <summary>
        /// override this to clear the table
        /// </summary>
        public void Clear() { }


    }

}
