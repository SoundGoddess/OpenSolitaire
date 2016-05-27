/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)

 */

using System.Collections.Generic;
using System.Linq;
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


        public Stack(Texture2D cardBack, Texture2D slotTex, SpriteBatch spriteBatch, int stackOffsetH, int stackOffsetV) {
            slot = new Slot(slotTex,spriteBatch);
            this.cardBack = cardBack;
            this.spriteBatch = spriteBatch;
            stackOffsetHorizontal = stackOffsetH;
            stackOffsetVertical = stackOffsetV;
        }


        public void shuffle() { }
        public Card drawCard() {
            return null;
        }
        
        public void Update(GameTime gameTime) {
            
            slot.Update(gameTime);

            foreach (var card in cards) card.Update(gameTime);

        }
        
        public void Draw(GameTime gameTime) {
            
            slot.Draw(gameTime);

            cards = cards.OrderBy(z => z.ZIndex).ToList();

            foreach (var card in cards) card.Draw(gameTime);
            
        }

        public void UpdatePositions(StackMethod stackMethod = StackMethod.undefined) {

            if (stackMethod != StackMethod.undefined) method = stackMethod;
            
            cards = cards.OrderBy(z => z.ZIndex).ToList();

            int i = 0;

            foreach (var card in cards) {
                
                card.Position = new Vector2(slot.Position.X + offset.X * i, slot.Position.Y + offset.Y * i);
                
                i++;

            }

        }


    }

}
