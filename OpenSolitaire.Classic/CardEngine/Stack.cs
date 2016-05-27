/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)

 */

using System.Collections.Generic;
using MonoGame.Ruge.DragonDrop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Ruge.CardEngine {
    
    public enum StackMethod {
        normal,
        horizontal,
        vertical
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

        public string name { get; set; } = "Stack";

        public List<Card> cards = new List<Card>();
        
        public int Count => cards.Count;

        public StackType type = StackType.hand;
        public StackMethod method = StackMethod.normal;

        protected int stackOffsetHorizontal, stackOffsetVertical;

        public Vector2 offset {
            get {

                switch (method) {
                    case StackMethod.horizontal: return new Vector2(stackOffsetHorizontal, 0);
                    case StackMethod.vertical: return new Vector2(0, stackOffsetVertical);
                    default: return Vector2.Zero;
                }
            }
        }

        public Slot slot { get; set; }

        public Stack(Texture2D cardBack, SpriteBatch spriteBatch, int stackOffsetH, int stackOffsetV) {
            this.cardBack = cardBack;
            this.spriteBatch = spriteBatch;
            stackOffsetHorizontal = stackOffsetH;
            stackOffsetVertical = stackOffsetV;
        }

    }

}
