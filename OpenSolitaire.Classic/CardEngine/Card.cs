/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)

 */

using MonoGame.Ruge.DragonDrop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Ruge.CardEngine {

    public enum Suit {

        clubs,
        hearts,
        diamonds,
        spades

    };

    public enum CardColor {

        red,
        black

    }

    public enum Rank {
        _A,
        _2,
        _3,
        _4,
        _5,
        _6,
        _7,
        _8,
        _9,
        _10,
        _J,
        _Q,
        _K
    }

    public class Card : IDragonDropItem {

        private readonly SpriteBatch spriteBatch;

        public Vector2 Position { get; set; }

        public Rectangle Border => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

        protected Texture2D cardBack, texture;
        public Texture2D Texture => isFaceUp ? texture : cardBack;

        public bool IsSelected { get; set; } = false;
        public bool IsMouseOver { get; set; } = false;
        public bool IsDraggable { get; set; } = false;
        public int ZIndex { get; set; } = 1;

        public CardColor color {
            get {
                if (suit.Equals(Suit.hearts) || suit.Equals(Suit.diamonds)) return CardColor.red;
                else return CardColor.black;
            }
        }

        public Rank rank;
        public Suit suit;

        public bool isFaceUp = false;
        

        public Card(Rank rank, Suit suit, Texture2D cardBack, SpriteBatch spriteBatch) {

            this.spriteBatch = spriteBatch;
            this.rank = rank;
            this.suit = suit;
            this.cardBack = cardBack;

        }




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
