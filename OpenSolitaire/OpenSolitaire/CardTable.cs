/* ©2016 Hathor Gaia 
 * http://HathorsLove.com
 * 
 * Licensed Under GNU GPL 3:
 * http://www.gnu.org/licenses/gpl-3.0.html
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Ruge.CardEngine;
using MonoGame.Ruge.DragonDrop;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace OpenSolitaire.Classic {

    public class CardTable : Table {

        public Deck drawPile { get; set; }
        public Deck discardPile { get; set; }
        
        public bool isSetup = false;
        public bool isAnimating = false;

        private Slot _drawSlot, _discardSlot;
        private List<Stack> playStacks = new List<Stack>();

        private MouseState prevMouseState;

        public Slot drawSlot { get { return _drawSlot; } }
        public Slot discardSlot { get { return _discardSlot; } }

        public CardTable(DragonDrop<IDragonDropItem> dd, Texture2D cardback, Texture2D slot, int stackOffsetH, int stackOffsetV)
            : base(dd, cardback, slot, stackOffsetH, stackOffsetV) {

            // create a fresh card deck
            drawPile = new Deck(cardback, dd.spriteBatch);
            drawPile.freshDeck();
            drawPile.type = StackType.deck;


            discardPile = new Deck(cardback, dd.spriteBatch);
            discardPile.type = StackType.discard;

        }


        public new void Clear() {

            foreach (Slot slot in slots) {
                foreach (Card card in slot.stack.cards) dragonDrop.Remove(card);
                slot.stack.Clear();
            }

            
            drawPile.freshDeck();

        }

        public void InitializeTable() {


            int x = 20;
            int y = 20;

            Slot drawSlot = new Slot(slotTex, cardBack, spriteBatch, new Vector2(x, y));
            Slot discardSlot = new Slot(slotTex, cardBack, spriteBatch, new Vector2(x * 2 + slotTex.Width, y));

            drawSlot.stack = drawPile;
            discardSlot.stack = discardPile;
            drawSlot.type = StackType.draw;
            discardSlot.type = StackType.discard;
            drawSlot.stack.name = "Draw";
            discardSlot.stack.name = "Discard";

            AddSlot(drawSlot);
            AddSlot(discardSlot);

            _drawSlot = drawSlot;
            _discardSlot = discardSlot;

            y += slotTex.Height + y;


            // set up second row of slots
            for (int i = 0; i < 7; i++) {

                Slot newSlot = new Slot(slotTex, cardBack, spriteBatch, new Vector2(x + x * i + slotTex.Width * i, y));
                newSlot.type = StackType.stack;
                newSlot.stack.method = StackMethod.vertical;
                newSlot.stack.name = "Stack " + i;
                AddSlot(newSlot);

            }


            y = 20;

            // set up play/score slots
            for (int i = 6; i >= 3; i--) {

                Slot newSlot = new Slot(slotTex, cardBack, spriteBatch, new Vector2(x + x * i + slotTex.Width * i, y));
                newSlot.type = StackType.play;
                AddSlot(newSlot);

                newSlot.stack.name = "Score " + i;
                
                playStacks.Add(newSlot.stack);
                
            }
                        
        }

        public new void SetTable() {
            
            int x = 20;
            int y = 20;

            drawPile.shuffle();

            foreach (var card in drawPile.cards) {
                
                if (!card.returnToOrigin) { 
                    card.Position = slots[0].Position;
                    card.origin = card.Position;
                    card.Selected += OnCardSelected;
                    card.Deselected += OnCardDeselected;
                    card.Collusion += OnCollusion;
                }
            }
            
            
            y += slotTex.Height + y;
            
            for (var i = 0; i < 7; i++) {
                
                var pos = new Vector2(x + x * i + slotTex.Width * i, y);
                var moveCard = drawPile.drawCard();
                moveCard.origin = pos;
                moveCard.returnToOrigin = true;
                moveCard.snapSpeed = 6.0f;
                moveCard.IsDraggable = false;
                moveCard.MoveStack(slots[i+2].stack);
                slots[i+2].stack.addCard(moveCard);

                for (var j = 1; j < i + 1; j++) {

                    moveCard = drawPile.drawCard();
                    moveCard.origin = new Vector2(pos.X, pos.Y + (stackOffsetVertical * j));
                    moveCard.returnToOrigin = true;
                    moveCard.snapSpeed = 6.0f;
                    moveCard.IsDraggable = false;
                    slots[i+2].stack.addCard(moveCard);

                }
                

            }
            
            isSetup = true;
            
        }
        

        private void OnCollusion(object sender, CollusionEvent e) {


            var type = e.item.GetType();

            if (type == typeof(Card)) {

                Card card, destination;
                
                var card1 = (Card)sender;
                var card2 = (Card)e.item;


                Console.WriteLine("??" + card1.suit.ToString() + card1.rank + " ?? " + card2.suit + card2.rank);

                // the secret sauce to getting this working ;)

                if (card1.Position != card1.origin) {
                    card = card1;
                    destination = card2;
                }
                else {
                    destination = card1;
                    card = card2;
                }

                if (card.isFaceUp && destination.isFaceUp && destination == destination.stack.topCard()) {              
                          
                    Console.WriteLine(card.suit.ToString() + card.rank + " -> " + destination.suit + destination.rank);

                    if (destination.stack.type == StackType.play && card.color == destination.color) card.SetParent(destination);
                    else if (destination.stack.type == StackType.stack && card.color != destination.color) card.SetParent(destination);
                    
                }

            }
            else if (type == typeof(Slot)) {


                Card card = (Card)sender;
                Slot slot = (Slot)e.item;

                if (slot.stack.Count == 0) { 

                    if (slot.type == StackType.play && card.rank == Rank._A && card.Child == null) card.MoveStack(slot.stack);
                    if (slot.type == StackType.stack && card.rank == Rank._K) card.MoveStack(slot.stack);

                    Console.WriteLine(card.suit.ToString() + card.rank + " -> " + slot.type);
                }

            }

        }

        private void OnCardSelected(object sender, EventArgs eventArgs) {

            Card card = (Card)sender;

            if (card.IsDraggable) {
                isAnimating = true;
            }

        }
        private void OnCardDeselected(object sender, EventArgs eventArgs) {


        }

        
        public void Update(GameTime gameTime) {


            foreach (Slot slot in slots) {

                Card topCard = slot.stack.topCard();

                if (topCard != null) topCard.IsDraggable = true;

                slot.Update(gameTime);

            }

            if (isSetup) {

                isAnimating = false;

                foreach (var slot in slots) { 

                    slot.Update(gameTime);

                    foreach (var card in slot.stack.cards) {
                        if (card.returnToOrigin) isAnimating = true;
                        card.Update(gameTime);
                    }
                
                }


                foreach (var slot in slots) {

                    if (!isAnimating && (slot.type == StackType.stack)) {

                        if (slot.stack.Count > 0) {

                            var topCard = slot.stack.topCard();

                            if (!topCard.isFaceUp) {
                                topCard.flipCard();
                                topCard.IsDraggable = true;
                                topCard.snapSpeed = 25f;
                            }

                        }

                    }

                }

            }


            if (isSetup && !isAnimating) {

                foreach (var slot in slots) {
                    
                    if (slot.stack.type == StackType.stack) { 

                        var card = slot.stack.topCard();

                        if (card != null) {

                            card.stackIndex = 0;
                            card.CryingChild();

                        }
                    }
                }

                var mouseState = Mouse.GetState();
                var point = dragonDrop.viewport.PointToScreen(mouseState.X, mouseState.Y);

                if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released) {

                    if (drawSlot.Border.Contains(point)) {

                        if (drawPile.Count > 0) {
                            
                            foreach (var disCard in discardPile.cards) {
                                disCard.IsDraggable = false;
                            }
                            
                            var card = drawPile.drawCard();
                            card.Position = discardSlot.Position;
                            card.flipCard();
                            card.origin = card.Position;
                            card.IsDraggable = true;
                            discardPile.addCard(card);

                        }
                        else if (drawPile.Count == 0) {

                            while (discardPile.Count > 0) {

                                Card disCard = discardPile.drawCard();

                                disCard.flipCard();
                                disCard.Position = drawSlot.Position;
                                disCard.origin = drawSlot.Position;
                                disCard.IsDraggable = false;

                                drawPile.addCard(disCard);

                            }

                        }

                    }

                }
                prevMouseState = mouseState;
            }




        }

    }
}
