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
        public int ZIndex { get; set; } = -1;
        public Texture2D Texture { get; set; }

        public bool IsDraggable { get; set; } = false;
        public bool IsVisible { get; set; } = true;

        private readonly SpriteBatch _spriteBatch;
        
        public StackType type {
            get { return stack.type; }
            set { stack.type = value; }
        }

        public Stack stack { get; set; }

        public Rectangle Border => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

        #region constructor 

        public Slot(Texture2D slotTex, Texture2D cardBack, SpriteBatch sb, Vector2 pos) {

            Position = pos;
            Texture = slotTex;
            _spriteBatch = sb;
            stack = new Stack(cardBack, sb);
            stack.slot = this;

        }

        #endregion

        #region methods


        public bool Contains(Vector2 pointToCheck) {
            var mouse = new Point((int)pointToCheck.X, (int)pointToCheck.Y);
            return Border.Contains(mouse);
        }

        public void Draw(GameTime gameTime) {
            _spriteBatch.Draw(Texture, Position, Color.White);
        }

        #endregion

        #region overrides

        // todo: override methods to set up your logic

        public void OnSelected() { }
        public void OnDeselected() { }

        public void Update(GameTime gameTime) { }
        public void OnCollusion(IDragonDropItem item) { }

        #endregion



    }

}
