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


        public Rank rank;
        public Suit suit;

        public bool isFaceUp = false;
        public bool faceUp { get { return isFaceUp; } }

        protected Texture2D _cardback, _texture;
        private readonly SpriteBatch _spriteBatch;

        #region properties

        public Card Child { get; set; } = null;
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

        public Vector2 origin { get; set; }
        public float snapSpeed { get; set; } = 25.0f;
        public bool returnToOrigin { get; set; } = false;
        protected const int ON_TOP = 1000;

        public Stack stack { get; set; }

        public Rectangle Border {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); }
        }

        public bool Contains(Vector2 pointToCheck) {
            Point mouse = new Point((int)pointToCheck.X, (int)pointToCheck.Y);
            return Border.Contains(mouse);
        }



        private Vector2 _position;

        public Vector2 Position {

            get { return _position; }

            set { _position = value; OnPositionUpdate(); }

        }


        public Texture2D Texture {

            get {

                if (isFaceUp) return _texture;
                else return _cardback;

            }

        }

        #endregion


        #region constructor


        public Card(Rank rank, Suit suit, Texture2D cardback, SpriteBatch sb) {

            _spriteBatch = sb;
            this.rank = rank;
            this.suit = suit;
            _cardback = cardback;

        }

        #endregion


        #region methods

        public void flipCard() {
            isFaceUp = !isFaceUp;
        }

        public void Draw(GameTime gameTime) {
            _spriteBatch.Draw(Texture, Position, Color.White);
        }

        public void SetTexture(Texture2D texture) { _texture = texture; }

        /// <summary>
        /// Animation for returning the card to its original position if it can't find a new place to snap to
        /// </summary>
        /// <returns>returns true if the card is back in its original position; otherwise it increments the animation</returns>
        private bool ReturnToOrigin() {

            bool backAtOrigin = false;

            var pos = Position;
            
            float distance = (float)Math.Sqrt(Math.Pow(origin.X - pos.X, 2) + (float)Math.Pow(origin.Y - pos.Y, 2));
            float directionX = (origin.X - pos.X) / distance;
            float directionY = (origin.Y - pos.Y) / distance;

            pos.X += directionX * snapSpeed;
            pos.Y += directionY * snapSpeed;


            if (Math.Sqrt(Math.Pow(pos.X - Position.X, 2) + Math.Pow(pos.Y - Position.Y, 2)) >= distance) {

                Position = origin;

                backAtOrigin = true;
                
            }
            else Position = pos;

            return backAtOrigin;

        }

        public void MoveStack(Stack newStack) {

            stack.cards.Remove(this);
            newStack.addCard(this);
            stack = newStack;
            Position = stack.slot.Position;
            origin = Position;

        }

        public void SetParent(Card parent) {

            MoveStack(parent.stack);

            Vector2 pos = new Vector2(parent.Position.X + stack.offset.X, parent.Position.Y + stack.offset.Y);

            Position = pos;
            origin = pos;

            parent.Child = this;

        }


        #endregion


        #region events

        public event EventHandler Selected;
        public void OnSelected() { Selected(this, EventArgs.Empty); }

        public event EventHandler Deselected;
        public void OnDeselected() { Deselected(this, EventArgs.Empty); }
        
        public void OnPositionUpdate() {

            if (Child != null) {

                Vector2 pos = new Vector2(Position.X + stack.offset.X, Position.Y + stack.offset.Y);
                Child.Position = pos;
                Child.ZIndex = ZIndex + 1;

            }

        }

        public event EventHandler<CollusionEvent> Collusion;

        public void OnCollusion(IDragonDropItem item) {

            CollusionEvent e = new CollusionEvent();
            e.item = item;

            Collusion(this, e);

        }


        #endregion


        #region overrides
        

        public void Update(GameTime gameTime) {


            if (returnToOrigin) {

                returnToOrigin = !ReturnToOrigin();

            }


        }
        #endregion
    }

    public class CollusionEvent : EventArgs {

        public IDragonDropItem item { get; set; }

    }
}