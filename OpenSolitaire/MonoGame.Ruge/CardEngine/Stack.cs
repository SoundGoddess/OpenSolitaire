/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)

 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoGame.Ruge.CardEngine {

    public enum StackMethod {
        normal,
        horizontal,
        vertical
    }

    public enum StackType {
        draw,
        discard,
        stack,
        deck,
        hand,
        play,
        undefined
    }

    public class Stack {

        protected SpriteBatch _spriteBatch;

        protected Texture2D _cardBack;
        public Texture2D cardBack { get { return _cardBack; } }

        protected List<Card> _cards = new List<Card>();

        protected int _stackOffsetHorizontal, _stackOffsetVertical;

        public Vector2 offset {
            get {

                switch (method) {
                    case StackMethod.horizontal: return new Vector2(_stackOffsetHorizontal, 0);
                    case StackMethod.vertical:   return new Vector2(0, _stackOffsetVertical);
                    default:                     return Vector2.Zero;
                }
            }
        }

        public Slot slot { get; set; }

        public IEnumerable<Card> cards_Zsort {
            get {

                // since MonoGame renders sprites on top of each other based on the order they are called in the Draw() method, this
                // little line of code sorts the sprite objects to take into consideration the ZIndex so that things render as expected.
                _cards = _cards.OrderBy(z => z.ZIndex).ToList();
                foreach (var item in _cards) { yield return item; }

            }
        }

        public List<Card> cards {
            get { return _cards; }

        }

        public int Count { get { return _cards.Count; } }

        public StackType type = StackType.hand;
        public StackMethod method = StackMethod.normal;

        #region public methods

        public void addCard(Card card) {
            card.stack = this;
            _cards.Add(card);
        }
        public void Clear() { _cards.Clear(); }

        private void OnStackChanged(object sender, EventArgs eventArgs) {
            _cards.Remove((Card)sender);
        }

        public void SetOffset(int x, int y) {

            _stackOffsetHorizontal = x;
            _stackOffsetVertical = y;

        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="cardBack"></param>
        /// <param name="sb"></param>
        public Stack(Texture2D cardBack, SpriteBatch sb) {
            _cardBack = cardBack;
            _spriteBatch = sb;
        }


        /// <summary>
        /// attempts to pull a specific card from your hand using the rank and suit
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="suit"></param>
        /// <returns>Card if found, null otherwise</returns>
        public Card playCard(Rank rank, Suit suit) {

            foreach (Card card in cards) {

                if ((card.rank == rank) && (card.suit == suit)) {

                    _cards.Remove(card);
                    return card;

                }

            }

            return null;

        }

        /// <summary>
        /// pulls a specific card from your hand using the index
        /// todo: test this method
        /// </summary>
        /// <param name="cardIndex"></param>
        /// <returns>Card if found, null otherwise</returns>
        public Card playCard(int cardIndex) {

            if (cards.Contains(_cards[cardIndex])) {
                Card card = _cards[cardIndex];
                _cards.RemoveAt(cardIndex);
                return card;
            }
            else { return null; }
        }


        /// <summary>
        /// just picks the top card on the stack and returns it
        /// </summary>
        /// <returns></returns>
        public Card drawCard() {

            if (_cards.Count > 0) {

                Card topCard = _cards[_cards.Count - 1];
                _cards.RemoveAt(_cards.Count - 1);
                return topCard;

            }
            else { return null; }

        }


        public void shuffle() {

            //wait a few ms to avoid seed collusion
            Thread.Sleep(30);

            Random rand = new Random();
            for (int i = _cards.Count - 1; i > 0; i--) {
                int randomIndex = rand.Next(i + 1);
                Card tempCard = _cards[i];
                _cards[i] = _cards[randomIndex];
                _cards[randomIndex] = tempCard;
            }
        }

        
        public void debug() {

            Console.WriteLine("===");

            if (_cards.Count > 0) {
                foreach (Card card in cards) {

                    String strFaceUp = (card.faceUp ? "face up" : "face down");
                    Console.WriteLine(card.ZIndex.ToString("00") + ": " + card.rank + " of " + card.suit + " (" + strFaceUp + ")");

                }
            }
            else { Console.WriteLine("(empty hand)"); }

        }


    }
    #endregion
}
