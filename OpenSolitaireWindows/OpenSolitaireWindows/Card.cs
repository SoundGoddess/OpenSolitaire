using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonDrop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OpenSolitaireMG {

    enum Suit {

        clubs,
        hearts,
        diamonds,
        spades

    };

    enum CardColor {

        red,
        black

    }

    enum Rank {
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

    class Card : IDragAndDropItem {
        
        public Rank rank;
        public Suit suit;

        bool isFaceUp = false;
        Texture2D cardback, _texture;

        bool _isDraggable = true;
                        
        public bool IsSelected { get; set; }
        public bool IsMouseOver { get; set; }
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
                else return cardback;

            }

        }

        public Card Child { get; set; }
        public bool IsDraggable {
            get {
                if (!isFaceUp) return false;
                else return _isDraggable;
            }
            set { _isDraggable = value; }
        } 

        private const int ON_TOP = 3000;



        public int ZIndex {
            get { return _zIndex; }
            set {
                _zIndex = value;
                if (hasChild) Child.ZIndex = _zIndex + 1;
            }
        }
        
        private int _zIndex;

        public Vector2 origin { get; set; }
        
        private bool returnToOrigin = false;
        public bool hasChild = false;
        public bool isDiscarded = false;
        public bool isPlayed = false;

        private readonly SpriteBatch spriteBatch;


        public void OnPositionUpdate() {

            
            if (hasChild) {

                Card child = Child;

                if (!child.isDiscarded) {
                    var pos = Position;

                    pos.Y = Position.Y + 28;
                    pos.X = Position.X;

                    child.origin = pos;
                    child.Position = pos;

                    Child = child;
                }
            }
            

        }

        public void OnSelected() {

            if (IsDraggable) {
                IsSelected = true;
                ZIndex += ON_TOP;
            }

        }

        public void OnDeselected() {

            // don't reset ZIndex here; let returnToOrigin handle it

            IsSelected = false;

            if (Position != origin) returnToOrigin = true;

        }

        public void HandleCollusion(IDragAndDropItem item) {

            Card parent = (Card)item;
         
                Console.WriteLine(rank.ToString().Substring(1) + "-" + suit.ToString() + "," +
                    parent.rank.ToString().Substring(1) + "-" + parent.suit.ToString() + ":" +
                    (int)rank + "," + (int)parent.rank);
            if (parent.isFaceUp && !parent.isDiscarded && !parent.isPlayed) {
                if ((parent.rank == (rank + 1)) && (parent.color != color)) {
                    parent.AttachChild(this);
                    item = parent;
                }
                else if (parent.isPlayed) {

                    ZIndex = parent.ZIndex + 1;
                    Position = parent.Position;
                    origin = Position;
                    isDiscarded = true;
                    IsDraggable = false;
                    isPlayed = true;

                }


            }
                        
        }


        public void AttachChild(Card child) {

            isDiscarded = false;
            var pos = Position;

            pos.Y = Position.Y + 28;
            pos.X = Position.X;

            child.origin = pos;
            child.Position = pos;

            Child = child;
            hasChild = true;

            ZIndex = -(int)rank;

        }
        /// <summary>
        /// Animation for returning the card to its original position if it can't find a new place to snap to
        /// </summary>
        /// <returns>returns true if the card is back in its original position; otherwise it increments the animation</returns>
        private bool ReturnToOrigin() {

            bool backAtOrigin = false;

            var pos = Position;
            float speed = 25.0f;

            float distance = (float)Math.Sqrt(Math.Pow(origin.X - pos.X, 2) + (float)Math.Pow(origin.Y - pos.Y, 2));
            float directionX = (origin.X - pos.X) / distance;
            float directionY = (origin.Y - pos.Y) / distance;

            pos.X += directionX * speed;
            pos.Y += directionY * speed;


            if (Math.Sqrt(Math.Pow(pos.X - Position.X, 2) + Math.Pow(pos.Y - Position.Y, 2)) >= distance) {

                Position = origin;

                backAtOrigin = true;

                ZIndex -= ON_TOP;

            }
            else Position = pos;

            return backAtOrigin;

        }

        public bool ReturnToOrigin(Vector2 destination) {

            origin = destination;
            return ReturnToOrigin();

        }


        public Rectangle Border {
            get {

                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);

            }
        }
        
        public Card(Rank rank, Suit suit, Texture2D cardback, SpriteBatch sb) {
            
            spriteBatch = sb;
            this.rank = rank;
            this.suit = suit;
            this.cardback = cardback;
            ZIndex = -(int)rank;

        }

        #region methods

        public void flipCard() {
            isFaceUp = !isFaceUp;
        }

        public CardColor color {

            get {

                if ((suit == Suit.clubs) || (suit == Suit.spades)) {
                    return CardColor.black;
                }
                else {
                    return CardColor.red;
                }

            }

        }

        public void SetTexture(Texture2D texture) { _texture = texture; }

        public void Update(GameTime gameTime) {

            // feels kinda hacky but this fixes a really annoying bug
            if (hasChild && Child.isDiscarded) hasChild = false;

            if (returnToOrigin) {

                returnToOrigin = !ReturnToOrigin();

            }


        }

        public void Draw(GameTime gameTime) {
            
            spriteBatch.Draw(Texture, Position, Color.White);
            //spriteBatch.Draw(rend, Border, Color.White);

        }

        #endregion

        #region properties

        //public int index { get { return Array.IndexOf(ranks.ranks, rank); } }

        //public Rectangle rect { get { return _rect; } }

        public string asset {

            get {

                string location;

                switch (rank) {

                    case Rank._A:
                        location = "ace";
                        break;
                    case Rank._K:
                        location = "king";
                        break;
                    case Rank._Q:
                        location = "queen";
                        break;
                    case Rank._J:
                        location = "jack";
                        break;
                    default:
                        location = rank.ToString().Substring(1);
                        break;

                }

                // hard coded to the current deck asset set can be improved later
                location = "assets/small/" + location + "_of_" + suit;

                return location;

            }

        }
        
        public bool faceUp { get { return isFaceUp; } }

        #endregion


    }
}
