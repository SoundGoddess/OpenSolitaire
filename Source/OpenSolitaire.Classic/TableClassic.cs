/* ©2016 Hathor Gaia 
* http://HathorsLove.com
* 
* Source code licensed under NWO-SA
* Assets licensed seperately (see LICENSE.md)
*/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Ruge.CardEngine;
using MonoGame.Ruge.DragonDrop;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Ruge.Glide;
// ReSharper disable AccessToStaticMemberViaDerivedType

namespace OpenSolitaire.Classic {

    public enum GameState { active, complete, won }

    class TableClassic : Table {

        public bool isSetup;
        private bool isSnapAnimating;
        public bool isAnimating {
            get {
                if (!isSnapAnimating) return animationCount > 0;
                else return true;
            }
        }

        private int animationCount = 28;

        
        public Deck drawPile { get; set; }
        public Stack discardPile { get; set; }

        public Slot drawSlot { get; set; }
        public Slot discardSlot { get; set; }

        private MouseState prevMouseState;
        SoundEffect kickSound, parentSound, playSound, restackSound, undoSound, winSound, bounceSound;

        private double clickTimer;
        private const double DELAY = 500;

        public bool muteSound {
            get { return Properties.Settings.Default.mute; }
            set { Properties.Settings.Default.mute = value; }
        }

        private Tweener tween = new Tweener();
        private Game game;

        private Card saveCard;
        private Stack saveStack;

        public GameState gameState = GameState.active;
        public bool solve;

        public TableClassic(Game game, SpriteBatch spriteBatch, DragonDrop<IDragonDropItem> dd, Texture2D cardBack, Texture2D slotTex, int stackOffsetH, int stackOffsetV, List<SoundEffect> soundFX)
            : base(spriteBatch, dd, cardBack, slotTex, stackOffsetH, stackOffsetV) {

            this.game = game;

            kickSound = soundFX[0];
            parentSound = soundFX[1];
            playSound = soundFX[2];
            restackSound = soundFX[3];
            undoSound = soundFX[4];
            bounceSound = soundFX[5];

            // create a fresh card deck
            drawPile = new Deck(this, DeckType.playing, cardBack, slotTex, spriteBatch, stackOffsetH, stackOffsetV) { type = StackType.deck };
            drawPile.freshDeck();

            Tween.TweenerImpl.SetLerper<Vector2Lerper>(typeof(Vector2));

        }


        public void NewGame(GameTime gameTime) {

            solve = false;
            gameState = GameState.active;

            dragonDrop.Clear();

            foreach (var stack in stacks) {
                stack.Clear();
                dragonDrop.Add(stack.slot);
            }

            drawPile.freshDeck();
            drawPile.shuffle();
            drawPile.UpdatePositions();

            foreach (var card in drawPile.cards) {
                dragonDrop.Add(card);
            }

            animationCount = 28;

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

                var newSlot = new Slot(slotTex, spriteBatch) {
                    Position = new Vector2(x + x*i + slotTex.Width*i, y),
                    name = "Stack " + i
                };
                
                var newStack = AddStack(newSlot, StackType.stack, StackMethod.vertical);

                // add crunch for these stacks
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
                card.Save += Save;
                card.stack = drawPile;
            }

            int x = 20;
            int y = 20;
            y += slotTex.Height + y;

            for (var i = 0; i < 7; i++) {

                var newX = x + x * i + slotTex.Width * i;
                var pos = new Vector2(newX, y);
                var moveCard = drawPile.drawCard();
                moveCard.Position = new Vector2(newX, 0 - moveCard.Texture.Height);

                if (i == 0) {

                    tween.Tween(moveCard, new { Position = pos }, 7, 40)
                        .OnComplete(afterTween)
                        .Ease(Ease.ElasticOut);
                }
                else {

                    var delay = 3f + i * 2.5f;

                    tween.Tween(moveCard, new { Position = pos }, 5, delay)
                        .Ease(Ease.BackOut)
                        .OnComplete(afterTween);
                }

                moveCard.snapPosition = pos;
                moveCard.IsDraggable = false;

                stacks[i + 2].addCard(moveCard);

                for (var j = 1; j < i + 1; j++) {

                    moveCard = drawPile.drawCard();
                    moveCard.snapPosition = new Vector2(pos.X, pos.Y + stackOffsetVertical * j);
                    moveCard.Position = new Vector2(newX, 0 - moveCard.Texture.Height);

                    if (j == i) {
                        tween.Tween(moveCard, new { Position = moveCard.snapPosition }, 7, 40)
                            .OnComplete(afterTween)
                            .Ease(Ease.ElasticOut);
                    }
                    else {

                        var delay = 3f + i * 2.5f + j * 2.5f;

                        tween.Tween(moveCard, new { Position = moveCard.snapPosition }, 5, delay)
                            .Ease(Ease.BackOut)
                            .OnComplete(afterTween);
                    }

                    moveCard.IsDraggable = false;
                    stacks[i + 2].addCard(moveCard);
                }
            }

