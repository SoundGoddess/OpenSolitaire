/* Attribution (a) 2016 The Ruge Project (http://ruge.metasmug.com/) 
 * Licensed under NWO-CS (see License.txt)
 */

using System;
using MonoGame.Ruge.DragonDrop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Ruge.Glide;

namespace MonoGame.Ruge.CardEngine {

    public class Card : IDragonDropItem {

        // Z-Index constants
        protected const int ON_TOP = 1000;

        // it's a tween thing
        private Tweener tween = new Tweener();

        private readonly SpriteBatch spriteBatch;
        public CardType cardType;

        private Vector2 _position;
        public Vector2 Position {
            get { return _position; }
            set {
                
                _position = value;
                
                if (Child != null) {
                    
                    var pos = new Vector2(_position.X + stack.offset.X, _position.Y + stack.offset.Y);

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
        private bool startTween = true;
        public bool snap = true;
        public float snapTime = .7f;

   //     public Tweener tweenager = new Tweener();

        public CardColor color {
            get {


                if (cardType.deckType == DeckType.hex) {

                    switch ((HexSuit) cardType.suit) {

                        case HexSuit.beakers:
                        case HexSuit.planets:
                            return CardColor.black;
                        default:
                            return CardColor.white;
                    }
                }
                if (cardType.deckType == DeckType.playing) {

                    switch ((PlayingSuit) cardType.suit) {

                        case PlayingSuit.diamonds:
                        case PlayingSuit.hearts:
                            return CardColor.red;
                        default:
                            return CardColor.black;
                    }
                }

                return CardColor.none;
                
            }
        }

        public bool isFaceUp = false;
        public Enum suit;
        public Enum rank;

        public Card(DeckType deckType, Enum suit, Enum rank, Texture2D cardBack, SpriteBatch spriteBatch) {

            cardType = new CardType(deckType) {
                suit = suit,
                rank = rank
            };

            this.suit = suit;
            this.rank = rank;

            this.spriteBatch = spriteBatch;
            this.cardBack = cardBack;

            Tween.TweenerImpl.SetLerper<Vector2Lerper>(typeof(Vector2));
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

                //                isSnapAnimating = !SnapAnimation();
                if (startTween) { 
                    tween.Tween(this, new { Position = snapPosition }, snapTime)
                        .Ease(Ease.ElasticOut)
                        .OnComplete(afterTween);

                    startTween = false;
                }

            }
            

            tween.Update(float.Parse(gameTime.ElapsedGameTime.Seconds + "." + gameTime.ElapsedGameTime.Milliseconds));

        }

        private void afterTween() {

            ZIndex -= ON_TOP;

            if (stack.crunchStacks) stack.UpdatePositions();

            isSnapAnimating = false;
            startTween = true;

        }

        public void Draw(GameTime gameTime) {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

        #endregion

        public void MoveToEmptyStack(Stack newStack) {

            stack.table.Save();

            if (newStack.Count == 0) newStack.addCard(this, true);

        }



        public void SetParent(Card parent) {

            stack.table.Save();
            
            parent.Child = this;
            parent.stack.addCard(this, true);

        }
        
        #region events

        public event EventHandler Selected;

        public void OnSelected() {

            if (IsDraggable) {
                IsSelected = true;
            }
            ZIndex += ON_TOP;

            Selected?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Deselected;

        public void OnDeselected() {
            
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
