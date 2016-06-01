/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)

 */

using System;
using System.Collections.Generic;
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

        // Z-Index constants
        protected const int ON_TOP = 1000;

        private readonly SpriteBatch spriteBatch;

        private Vector2 _position;
        public Vector2 Position {
            get { return _position; }
            set {
                
                _position = value;
                
                if (Child != null) {
                    
                    Vector2 pos = new Vector2(_position.X + stack.offset.X, _position.Y + stack.offset.Y);

                    Child.Position = pos;
                    Child.snapPosition = pos;
                    Child.ZIndex = ZIndex + 1;
                    
                }

            }
        }
        
        public Vector2 snapPosition { get; set; }
        public Card Child { get; set; } = null;

        public Rectangle Border => new Rectangle((int) Position.X, (int) Position.Y, Texture.Width, Texture.Height);

        protected Texture2D cardBack, texture;

        public Texture2D Texture => isFaceUp ? texture : cardBack;
        public void SetTexture(Texture2D newTexture) => texture = newTexture;

        public Stack stack { get; set; }
        public int ZIndex { get; set; } = 1;

        public bool isSnapAnimating = false;
        public bool snap = true;
        public float snapSpeed = 25f;

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


        public void flipCard() {
            isFaceUp = !isFaceUp;
        }


        #region DragonDrop Stuff

        public bool IsSelected { get; set; } = false;
        
        public bool IsMouseOver { get; set; }

        public bool IsDraggable { get; set; } = false;
        public bool Contains(Vector2 pointToCheck) {
            var mouse = new Point((int)pointToCheck.X, (int)pointToCheck.Y);
            return Border.Contains(mouse);
        }

        #endregion

        #region MonoGame
        

        public void Update(GameTime gameTime) {
            

            if (IsSelected) {
                var fixChild = Child;

                while (fixChild != null) {
                    fixChild.ZIndex += ON_TOP;
                    fixChild = fixChild.Child;
                }
            }
            
            if (isSnapAnimating) {

                isSnapAnimating = !SnapAnimation();

            }

        }

        public void Draw(GameTime gameTime) {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

        #endregion

        public void MoveToEmptyStack(Stack newStack) {

            if (newStack.Count == 0) newStack.addCard(this, true);

        }



        public void SetParent(Card parent) {
            
            parent.Child = this;
            parent.stack.addCard(this, true);

        }



        /// <summary>
        /// Animation for returning the card to its original position if it can't find a new place to snap to
        /// </summary>
        /// <returns>returns true if the card is back in its original position; otherwise it increments the animation</returns>
        private bool SnapAnimation() {

            var backAtOrigin = false;

            var pos = Position;

            float distance = (float)Math.Sqrt(Math.Pow(snapPosition.X - pos.X, 2) + (float)Math.Pow(snapPosition.Y - pos.Y, 2));
            float directionX = (snapPosition.X - pos.X) / distance;
            float directionY = (snapPosition.Y - pos.Y) / distance;

            pos.X += directionX * snapSpeed;
            pos.Y += directionY * snapSpeed;


            if (Math.Sqrt(Math.Pow(pos.X - Position.X, 2) + Math.Pow(pos.Y - Position.Y, 2)) >= distance) {

                Position = snapPosition;

                backAtOrigin = true;

                ZIndex -= ON_TOP;

                if (stack.crunchStacks) stack.UpdatePositions();

            }
            else Position = pos;

            return backAtOrigin;

        }


        #region events

        public event EventHandler Selected;

        public void OnSelected() {
            
//            Console.WriteLine("mouse: " + suit + "-" + rank + " - selected");

            if (IsDraggable) {
                IsSelected = true;
            }
            ZIndex += ON_TOP;

            Selected?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Deselected;

        public void OnDeselected() {

//            Console.WriteLine("mouse: " + suit + "-" + rank + " - deselected");

            IsSelected = false;

            if (Position != snapPosition) isSnapAnimating = true;

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