            var restackAnimation = drawPile.topCard();
            restackAnimation.Position += new Vector2(stackOffsetHorizontal * 2, 0);
            restackAnimation.isSnapAnimating = true;
            restackAnimation.snapTime = 4.5f;
            if (!muteSound) restackSound.Play();

            isSetup = true;
        }

        private void afterTween() {

            animationCount--;
            if (!muteSound) parentSound.Play();
        }

        private void afterTween(Card card) {
            card.flipCard();
            card.snapPosition = card.Position;
            card.IsDraggable = true;
            card.snapTime = .7f;
            discardPile.addCard(card);
            animationCount--;
            if (!muteSound) parentSound.Play();
        }

        private void afterTween(Card card, Card destinationCard) {

            card.ZIndex -= ON_TOP;
            card.isSnapAnimating = false;
            card.SetParent(destinationCard);
            if (!muteSound) playSound.Play(.3f, 0, 0);
            animationCount = 0;
        }

        private void afterTween(Card card, Stack stack) {
            card.ZIndex -= ON_TOP;
            card.isSnapAnimating = false;
            card.MoveToEmptyStack(stack);
            if (!muteSound) playSound.Play(.3f, 0, 0);
            animationCount = 0;
        }

        private void Save(object sender, Card.SaveEvent s) {
            saveCard = (Card)sender;
            saveStack = s.stack;
        }

        private void Save(Card card, Stack stack) {
            saveCard = card;
            saveStack = stack;
        }

