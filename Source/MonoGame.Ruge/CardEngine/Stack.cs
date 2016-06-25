/* Attribution (a) 2016 The Ruge Project (http://ruge.metasmug.com/) 
 * Unlicensed under NWO-CS (see UNLICENSE)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Ruge.CardEngine {
    
    /// <summary>
    /// a normal stack is a pile of cards placed on top of each other.
    /// an example of a horizontal stack would be a hand of cards
    /// and example of a vertical stack would be a solitaire stack
    /// 
    /// undefined stacks are useful for when you need to create a stack 
    /// object in memory but aren't actually rendering it on the screen.
    /// </summary>
    public enum StackMethod {
        normal,
        horizontal,
        vertical,
        undefined
    }

    /// <summary>
    /// stack types are just general suggestions of the type of stacks 
    /// you may need in your card game. 
    /// 
    /// it's up to you to define what logic you are going to use for
    /// a particular stack type.
    /// </summary>
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
        protected Texture2D cardBack;

        public List<Card> cards = new List<Card>();

        /// <summary>counts your card objects</summary>
        public int Count => cards.Count;

        /// <summary>clears your card objects</summary>
        public void Clear() {
            foreach (var card in cards) card.render = false; // hack for annoying issue
            cards.Clear();
        } 
            

        // defining what type of stack it is
        public StackType type = StackType.hand;
        public StackMethod method = StackMethod.normal;

        // the stack has a slot object; the slot defines where
        // on the board you can place a stack.
        public Slot slot { get; set; }
        public Table table { get; set; }

        // naming the stack; mostly for debugging purposes.
        // you want the stack name to be the same as the slot
        // name or else things can get really confusing.
        public string name => slot.name;

        // when the cards are stacked horizontally or vertically,
        // these tell the renderer how much of an offset to move
        // the children cards by.
        //
        // the values may be different depending on what assets
        // you end up using for your card deck.
        protected int stackOffsetHorizontal, stackOffsetVertical;

        // the stack offset vector is calculated here
        public Vector2 offset {
            get {

                switch (method) {
                    case StackMethod.horizontal: return new Vector2(stackOffsetHorizontal, 0);
                    case StackMethod.vertical:   return new Vector2(0, stackOffsetVertical);
                    default:                     return Vector2.Zero;
                }
            }
        }
        
        /// <summary>
        /// chrunchItems is the number of items in your stack before you 
        /// need to crunch for example, if it looks fine with 5 items, 
        /// but with 6 items the stack is too tall or wide, set this 
        /// number to 6.
        /// </summary>
        public int crunchItems { get; set; } = 0;

        /// <summary> Let's say you're adding cards to a stack and the 
        /// stack has too many cards so it starts to render 
        /// off screen. You can decrease the stack's rendering
        /// offset to compensate for this. </summary>
        public bool crunchStacks = false;

        /// <summary>
        /// stack constructor.  this creates a card slot and assigns it to the stack.
        /// </summary>
        /// <param name="table">the table object</param>
        /// <param name="cardBack">the texture asset for when your cards are face down</param>
        /// <param name="slotTex">the texture asset for your slot</param>
        /// <param name="spriteBatch">to allow you to implement the Draw() method</param>
        /// <param name="stackOffsetH">if your stack will render horizontally, this is how much space to push the cards by</param>
        /// <param name="stackOffsetV">if your stack will render vertically, this is how much space to push the cards by</param>
        public Stack(Table table, Texture2D cardBack, Texture2D slotTex, SpriteBatch spriteBatch, int stackOffsetH, int stackOffsetV) {
            this.table = table;
            slot = new Slot(slotTex,spriteBatch) {stack = this};
            this.cardBack = cardBack;
            this.spriteBatch = spriteBatch;
            stackOffsetHorizontal = stackOffsetH;
            stackOffsetVertical = stackOffsetV;
        }

        /// <summary>
        /// use this to shuffle the card deck
        /// </summary>
        public void shuffle() {

            // wait a few ms to avoid seed collusion; otherwise
            // the random method will return the same value twice
            Thread.Sleep(30);

            var rand = new Random();
            for (int i = cards.Count - 1; i > 0; i--) {
                int randomIndex = rand.Next(i + 1);
                var tempCard = cards[i];
                cards[i] = cards[randomIndex];
                cards[randomIndex] = tempCard;
            }
        }

        #region public methods

        /// <summary>
        /// returns the card object for the top card on the stack
        /// </summary>
        /// <returns>the top card on the pile</returns>
        public Card topCard() {
            cards = cards.OrderBy(z => z.ZIndex).ToList();
            return cards?.Last();
        }
        
        /// <summary>
        /// removes the top card from the stack and returns it
        /// </summary>
        /// <returns>the card you drew</returns>
        public Card drawCard() {

            if (cards.Count > 0) {
                
                var topCard = cards[cards.Count - 1];
                cards.RemoveAt(cards.Count - 1);
                return topCard;

            }
            return null;
        }

        /// <summary>
        /// add the card to the stack
        /// </summary>
        /// <param name="card">the card you wish to add to the stack</param>
        /// <param name="update">if necessary to fix the rendering of the stack, pass true</param>
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

        /// <summary>
        /// you'll want to call this if you're using crunching.
        /// todo: this code really could use some clean-up
        /// </summary>
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
        
        #endregion

        #region private methods

        /// <summary>
        /// this destroys any parent/child relationships 
        /// that exist for the card being passed in.
        /// 
        /// it's very problematic if your card ever ends 
        /// up having more than one parent.
        /// </summary>
        /// <param name="nukeMe"></param>
        private void NukeParents(Card nukeMe) {
            foreach (var card in cards)
                if (card.Child == nukeMe) card.Child = null;
        }
        #endregion
        


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



        /// <summary>
        /// spits out a bunch of debugging info to your Debug
        /// </summary>
        public void debug() {

            Debug.WriteLine("========");
            Debug.WriteLine(name);

            if (cards.Count > 0) {

                Card top = topCard();
                string strFaceUp = top.isFaceUp ? "face up" : "face down";
                Debug.WriteLine("top " + "z" + top.ZIndex.ToString("00") + ": " + top.rank + " of " + top.suit + " (" + strFaceUp + ")");


                foreach (var card in cards) {

                    strFaceUp = (card.isFaceUp ? "face up" : "face down");
                    Debug.Write("z" + card.ZIndex.ToString("00") + ": " + card.rank + " of " + card.suit + " (" + strFaceUp + ")");
                    Debug.Write(" - " + card.stack.name);

                    if (card.Child != null) {
                        strFaceUp = (card.Child.isFaceUp ? "face up" : "face down");
                        Debug.Write(" -> z" + card.Child.ZIndex.ToString("00") + ": " +
                        card.Child.rank + " of " + card.Child.suit + " (" + strFaceUp + ")");
                    }

                    Debug.WriteLine();
                }
            }
            else { Debug.WriteLine("(empty stack)"); }

        }


    }

}
