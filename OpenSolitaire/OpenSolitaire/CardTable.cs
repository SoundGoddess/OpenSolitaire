/* ©2016 Hathor Gaia 
 * http://HathorsLove.com
 * 
 * Licensed Under GNU GPL 3:
 * http://www.gnu.org/licenses/gpl-3.0.html
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Ruge.CardEngine;
using MonoGame.Ruge.DragonDrop;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OpenSolitaire {

    public class CardTable : Table {

        public Deck deck;

        private Stack animateMe;

        private bool isSetup = false;

        public CardTable(DragonDrop<IDragonDropItem> dd, Texture2D cardback, Texture2D slot, int stackOffsetH, int stackOffsetV)
            : base(dd, cardback, slot, stackOffsetH, stackOffsetV) {

            // create a fresh card deck
            deck = new Deck(cardback, dd.spriteBatch);
            deck.freshDeck();
            deck.shuffle();

        }

        public new void SetTable(bool resetTable = false) {

            if (resetTable) {
                Clear();
                deck.freshDeck();
                deck.shuffle();
            }

            else {

                int x = 20;
                int y = 20;

                Slot drawSlot = new Slot(_slot, _spriteBatch, new Vector2(x, y));
                Slot discardSlot = new Slot(_slot, _spriteBatch, new Vector2(x * 2 + _slot.Width, y));

                AddSlot(drawSlot);
                AddSlot(discardSlot);

                y += _slot.Height + y;


                animateMe = new Stack(_cardBack, _spriteBatch);

                for (int i = 0; i < 7; i++) {

                    Slot newSlot = new Slot(_slot, _spriteBatch, new Vector2(x + x * i + _slot.Width * i, y));
                    AddSlot(newSlot);

                    Card moveCard = deck.drawCard();
                    moveCard.origin = newSlot.Position;
                    moveCard.ZIndex += ON_TOP + i;
                    moveCard.returnToOrigin = true;
                    moveCard.snapSpeed = 4.0f;
                    moveCard.IsDraggable = false;
                    animateMe.addCard(moveCard);

                    for (int j = 1; j < i + 1; j++) {

                        moveCard = deck.drawCard();
                        moveCard.origin = new Vector2(newSlot.Position.X, newSlot.Position.Y + (_stackOffsetVertical * j));
                        moveCard.ZIndex += ON_TOP + i + j;
                        moveCard.returnToOrigin = true;
                        moveCard.snapSpeed = 4.0f;
                        moveCard.IsDraggable = false;
                        animateMe.addCard(moveCard);

                    }

                }

                AddStack(animateMe);

                y = 20;

                for (int i = 6; i >= 3; i--) {

                    Slot newSlot = new Slot(_slot, _spriteBatch, new Vector2(x + x * i + _slot.Width * i, y));
                    AddSlot(newSlot);

                }

                isSetup = true;

            }

            // set up the board
            foreach (Card card in deck.cards) {

                if (!card.returnToOrigin) { 
                    card.IsDraggable = true;
                    card.Position = slots[0].Position;
                    card.origin = card.Position;
                    card.Selected += OnCardSelected;
                    card.Deselected += OnCardDeselected;
                }
            }

            AddStack(deck);

            /*
            
            for (int i = 0; i < 7; i++) {
                
                Card moveCard = deck.drawCard();
                moveCard.origin = slots[i].Position;
                moveCard.ZIndex += ON_TOP;
                moveCard.returnToOrigin = true;
                
                for (int j = 1; j < i + 1; j++) {

                    moveCard = deck.drawCard();
                    moveCard.origin = new Vector2(slots[i].Position.X, slots[i].Position.Y + (_stackOffsetVertical * j));
                    moveCard.returnToOrigin = true;

                }

            }

            Card moveCard = deck.drawCard();
            moveCard.origin = slots[i].Position;
            moveCard.ZIndex += ON_TOP;
            moveCard.returnToOrigin = true;
            */

        }

        private void OnCardSelected(object sender, EventArgs eventArgs) {

            Card card = (Card)sender;

            if (card.IsDraggable) {
                card.IsSelected = true;
                card.ZIndex += ON_TOP;
            }

        }
        private void OnCardDeselected(object sender, EventArgs eventArgs) {

            Card card = (Card)sender;

            card.IsSelected = false;

            if (card.Position != card.origin) card.returnToOrigin = true;
        }


        bool AnimateSprite(ref Card sprite, Vector2 destination) {

            bool hasArrived = false;

            if (sprite.Position == destination) {
                hasArrived = true;
                sprite.ZIndex = -ON_TOP;
            }
            else {

                var pos = sprite.Position;
                float speed = 5.0f;

                float distance = (float)Math.Sqrt(Math.Pow(destination.X - pos.X, 2) + (float)Math.Pow(destination.Y - pos.Y, 2));
                float directionX = (destination.X - pos.X) / distance;
                float directionY = (destination.Y - pos.Y) / distance;

                pos.X += directionX * speed;
                pos.Y += directionY * speed;


                if (Math.Sqrt(Math.Pow(pos.X - sprite.Position.X, 2) + Math.Pow(pos.Y - sprite.Position.Y, 2)) >= distance) {

                    sprite.Position = destination;

                    hasArrived = true;

                    sprite.ZIndex = -ON_TOP;

                }
                else sprite.Position = pos;
            }

            return hasArrived;

        }

        public void Update(GameTime gameTime) {

            foreach (Slot slot in this.slots) slot.Update(gameTime);

            foreach (Stack stack in this.stacks) {

                foreach (Card card in stack.cards) card.Update(gameTime);

            }

            /*
            if (isSetup) { 
                int animationCount = 0;

                // animates dealing from the draw pile to set up the board
                if (animationQueue.Count > 0) {
                    for (int i = 1; i < animationQueue.Count + 1; i++) {

                        Card sprite = deck.cards;

                        bool hasArrived = AnimateSprite(ref sprite, animationQueue[i - 1]);

                        if (hasArrived) {
                            animationCount++;
                        }

                    }
                }

                if (animationCount == animationQueue.Count) {
                
                    animationQueue.Clear();

                }
            }

        */


        }

    }
}
