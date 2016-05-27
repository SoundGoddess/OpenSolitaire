/* ©2016 Hathor Gaia 
* http://HathorsLove.com
* 
* Source code licensed under GPL-3
* Assets licensed seperately (see LICENSE.md)
*/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Ruge.CardEngine;
using MonoGame.Ruge.DragonDrop;
using System.Collections.Generic;

namespace OpenSolitaire.Classic {
    class TableClassic : Table {

        public bool isSetup = false;
        public bool isAnimating = false;

        public TableClassic(DragonDrop<IDragonDropItem> dd, Texture2D cardback, Texture2D slot, int stackOffsetH, int stackOffsetV)
            : base(dd, cardback, slot, stackOffsetH, stackOffsetV) {


        }

        internal void InitializeTable() { }

        
        public new void SetTable() { }

        public new void Clear() { }
        
        public new void Update() { }


    }
}
