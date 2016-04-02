using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

    class Rank {

        public char[] ranks = {

            'A',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            'J',
            'Q',
            'K'

        };
        

    }


    class Card {

        Rank ranks = new Rank();
        public char rank;
        public Suit suit;
        bool isFaceUp = false;

        public Card (char rank, Suit suit) {

            this.rank = rank;
            this.suit = suit;

        }

        #region methods

        public void flipCard() {
            isFaceUp = !isFaceUp;
        }

        public CardColor color {

            get {

                if ( (suit == Suit.clubs) || (suit == Suit.spades) ){
                    return CardColor.black;
                }
                else {
                    return CardColor.red;
                }

            }

        }

        #endregion

        #region properties

        public int index {

            get { return Array.IndexOf(ranks.ranks, rank); }

        }

        // hard coded to the particular deck I'm using atm.  can update later

        public string asset {

            get {

                string location;

                switch (rank) {

                    case 'A':
                        location = "ace";
                        break;
                    case 'K':
                        location = "king";
                        break;
                    case 'Q':
                        location = "queen";
                        break;
                    case 'J':
                        location = "jack";
                        break;
                    default:
                        location = rank.ToString();
                        break;

                }

                location = "assets/" + location + "_of_" + suit + ".png";

                return location;

            }

        }

        public bool faceUp { get { return isFaceUp; } }

        #endregion


    }

    
    class Deck {

        Rank ranks = new Rank();
        List<Card> cards = new List<Card>();

        #region methods
        
        //populate your deck with a typical set of cards
        public void freshDeck() {

            cards.Clear();

            foreach (Suit mySuit in Enum.GetValues(typeof(Suit))) {

                foreach (char myRank in ranks.ranks) {

                    cards.Add(new Card(myRank, mySuit));

                }

            }

        }

        public void debugDeck() {

            Console.WriteLine("===");

            if (cards.Count > 0) { 
                foreach (Card card in cards) {

                    String strFaceUp = (card.faceUp ? "face up" : "face down");
                    Console.WriteLine(card.index.ToString("00") + ": " + card.rank + " of " + card.suit + " (" + strFaceUp + ")");

                }
            }
            else { Console.WriteLine("(empty hand)"); }

        }

        //makes a smaller random deck for testing
        public void testDeck(int numCards) {

            cards.Clear();

            Deck subDeck = new Deck();
            subDeck.freshDeck();
            subDeck.shuffle();

            if (numCards <= subDeck.Count) {

                for (int i=0; i < numCards; i++) {
                    cards.Add(subDeck.drawCard());
                }

            }

            subDeck = null;

        }

        public void shuffle() {

            //wait a few ms to avoid seed collusion
            Thread.Sleep(30);

            Random rand = new Random();
            for (int i = cards.Count - 1; i > 0; i--) {
                int randomIndex = rand.Next(i + 1);
                Card tempCard = cards[i];
                cards[i] = cards[randomIndex];
                cards[randomIndex] = tempCard;
            }
        }

        /// <summary>
        /// just picks the top card on the deck and returns it
        /// </summary>
        /// <returns></returns>
        public Card drawCard() {

            if (cards.Count > 0) {

                Card topCard = cards[cards.Count - 1];
                cards.RemoveAt(cards.Count - 1);
                return topCard;

            }
            else { return null; }

        }

        /// <summary>
        /// adds card to your hand or deck
        /// </summary>
        /// <param name="card"></param>
        public void addCard(Card card) {

            cards.Add(card);

        }

        /// <summary>
        /// pulls a specific card from your hand
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="suit"></param>
        /// <returns>Card if found, null otherwise</returns>
        public Card playCard(char rank, Suit suit) {
            
            foreach (Card card in cards){
                
                if ((card.rank == rank) && (card.suit == suit)) {

                    cards.Remove(card);
                    return card;

                }

            }

            return null;            

        }

        // TODO: have no idea if this works or not
        public Card playCard(int cardIndex) {
            
            if (cards.Contains(cards[cardIndex])) {
                Card card = cards[cardIndex];
                cards.RemoveAt(cardIndex);
                return card;
            }
            else { return null; }
        }


        #endregion


        #region properties

        public int Count { get { return cards.Count; } }


        #endregion

    }
    

}
