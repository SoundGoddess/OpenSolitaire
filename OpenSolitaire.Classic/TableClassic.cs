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

namespace OpenSolitaire.Classic {
    class TableClassic : Table {

        public bool isSetup = false;
        public bool isAnimating = false;
        
        public Deck drawPile { get; set; }
        public Stack discardPile { get; set; }

        public Slot drawSlot { get; set; }
        public Slot discardSlot { get; set; }

        public TableClassic(DragonDrop<IDragonDropItem> dd, Texture2D cardBack, Texture2D slotTex, int stackOffsetH, int stackOffsetV)
            : base(dd, cardBack, slotTex, stackOffsetH, stackOffsetV) {

            // create a fresh card deck
            drawPile = new Deck(cardBack, slotTex, dd.spriteBatch, stackOffsetH, stackOffsetV) { type = StackType.deck };
            drawPile.freshDeck();
            
        }


        public new void Clear() {

            foreach (var stack in stacks) {
                foreach (var card in stack.cards) dragonDrop.Remove(card);
                stack.Clear();
            }


            drawPile.freshDeck();

        }

        internal void InitializeTable() {
            
            foreach (var card in drawPile.cards) {
                card.IsDraggable = true;
                dragonDrop.Add(card);
            }
        
            int x = 20;
            int y = 20;

            drawSlot = new Slot(slotTex, spriteBatch) {
                name = "Draw",
                Position = new Vector2(x, y)
            };
            discardSlot = new Slot(slotTex, spriteBatch) {
                name = "Discard",
                Position = new Vector2(x * 2 + slotTex.Width, y)
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
                AddStack(newSlot, StackType.stack, StackMethod.vertical);

            }


            y = 20;

            // set up play/score slots
            for (int i = 6; i >= 3; i--) {
                
                var newSlot = new Slot(slotTex, spriteBatch) {
                    Position = new Vector2(x + x * i + slotTex.Width * i, y),
                    name = "Play " + i
                };

                AddStack(newSlot, StackType.play, StackMethod.normal);
                
            }

        }


        public new void SetTable() {





            isSetup = true;
        }
        

        public new void Update(GameTime gameTime) {

            

            base.Update(gameTime);
        }

    }
}
