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
        public bool faceUp => isFaceUp;

        protected Texture2D cardback, texture;
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
        
        public Stack stack { get; set; }
        public int stackIndex { get; set; } = 0;

        public Rectangle Border => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

        public bool Contains(Vector2 pointToCheck) {
            Point mouse = new Point((int)pointToCheck.X, (int)pointToCheck.Y);
            return Border.Contains(mouse);
        }



        private Vector2 _position;

        public Vector2 Position {

            get { return _position; }

            set {
                _position = value;

                if (IsSelected) OnPositionUpdate();
            }

        }


        public Texture2D Texture => isFaceUp ? texture : cardback;

        #endregion


        #region constructor


        public Card(Rank rank, Suit suit, Texture2D cardback, SpriteBatch sb) {

            _spriteBatch = sb;
            this.rank = rank;
            this.suit = suit;
            this.cardback = cardback;

        }

        #endregion


        #region methods

        public void flipCard() {
            isFaceUp = !isFaceUp;
        }


        public void SetTexture(Texture2D tex) => texture = tex;

        /// <summary>
        /// Animation for returning the card to its original position if it can't find a new place to snap to
        /// </summary>
        /// <returns>returns true if the card is back in its original position; otherwise it increments the animation</returns>
        private bool ReturnToOrigin() {

            var backAtOrigin = false;

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

            foreach (var card in stack.cards) {

                // if you're the parent then it's my time to leave home :P
                if (card.Child == this) card.Child = null;

            }

            stack.cards.Remove(this);
            newStack.addCard(this);
            stack = newStack;

            if (stack.Count == 1) {

                Position = stack.slot.Position;
                origin = Position;

            }
            else {

                foreach (var card in stack.cards) {

                    // new parent sets the stack index
                    if (card.Child == this) stackIndex = card.stackIndex + 1;

                }
            }
            int newIndex = stackIndex + 1;

            while (Child != null) {
                
                stack.cards.Remove(Child);
                newStack.addCard(Child);
                Child.stack = newStack;
                Child.stackIndex = newIndex;
                newIndex++;
                Child = Child.Child;
            }

        }

        public void SetParent(Card parent) {

            MoveStack(parent.stack);
            parent.Child = this;

            stackIndex = parent.stackIndex + 1;

            int newIndex = stackIndex + 1;

            while (Child != null) {
                
                Child.stackIndex = newIndex;
                newIndex++;
                Child = Child.Child;

            }
            
        }

        public void CryingChild() {

            int newIndex = stackIndex + 1;

            while (Child != null) {

                Vector2 pos = new Vector2(Position.X + stack.offset.X, Position.Y + stack.offset.Y);

                Child.Position = pos;
                Child.origin = pos;
                Child.ZIndex = ZIndex + 1;

                Child.stackIndex = newIndex;
                newIndex++;

                Child = Child.Child;

            }

        }


        #endregion


        #region events

        public event EventHandler Selected;

        public void OnSelected() {
            
            if (IsDraggable) {
                IsSelected = true;
            }

            Selected(this, EventArgs.Empty);


        }

        public event EventHandler Deselected;

        public void OnDeselected() {
            
            if (Position != origin) returnToOrigin = true;
            
            IsSelected = false;

            Deselected(this, EventArgs.Empty);

        }
        
        public void OnPositionUpdate() { CryingChild(); }

        public event EventHandler<CollusionEvent> Collusion;

        public void OnCollusion(IDragonDropItem item) {

            CollusionEvent e = new CollusionEvent();
            e.item = item;

            Collusion(this, e);

        }


        #endregion


        #region MonoGame

        public void Draw(GameTime gameTime) {
            _spriteBatch.Draw(Texture, Position, Color.White);
        }

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