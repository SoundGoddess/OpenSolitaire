/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)

 */

using System;
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
        public Vector2 snapPosition { get; set; }

        public Rectangle Border => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

        protected Texture2D cardBack, texture;

        public Texture2D Texture => isFaceUp ? texture : cardBack;
        public void SetTexture(Texture2D tex) => texture = tex;

        public bool IsSelected { get; set; } = false;
        public bool IsMouseOver { get; set; } = false;
        public bool IsDraggable { get; set; } = false;
        public int ZIndex { get; set; } = 1;
        
        protected const int ON_TOP = 1000;

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


        public bool Contains(Vector2 pointToCheck) {
            var mouse = new Point((int)pointToCheck.X, (int)pointToCheck.Y);
            return Border.Contains(mouse);
        }

        #region MonoGame

        public void Update(GameTime gameTime) {
            


        }

        public void Draw(GameTime gameTime) {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

        #endregion

        #region events

        public event EventHandler Selected;

        public void OnSelected() {
            if (IsDraggable) {
                IsSelected = true;
            }

            Selected?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Deselected;

        public void OnDeselected() {
            
            IsSelected = false;

            Deselected?.Invoke(this, EventArgs.Empty);

        }

        public event EventHandler<CollusionEvent> Collusion;

        public void OnCollusion(IDragonDropItem item) {

            var e = new CollusionEvent {item = item};

            Collusion?.Invoke(this, e);

        }

        public class CollusionEvent : EventArgs {

            public IDragonDropItem item { get; set; }

        }

#endregion

    }
}
