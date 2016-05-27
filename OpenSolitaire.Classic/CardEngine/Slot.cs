/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)

 */

using MonoGame.Ruge.DragonDrop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Ruge.CardEngine {
    
    public class Slot : IDragonDropItem {
        
        public Vector2 Position { get; set; }
        public bool IsSelected { get; set; }
        public bool IsMouseOver { get; set; }
        public Rectangle Border { get; }
        public bool IsDraggable { get; set; }
        public int ZIndex { get; set; }
        public Texture2D Texture { get; }

        public void OnSelected() {
            throw new System.NotImplementedException();
        }

        public void OnDeselected() {
            throw new System.NotImplementedException();
        }

        public bool Contains(Vector2 pointToCheck) {
            throw new System.NotImplementedException();
        }

        public void OnCollusion(IDragonDropItem item) {
            throw new System.NotImplementedException();
        }
    }
}
