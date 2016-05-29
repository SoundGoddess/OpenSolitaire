/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)

 */

using System;
using MonoGame.Ruge.DragonDrop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Ruge.CardEngine {

    public class Deck : Stack {

        public Deck(Texture2D cardBack, Texture2D slotTex, SpriteBatch spriteBatch, int stackOffsetH, int stackOffsetV) 
            : base(cardBack, slotTex, spriteBatch, stackOffsetH, stackOffsetV) {

            type = StackType.deck;

        }

        /// <summary>
        /// populate your deck with a typical set of cards
        /// </summary>
        public void freshDeck() {

            cards.Clear();

            foreach (Suit mySuit in Enum.GetValues(typeof(Suit))) {

                foreach (Rank myRank in Enum.GetValues(typeof(Rank))) {

                    cards.Add(new Card(myRank, mySuit, cardBack, spriteBatch));

                }

            }

        }

        /// <summary>
        /// makes a smaller random deck for testing
        /// </summary>
        /// <param name="numCards"></param>
        public void testDeck(int numCards) {

            cards.Clear();

            var subDeck = new Deck(cardBack, slot.Texture, spriteBatch, stackOffsetHorizontal, stackOffsetVertical);
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
