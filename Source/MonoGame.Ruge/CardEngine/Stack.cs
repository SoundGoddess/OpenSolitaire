/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)

 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using MonoGame.Ruge.DragonDrop;
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

        public int crunchItems { get; set; } = 0;
        public bool crunchStacks = false;


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


        private void NukeParents(Card nukeMe) {

            foreach (var card in cards)
                if (card.Child == nukeMe) card.Child = null;

        }


        public void addCard(Card card, bool update = false) {
            
            if (card.stack != null) { 
                card.stack.cards.Remove(card);
                card.stack.NukeParents(card);
            }
            card.stack = this;
            cards.Add(card);
            card.ZIndex = Count + 1;
            
            var fixChild = card.Child;

            while (fixChild != null) {

                if (fixChild.stack != null) {
                    fixChild.stack.cards.Remove(fixChild);
                    fixChild.stack.NukeParents(fixChild);
                }
                
                fixChild.stack = this;

                cards.Add(fixChild);

                fixChild = fixChild.Child;

            }


            int i = 0;

            foreach (var fixIndex in cards) fixIndex.ZIndex = i++;
            
            if (update) UpdatePositions();
        }

        
        public void UpdatePositions() {
            
            int i = 0;
            int numFaceDown = 0;
            
            cards = cards.OrderBy(z => z.ZIndex).ToList();
            foreach (var card in cards) {

                if (!card.isFaceUp) numFaceDown++;

                var stackOffestX = offset.X;
                var stackOffestY = offset.Y;

                crunchStacks = false;

                // the stack has a lot of items so crunch
                if (crunchItems > 0 && cards.Count >= crunchItems) {

                    if (card.isFaceUp) {
                            
                        stackOffestX = (stackOffestX > 0) ? stackOffestX - 3 : 0;
                        stackOffestY = (stackOffestY > 0) ? stackOffestY - 3 : 0;
                            
                    }
                    else {
                        stackOffestX = stackOffestX / 2;
                        stackOffestY = stackOffestY / 2;
                    }
                    crunchStacks = true;
                   
                }


                var newCardX = slot.Position.X + stackOffestX * i;
                var newCardY = slot.Position.Y + stackOffestY * i;

                if (card.isFaceUp && crunchItems > 0 && cards.Count >= crunchItems) {
                    newCardX -= offset.X * numFaceDown / 2 - offset.X / 2;
                    newCardY -= offset.Y * numFaceDown / 2 - offset.Y / 2;

                    if (numFaceDown == 0) {
                        newCardX -= offset.X / 2;
                        newCardY -= offset.Y / 2;
                    }

                }

                card.Position = new Vector2(newCardX, newCardY);
                card.snapPosition = card.Position;

                i++;

            }

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


        #region MonoGame

        public void Update(GameTime gameTime) {
            slot.Update(gameTime);

            if (crunchStacks) {
                foreach (var card in cards) card.Update(gameTime);
                if (cards.Count < crunchItems) UpdatePositions();
            }

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
                    Console.Write(" - " + card.stack.name);
                    
                    if (card.Child != null) {
                        strFaceUp = (card.Child.isFaceUp ? "face up" : "face down");
                        Console.Write(" -> z" + card.Child.ZIndex.ToString("00") + ": " +
                        card.Child.rank + " of " + card.Child.suit + " (" + strFaceUp + ")");
                    }
                    
                    Console.WriteLine();
                }
            }
            else { Console.WriteLine("(empty stack)"); }

        }
    }

}
