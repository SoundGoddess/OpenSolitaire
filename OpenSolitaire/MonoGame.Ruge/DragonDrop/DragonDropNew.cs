/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)
 
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Ruge.ViewportAdapters;

namespace MonoGame.Ruge.DragonDrop {

    public class DragonDrop<T> : DrawableGameComponent where T : IDragonDropItem {

        MouseState oldMouse, currentMouse;
        
        ViewportAdapter viewport;

        private T selectedItem;
        private List<T> draggableItems;


        /// <summary>
        /// Constructor. Uses MonoGame.Extended ViewportAdapter
        /// </summary>
        /// <param name="game"></param>
        /// <param name="sb"></param>
        /// <param name="vp"></param>
        public DragonDrop(Game game, ViewportAdapter vp) : base(game) {
            viewport = vp;
            selectedItem = default(T);
            draggableItems = new List<T>();
        }

    }
}
