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

namespace OpenSolitaire {

    public class CardTable : Table {

        public Deck deck;

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


                for (int i = 0; i < 7; i++) {

                    Slot newSlot = new Slot(_slot, _spriteBatch, new Vector2(x + x * i + _slot.Width * i, y));
                    AddSlot(newSlot);

                }

                y = 20;

                for (int i = 6; i >= 3; i--) {

                    Slot newSlot = new Slot(_slot, _spriteBatch, new Vector2(x + x * i + _slot.Width * i, y));
                    AddSlot(newSlot);

                }


            }

            // set up the board
            foreach (Card card in deck.cards) {
                card.Position = slots[0].Position;
                card.Selected += OnCardSelected;
                card.Deselected += OnCardDeselected;
                card.flipCard();
            }

            AddStack(deck);

        }

        private void OnCardSelected(object sender, EventArgs eventArgs) {

            Card card = (Card)sender;

            card.ZIndex += ON_TOP;
                        
        }
        private void OnCardDeselected(object sender, EventArgs eventArgs) {

            Card card = (Card)sender;

            card.ZIndex -= ON_TOP;
        }

    }
}
