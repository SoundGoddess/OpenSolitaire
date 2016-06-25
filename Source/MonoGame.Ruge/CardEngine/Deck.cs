/* Attribution (a) 2016 The Ruge Project (http://ruge.metasmug.com/) 
 * Unlicensed under NWO-CS (see UNLICENSE)
 */

using System;
using MonoGame.Ruge.DragonDrop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Ruge.CardEngine {

    public class Deck : Stack {

        public DeckType deckType;

        public Deck(Table table, DeckType deckType, Texture2D cardBack, Texture2D slotTex, SpriteBatch spriteBatch, int stackOffsetH, int stackOffsetV) 
            : base(table, cardBack, slotTex, spriteBatch, stackOffsetH, stackOffsetV) {

            this.deckType = deckType;
            type = StackType.deck;

        }

        /// <summary>
        /// populate your deck with a typical set of cards
        /// </summary>
        public void freshDeck() {

            cards.Clear();

            if (deckType == DeckType.hex)
                foreach (HexSuit mySuit in Enum.GetValues(typeof(HexSuit)))
                    foreach (HexRank myRank in Enum.GetValues(typeof(HexRank)))
                        cards.Add(new Card(deckType, mySuit, myRank, cardBack, spriteBatch));
            
            else if (deckType == DeckType.playing) 
                foreach (PlayingSuit mySuit in Enum.GetValues(typeof(PlayingSuit)))
                    foreach (PlayingRank myRank in Enum.GetValues(typeof(PlayingRank)))
                        cards.Add(new Card(deckType, mySuit, myRank, cardBack, spriteBatch));

            else if (deckType == DeckType.friendly)
                foreach (FriendlySuit mySuit in Enum.GetValues(typeof(FriendlySuit)))
                    foreach (PlayingRank myRank in Enum.GetValues(typeof(PlayingRank)))
                        cards.Add(new Card(deckType, mySuit, myRank, cardBack, spriteBatch));

            else if (deckType == DeckType.tarot) {

                foreach (TarotSuit mySuit in Enum.GetValues(typeof(TarotSuit))) {
                    
                    if (mySuit == TarotSuit.major)
                        foreach (TarotRankMajor myRank in Enum.GetValues(typeof(TarotRankMajor)))
                            cards.Add(new Card(deckType, mySuit, myRank, cardBack, spriteBatch));
                    else
                        foreach (TarotRank myRank in Enum.GetValues(typeof(TarotRank)))
                            cards.Add(new Card(deckType, mySuit, myRank, cardBack, spriteBatch));
                }
            }
        }

        /// <summary>
        /// makes a smaller random deck for testing
        /// </summary>
        /// <param name="numCards"></param>
        public void testDeck(int numCards) {

            cards.Clear();

            var subDeck = new Deck(table, deckType, cardBack, slot.Texture, spriteBatch, stackOffsetHorizontal, stackOffsetVertical);
            subDeck.freshDeck();
            subDeck.shuffle();

            if (numCards <= subDeck.Count) {

                for (int i = 0; i < numCards; i++) {
                    cards.Add(subDeck.drawCard());
                }

            }

            subDeck = null;

        }


    }
}
