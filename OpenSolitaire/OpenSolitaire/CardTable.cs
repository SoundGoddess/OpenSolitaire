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
using System.Threading;

namespace OpenSolitaire {

    public class CardTable : Table {

        public Deck drawPile { get; set; }
        public Deck discardPile { get; set; }
        
        public bool isSetup = false;
        public bool isAnimating = false;

        private Slot _drawSlot, _discardSlot;

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

            foreach (Stack stack in stacks) {
                foreach (Card card in stack.cards) _dragonDrop.Remove(card);
                stack.Clear();
            }


            stacks.Clear();

            drawPile.freshDeck();

        }

        public void InitializeTable() {


            int x = 20;
            int y = 20;

            Slot drawSlot = new Slot(_slot, _spriteBatch, new Vector2(x, y));
            Slot discardSlot = new Slot(_slot, _spriteBatch, new Vector2(x * 2 + _slot.Width, y));

            drawSlot.type = SlotType.draw;
            discardSlot.type = SlotType.discard;

            AddSlot(drawSlot);
            AddSlot(discardSlot);

            _drawSlot = drawSlot;
            _discardSlot = discardSlot;

            y += _slot.Height + y;


            // set up second row of slots
            for (int i = 0; i < 7; i++) {

                Slot newSlot = new Slot(_slot, _spriteBatch, new Vector2(x + x * i + _slot.Width * i, y));
                newSlot.type = SlotType.stack;
                AddSlot(newSlot);

            }


            y = 20;

            // set up play/score slots
            for (int i = 6; i >= 3; i--) {

                Slot newSlot = new Slot(_slot, _spriteBatch, new Vector2(x + x * i + _slot.Width * i, y));
                newSlot.type = SlotType.play;
                AddSlot(newSlot);

            }

        }

        public new void SetTable() {
            
            int x = 20;
            int y = 20;

            drawPile.shuffle();

            foreach (Card card in drawPile.cards) {
                
                if (!card.returnToOrigin) { 
                    card.Position = slots[0].Position;
                    card.origin = card.Position;
                    card.Selected += OnCardSelected;
                    card.Deselected += OnCardDeselected;
                }
            }

            AddStack(drawPile);
            AddStack(discardPile);

            
            y += _slot.Height + y;
            
            for (int i = 0; i < 7; i++) {
                
                Stack stack = new Stack(_cardBack, _spriteBatch);
                stack.type = StackType.stack;
                Vector2 pos = new Vector2(x + x * i + _slot.Width * i, y);
                Card moveCard = drawPile.drawCard();
                moveCard.origin = pos;
                moveCard.returnToOrigin = true;
                moveCard.snapSpeed = 4.0f;
                moveCard.IsDraggable = false;
                stack.addCard(moveCard);

                for (int j = 1; j < i + 1; j++) {

                    moveCard = drawPile.drawCard();
                    moveCard.origin = new Vector2(pos.X, pos.Y + (_stackOffsetVertical * j));
                    moveCard.returnToOrigin = true;
                    moveCard.snapSpeed = 4.0f;
                    moveCard.IsDraggable = false;
                    stack.addCard(moveCard);

                }

                AddStack(stack);

            }

            isSetup = true;
            
        }

        private void OnCardSelected(object sender, EventArgs eventArgs) {

            Card card = (Card)sender;

            if (card.IsDraggable) {
                card.IsSelected = true;
                isAnimating = true;
            }

        }
        private void OnCardDeselected(object sender, EventArgs eventArgs) {

            Card card = (Card)sender;

            card.IsSelected = false;

            if (card.Position != card.origin) card.returnToOrigin = true;

        }

        
        public void Update(GameTime gameTime) {

            if (isSetup) {

                foreach (Slot slot in this.slots) slot.Update(gameTime);

                foreach (Stack stack in this.stacks) {

                    isAnimating = false;

                    foreach (Card card in stack.cards) {
                        if (card.returnToOrigin) isAnimating = true;
                        card.Update(gameTime);
                    }
                
                }


                foreach (Stack stack in this.stacks) {

                    if (!isAnimating && (stack.type == StackType.stack)) {

                        if (stack.Count > 0) {

                            Card topCard = stack.cards[stack.Count - 1];

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

                MouseState mouseState = Mouse.GetState();
                Point point = _dragonDrop.viewport.PointToScreen(mouseState.X, mouseState.Y);

                if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released) {

                    if (drawSlot.Border.Contains(point)) {

                        if (drawPile.Count > 0) {

                            foreach (Card disCard in discardPile.cards) {
                                disCard.IsDraggable = false;
                            }
                            
                            Card card = drawPile.drawCard();
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
