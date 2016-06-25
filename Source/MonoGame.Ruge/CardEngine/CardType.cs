/* Attribution (a) 2016 The Ruge Project (http://ruge.metasmug.com/) 
 * Unlicensed under NWO-CS (see UNLICENSE)
 */

using System;

namespace MonoGame.Ruge.CardEngine {

    public enum DeckType { hex, playing, friendly, tarot }
  
    public enum HexSuit { beakers, chips, dna, planets }
    public enum HexRank { _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _A, _B, _C, _D, _E, _F }

    public enum PlayingSuit { clubs, hearts, diamonds, spades }
    public enum PlayingRank { _A, _2, _3, _4, _5, _6, _7, _8, _9, _10, _J, _Q, _K }

    public enum FriendlySuit { carrots, oranges, peppers, strawberries }

    public enum TarotSuit { major, coins, wands, swords, cups }
    public enum TarotRank { _ace, _2, _3, _4, _5, _6, _7, _8, _9, _10, _page, _knight, _queen, _king }
    public enum TarotRankMajor { _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15, _16, _17, _18, _19, _20, _21 }
    
    public enum CardColor { red, black, orange, white, none }

    public class CardType {

        public Enum suit;
        public Enum rank;
        public DeckType deckType;

        public CardType(DeckType deckType) {
            this.deckType = deckType;
        }
        
    }
}