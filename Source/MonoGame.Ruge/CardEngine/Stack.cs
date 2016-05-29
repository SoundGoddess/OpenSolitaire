/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)

 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Ruge.CardEngine {
    
    public enum StackMethod {
        normal,
        horizontal,
        vertical,
        undefined
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

    public class Stack  {

        protected SpriteBatch spriteBatch;
        public Texture2D cardBack;
        
        public List<Card> cards = new List<Card>();
        
        public int Count => cards.Count;

        public StackType type = StackType.hand;
        public StackMethod method = StackMethod.normal;

        public string name => slot.name;

        protected int stackOffsetHorizontal, stackOffsetVertical;

        public Vector2 offset {
            get {

                switch (method) {
                    case StackMethod.horizontal: return new Vector2(stackOffsetHorizontal, 0);
                    case StackMethod.vertical:   return new Vector2(0, stackOffsetVertical);
                    default:                     return Vector2.Zero;
                }
            }
        }

        public Slot slot { get; set; }
        public void Clear() { cards.Clear(); }


        public Stack(Texture2D cardBack, Texture2D slotTex, SpriteBatch spriteBatch, int stackOffsetH, int stackOffsetV) {
            slot = new Slot(slotTex,spriteBatch) {stack = this};
            this.cardBack = cardBack;
            this.spriteBatch = spriteBatch;
            stackOffsetHorizontal = stackOffsetH;
            stackOffsetVertical = stackOffsetV;
        }


        public void shuffle() {

            //wait a few ms to avoid seed collusion
            Thread.Sleep(30);

            var rand = new Random();
            for (int i = cards.Count - 1; i > 0; i--) {
                int randomIndex = rand.Next(i + 1);
                var tempCard = cards[i];
                cards[i] = cards[randomIndex];
                cards[randomIndex] = tempCard;
            }
        }

        /// <summary>
        /// just picks the top card on the stack and returns it
        /// </summary>
        /// <returns></returns>
        public Card topCard() {
            cards = cards.OrderBy(z => z.ZIndex).ToList();
            return cards?.Last();
        }


        public void addCard(Card card, bool update = false) {
            card.stack = this;
            cards.Add(card);
            card.ZIndex = Count + 1;
            if (update) UpdatePositions();
        }
        
        /// <summary>
        /// just picks the top card on the stack and returns it
        /// </summary>
        /// <returns></returns>
        public Card drawCard() {

            if (cards.Count > 0) {

                var topCard = cards[cards.Count - 1];
                cards.RemoveAt(cards.Count - 1);
                return topCard;

            }
            return null;
        }


        public void UpdatePositions(StackMethod stackMethod = StackMethod.undefined) {

            if (stackMethod != StackMethod.undefined) method = stackMethod;

            cards = cards.OrderBy(z => z.ZIndex).ToList();

            int i = 0;

            foreach (var card in cards) {

                card.Position = new Vector2(slot.Position.X + offset.X * i, slot.Position.Y + offset.Y * i);
                card.snapPosition = card.Position;

                i++;

            }

        }

        #region MonoGame

        public void Update(GameTime gameTime) {
            slot.Update(gameTime);
        }
        
        public void Draw(GameTime gameTime) {
            slot.Draw(gameTime);
        }

        #endregion
        

        public void debug() {

            Console.WriteLine("========");
            Console.WriteLine(name);

            if (cards.Count > 0) {

                Card top = topCard(); 
                string strFaceUp = top.isFaceUp ? "face up" : "face down";
                Console.WriteLine("top " + "z" + top.ZIndex.ToString("00") + ": " + top.rank + " of " + top.suit + " (" + strFaceUp + ")");


                foreach (var card in cards) {

                    strFaceUp = (card.isFaceUp ? "face up" : "face down");
                    Console.Write("z" + card.ZIndex.ToString("00") + ": " + card.rank + " of " + card.suit + " (" + strFaceUp + ")");
                    
                    if (card.Child != null) {
                        strFaceUp = (card.Child.isFaceUp ? "face up" : "face down");
                        Console.Write(" - z" + card.Child.ZIndex.ToString("00") + ": " +
                        card.Child.rank + " of " + card.Child.suit + " (" + strFaceUp + ")");
                    }
                    
                    Console.WriteLine();
                }
            }
            else { Console.WriteLine("(empty stack)"); }

        }
    }

}