        public void Undo(GameTime gameTime) {

            if (saveCard != null && saveStack != null) {

                if (saveStack.Count == 0) saveCard.MoveToEmptyStack(saveStack);

                else if (saveStack.type == StackType.deck) {
                    saveCard.Position = drawSlot.Position;
                    saveCard.flipCard();
                    saveCard.snapPosition = saveCard.Position;
                    saveCard.IsDraggable = false;
                    drawPile.addCard(saveCard);
                }
                else if (saveStack.type == StackType.discard) {
                    saveCard.Position = discardSlot.Position;
                    saveCard.snapPosition = saveCard.Position;
                    discardPile.addCard(saveCard);
                }
                else if (saveStack.type == StackType.stack) {
                    var topCard = saveStack.topCard();
                    topCard.IsDraggable = false;
                    topCard.flipCard();
                    saveStack.addCard(saveCard, true);
                }

                saveCard = null;
                saveStack = null;

                if (!muteSound) undoSound.Play(.6f, 0, 0);

            }
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

                    Debug.WriteLine(card.suit.ToString() + card.rank + " -> " + destination.suit + destination.rank);


                    if (destination.stack.type == StackType.play && Equals(card.suit, destination.suit) &&
                        (PlayingRank)card.rank == (PlayingRank)destination.rank + 1) {
                        card.SetParent(destination);
                        if (!muteSound) playSound.Play(.3f, 0, 0);
                    }
                    else if (destination.stack.type == StackType.stack && card.color != destination.color &&
                        (PlayingRank)card.rank == (PlayingRank)destination.rank - 1) {
                        card.SetParent(destination);

                        if (!muteSound) parentSound.Play();
                    }


                }

            }
            else if (type == typeof(Slot)) {

                var card = (Card)sender;
                var slot = (Slot)e.item;


                //Console.WriteLine("(debug) " + card.suit.ToString() + card.rank + " -> " + slot.stack.type);

                if (slot.stack.Count == 0) {

                    if (slot.stack.type == StackType.play && Equals(card.rank, PlayingRank._A) && card.Child == null) {
                        card.MoveToEmptyStack(slot.stack);
                        if (!muteSound) playSound.Play(.3f, 0, 0);
                    }
                    if (slot.stack.type == StackType.stack && Equals(card.rank, PlayingRank._K)) {
                        card.MoveToEmptyStack(slot.stack);
                        if (!muteSound) parentSound.Play();
                    }

                    Debug.WriteLine(card.suit.ToString() + card.rank + " -> " + slot.stack.type);
                }

            }

        }


        private void OnCardSelected(object sender, EventArgs eventArgs) {

            var card = (Card)sender;

            if (card.IsDraggable) {
                isSnapAnimating = true;
            }

        }

        public void PlayTopCard(Card topCard) {
            foreach (var playStack in stacks) {

                if (playStack.type == StackType.play) {

                    if (playStack.Count > 0) {

                        var playStackTop = playStack.topCard();

                        if (Equals(topCard.suit, playStackTop.suit) &&
                            (PlayingRank)topCard.rank == (PlayingRank)playStackTop.rank + 1) {

                            Vector2 pos = playStackTop.Position;

                            // check again, since apparently if you click too fast it gets confused.
                            if (!isAnimating) {
                                animationCount++;
                                topCard.ZIndex += ON_TOP;

                                tween.Tween(topCard, new { Position = pos }, 3)
                                    .Ease(Ease.CubeInOut)
                                    .OnComplete(() => afterTween(topCard, playStackTop));
                            }

                        }
                    }
                    else if (Equals(topCard.rank, PlayingRank._A)) {

                        Vector2 pos = playStack.slot.Position;

                        // check again, since apparently if you click too fast it gets confused.
                        if (!isAnimating) {
                            animationCount++;
                            topCard.ZIndex += ON_TOP;

                            tween.Tween(topCard, new { Position = pos }, 3)
                                .Ease(Ease.CubeInOut)
                                .OnComplete(() => afterTween(topCard, playStack));
                        }
                    }
                }
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


                if (!isAnimating) {

                    var faceDown = 0;

                    foreach (var stack in stacks)
                        foreach (var card in stack.cards)
                            if (!card.isFaceUp) faceDown++;

                    if (faceDown == 0 && discardPile.Count == 0 && drawPile.Count == 0) gameState = GameState.complete;

                    if (gameState == GameState.complete) {

                        if (solve && !isAnimating) {
                            // does the autoplayyyy work
                            foreach (var stack in stacks) {
                                if (stack.Count > 0 && stack.type == StackType.stack) {
                                    var newTopCard = stack.topCard();
                                    PlayTopCard(newTopCard);
                                }
                            }
                        }

                        if (drawPile.Count == 0 && discardPile.Count == 0) {

                            int totalCount = 0;

                            foreach (var stack in stacks)
                                if (stack.type == StackType.stack) totalCount += stack.Count;

                            if (totalCount == 0) gameState = GameState.won;

                        }

                    }




                    foreach (var stack in stacks) {

                        if (stack.Count > 0 && stack.type == StackType.stack) {

                            var topCard = stack.topCard();

                            if (!topCard.isFaceUp) {
                                topCard.flipCard();
                                topCard.IsDraggable = true;
                                topCard.ZIndex = stack.Count;
                            }
                        }
                        int i = 1;

                        foreach (var card in stack.cards) {
                            if (!card.IsSelected) card.ZIndex = i;
                            i++;
                        }

                    }


                    var mouseState = Mouse.GetState();
                    var point = dragonDrop.viewport.PointToScreen(mouseState.X, mouseState.Y);

                    if (mouseState.LeftButton == ButtonState.Pressed &&
                        prevMouseState.LeftButton == ButtonState.Released) {

                        if (drawSlot.Border.Contains(point)) {

                            if (drawPile.Count > 0) {

                                if (!isAnimating) {


                                    var card = drawPile.drawCard();

                                    Save(card, drawPile);

                                    card.ZIndex = discardPile.Count;

                                    //card.Position = discardSlot.Position;
                                    animationCount++;
                                    card.ZIndex += ON_TOP;

                                    tween.Tween(card, new { Position = discardSlot.Position }, 1.8f)
                                        .Ease(Ease.BackOut)
                                        .OnComplete(() => afterTween(card));
                                }
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
                                    restackAnimation.Position += new Vector2(stackOffsetHorizontal * 2, 0);
                                    restackAnimation.isSnapAnimating = true;
                                    restackAnimation.snapTime = 4.5f;
                                    if (!muteSound) restackSound.Play();
                                }
                                else if (!muteSound && drawPile.Count > 0) parentSound.Play();
                            }
                        }
                    }

                    clickTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (mouseState.LeftButton == ButtonState.Pressed &&
                        prevMouseState.LeftButton == ButtonState.Released) {

                        if (!isAnimating && clickTimer < DELAY) {

                            // check for double-click event
                            foreach (var stack in stacks) {

                                if (stack.Count > 0) {
                                    if (stack.type == StackType.stack || stack.type == StackType.discard) {

                                        var topCard = stack.topCard();

                                        if (topCard.Border.Contains(point) && topCard.Child == null) {

                                            PlayTopCard(topCard);
                                            Debug.WriteLine("double-click: " + topCard.suit.ToString() + topCard.rank);
                                        }
                                    }
                                }
                            }
                        }
                        clickTimer = 0;
                    }
                    prevMouseState = mouseState;
                }
                tween.Update(float.Parse(gameTime.ElapsedGameTime.Seconds + "." + gameTime.ElapsedGameTime.Milliseconds));
            }
            base.Update(gameTime);
        }

    }
}
