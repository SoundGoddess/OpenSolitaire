/* ©2016 Hathor Gaia 
* http://HathorsLove.com
* 
* Source code licensed under GPL-3
* Assets licensed seperately (see LICENSE.md)
*/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Ruge.CardEngine;
using MonoGame.Ruge.DragonDrop;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace OpenSolitaire.Classic {
    class TableClassic : Table {

        public bool isSetup = false;
        public bool isSnapAnimating = false;
        
        public Deck drawPile { get; set; }
        public Stack discardPile { get; set; }

        public Slot drawSlot { get; set; }
        public Slot discardSlot { get; set; }

        private MouseState prevMouseState;
        SoundEffect tableAnimationSound, cardParentSound, cardPlaySound, restackSound, winSound;

        private double clickTimer;
        private const double DELAY = 500;


        public TableClassic(SpriteBatch spriteBatch, DragonDrop<IDragonDropItem> dd, Texture2D cardBack, Texture2D slotTex, int stackOffsetH, int stackOffsetV, List<SoundEffect> soundFX)
            : base(spriteBatch, dd, cardBack, slotTex, stackOffsetH, stackOffsetV) {

            tableAnimationSound = soundFX[0];
            cardParentSound = soundFX[1];
            cardPlaySound = soundFX[2];
            restackSound = soundFX[3];
            winSound = soundFX[4];

            // create a fresh card deck
            drawPile = new Deck(cardBack, slotTex, spriteBatch, stackOffsetH, stackOffsetV) { type = StackType.deck };
            drawPile.freshDeck();
            
        }


        public void NewGame() {

            foreach (var stack in stacks) {
                foreach (var card in stack.cards) dragonDrop.Remove(card);
                stack.Clear();
            }


            drawPile.freshDeck();
            drawPile.shuffle();
            drawPile.UpdatePositions();


            foreach (var card in drawPile.cards) {
                dragonDrop.Add(card);
            }

        }

        internal void InitializeTable() {
            
            foreach (var card in drawPile.cards) {
                dragonDrop.Add(card);
            }

            drawPile.shuffle();

            int x = 20;
            int y = 20;

            drawSlot = new Slot(slotTex, spriteBatch) {
                name = "Draw",
                Position = new Vector2(x, y),
                stack = drawPile
            };
            discardSlot = new Slot(slotTex, spriteBatch) {
                name = "Discard",
                Position = new Vector2(x * 2 + slotTex.Width, y),
                stack = discardPile
            };

            dragonDrop.Add(drawSlot);
            dragonDrop.Add(discardSlot);

            drawPile.slot = drawSlot;
            AddStack(drawPile);

            discardPile = AddStack(discardSlot, StackType.discard);
            

            y += slotTex.Height + y;


            // set up second row of slots
            for (int i = 0; i < 7; i++) {

                // add crunch for these stacks
                var newSlot = new Slot(slotTex, spriteBatch) {
                    Position = new Vector2(x + x*i + slotTex.Width*i, y),
                    name = "Stack " + i
                };
                
                var newStack = AddStack(newSlot, StackType.stack, StackMethod.vertical);
                newStack.crunchItems = 12;

            }


            y = 20;

            // set up play/score slots
            for (int i = 6; i >= 3; i--) {
                
                var newSlot = new Slot(slotTex, spriteBatch) {
                    Position = new Vector2(x + x * i + slotTex.Width * i, y),
                    name = "Play " + i
                };

                AddStack(newSlot, StackType.play);
                
            }

        }


        public new void SetTable() {

            
            foreach (var card in drawPile.cards) {
                card.Selected += OnCardSelected;
                card.Collusion += OnCollusion;
                card.stack = drawPile;
            }

            int x = 20;
            int y = 20;
            y += slotTex.Height + y;

            for (var i = 0; i < 7; i++) {

                var pos = new Vector2(x + x * i + slotTex.Width * i, y);
                var moveCard = drawPile.drawCard();
                moveCard.snapPosition = pos;
                moveCard.isSnapAnimating = true;
                moveCard.snapSpeed = 6.0f;
                moveCard.IsDraggable = false;
                stacks[i+2].addCard(moveCard);

                for (var j = 1; j < i + 1; j++) {

                    moveCard = drawPile.drawCard();
                    moveCard.snapPosition = new Vector2(pos.X, pos.Y + stackOffsetVertical * j);
                    moveCard.isSnapAnimating = true;
                    moveCard.snapSpeed = 6.0f;
                    moveCard.IsDraggable = false;
                    stacks[i + 2].addCard(moveCard);

                }


            }

            tableAnimationSound.Play();

            isSetup = true;
        }



        private void OnCollusion(object sender, Card.CollusionEvent e) {


            var type = e.item.GetType();

            if (type == typeof(Card)) {

                Card card, destination;

                var card1 = (Card)sender;
                var card2 = (Card)e.item;

                //Console.WriteLine("??" + card1.suit.ToString() + card1.rank + " ?? " + card2.suit + card2.rank);

                if (card1.Position != card1.snapPosition) {
                    card = card1;
                    destination = card2;
                }
                else {
                    destination = card1;
                    card = card2;
                }

                var topCard = destination.stack.topCard();

                if (card.isFaceUp && destination.isFaceUp && destination == topCard) {

                    Console.WriteLine(card.suit.ToString() + card.rank + " -> " + destination.suit + destination.rank);

                    
                    if (destination.stack.type == StackType.play && card.suit == destination.suit &&
                        card.rank == destination.rank + 1) {
                        card.SetParent(destination);
                        cardPlaySound.Play();
                    }
                    else if (destination.stack.type == StackType.stack && card.color != destination.color &&
                        card.rank == destination.rank - 1) {
                        card.SetParent(destination);
                        cardParentSound.Play(.6f, 1f, 1f);
                    }
                    

                    // todo: delete after testing
                    //card.SetParent(destination);

                }

            }
            else if (type == typeof(Slot)) {

                var card = (Card)sender;
                var slot = (Slot)e.item;


                //Console.WriteLine("(debug) " + card.suit.ToString() + card.rank + " -> " + slot.stack.type);

                if (slot.stack.Count == 0) {

                    if (slot.stack.type == StackType.play && card.rank == Rank._A && card.Child == null) {
                        card.MoveToEmptyStack(slot.stack);
                        cardPlaySound.Play();
                    }
                    if (slot.stack.type == StackType.stack && card.rank == Rank._K) {
                        card.MoveToEmptyStack(slot.stack);
                        cardParentSound.Play(.6f, 1f, 1f);
                    }

                    Console.WriteLine(card.suit.ToString() + card.rank + " -> " + slot.stack.type);
                }

            }

        }


        private void OnCardSelected(object sender, EventArgs eventArgs) {

            var card = (Card)sender;

            if (card.IsDraggable) {
                isSnapAnimating = true;
            }

        }
        

        public new void Update(GameTime gameTime) {

            if (isSetup) {

                isSnapAnimating = false;

                foreach (var stack in stacks) {
                    
                    foreach (var card in stack.cards) {
                        if (card.isSnapAnimating) isSnapAnimating = true;
                    }

                }


                if (!isSnapAnimating) {


                    foreach (var stack in stacks) {

                        if (stack.Count > 0) {
                            if (stack.type == StackType.stack) {
                                var topCard = stack.topCard();

                                if (!topCard.isFaceUp) {
                                    topCard.flipCard();
                                    topCard.IsDraggable = true;
                                    topCard.snapSpeed = 25f;
                                    topCard.ZIndex = stack.Count;
                                }
                            }
                            int i = 1;

                            foreach (var card in stack.cards) {
                                if (!card.IsSelected) card.ZIndex = i;
                                i++;
                            }
                        }
                    }


                    var mouseState = Mouse.GetState();
                    var point = dragonDrop.viewport.PointToScreen(mouseState.X, mouseState.Y);

                    if (mouseState.LeftButton == ButtonState.Pressed &&
                        prevMouseState.LeftButton == ButtonState.Released) {

                        if (drawSlot.Border.Contains(point)) {

                            if (drawPile.Count > 0) {

                                var card = drawPile.drawCard();

                                card.ZIndex = discardPile.Count;

                                card.Position = discardSlot.Position;
                                card.flipCard();
                                card.snapPosition = card.Position;
                                card.IsDraggable = true;
                                discardPile.addCard(card);
                                cardParentSound.Play(.6f, 1f, 1f);

                            }
                            else if (drawPile.Count == 0) {

                                while (discardPile.Count > 0) {

                                    var disCard = discardPile.drawCard();

                                    disCard.ZIndex = 1;
                                    disCard.flipCard();
                                    disCard.Position = drawSlot.Position;
                                    disCard.snapPosition = drawSlot.Position;
                                    disCard.IsDraggable = false;

                                    drawPile.addCard(disCard);

                                }
                                if (drawPile.Count > 1) { 
                                    var restackAnimation = drawPile.topCard();
                                    restackAnimation.Position += new Vector2(stackOffsetHorizontal * 2,0);
                                    restackAnimation.isSnapAnimating = true;
                                    restackAnimation.snapSpeed = 3f;
                                    restackSound.Play();
                                }
                                else cardParentSound.Play(.6f, 1f, 1f);
                            }

                        }

                    }
                    
                    clickTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (mouseState.LeftButton == ButtonState.Pressed &&
                        prevMouseState.LeftButton == ButtonState.Released) {

                        if (clickTimer < DELAY) {

                            // check for double-click event
                            foreach (var stack in stacks) {

                                if (stack.Count > 0) { 
                                    if (stack.type == StackType.stack || stack.type == StackType.discard) {

                                        var topCard = stack.topCard();

                                        if (topCard.Border.Contains(point) && topCard.Child == null) {

                                            foreach (var playStack in stacks) {

                                                if (playStack.type == StackType.play) {

                                                    if (playStack.Count > 0) {

                                                        var playStackTop = playStack.topCard();

                                                        if (topCard.suit == playStackTop.suit && topCard.rank == playStackTop.rank + 1) {
                                                            topCard.SetParent(playStackTop);
                                                            cardPlaySound.Play();
                                                        }
                                                    }
                                                    else if (topCard.rank == Rank._A) {
                                                        topCard.MoveToEmptyStack(playStack);
                                                        cardPlaySound.Play();
                                                    }

                                                }

                                            }

                                            Console.WriteLine("double-click: " + topCard.suit.ToString() + topCard.rank);
                                        
                                        }
                                    }
                                }

                            }


                        }
                        clickTimer = 0;
                    }

                    prevMouseState = mouseState;
                }
            }

            base.Update(gameTime);
        }

    }
}
