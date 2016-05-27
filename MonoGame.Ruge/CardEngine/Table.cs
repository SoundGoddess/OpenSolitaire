/* 
© 2016 The Ruge Project (http://ruge.metasmug.com/) 

Licensed under MIT (see License.txt)

 */

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Ruge.DragonDrop;

namespace MonoGame.Ruge.CardEngine {

    public class Table {

        // Z-Index constants
        protected const int ON_TOP = 1000;
        
        protected int stackOffsetHorizontal, stackOffsetVertical;
        protected Texture2D cardBack, slotTex;
        protected SpriteBatch spriteBatch;
        protected DragonDrop<IDragonDropItem> dragonDrop;
        
        public List<Stack> stacks = new List<Stack>();

        public Table(DragonDrop<IDragonDropItem> dragonDrop, Texture2D cardBack, Texture2D slotTex, int stackOffsetH, int stackOffsetV) {
            spriteBatch = dragonDrop.spriteBatch;
            this.dragonDrop = dragonDrop;
            stackOffsetHorizontal = stackOffsetH;
            stackOffsetVertical = stackOffsetV;
            this.cardBack = cardBack;
            this.slotTex = slotTex;
        }


        public Stack AddStack(Slot slot, StackType type = StackType.undefined, StackMethod stackMethod = StackMethod.normal) {

            var stack = new Stack(cardBack, slotTex, spriteBatch, stackOffsetHorizontal, stackOffsetVertical) {
                slot = slot,
                method = stackMethod,
                type = type
            };

            stacks.Add(stack);

            return stack;

        }

        public void AddStack(Stack stack) {
            
            stack.UpdatePositions();
            stacks.Add(stack);
            
        }

        /// <summary>
        /// I recommend only using this for debugging purposes, this is not an ideal way to do things
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Slot GetSlotByName(string name) {

            Slot slot = null;

            foreach (var stack in stacks) {

                if (stack.name == name) slot = stack.slot;

            }

            return slot;

        }


        /// <summary>
        /// I recommend only using this for debugging purposes, this is not an ideal way to do things
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Stack GetStackByName(string name) {

            Stack foundStack = null;

            foreach (var stack in stacks) {

                if (stack.name == name) foundStack = stack;

            }

            return foundStack;

        }


        /// <summary>
        /// override this to set up your table
        /// </summary>
        public void SetTable() { }

        /// <summary>
        /// override this to clear the table
        /// </summary>
        public void Clear() { }


        public void Update(GameTime gameTime) {
            foreach (var stack in stacks) stack.Update(gameTime);
        }

        public void Draw(GameTime gameTime) {

            foreach (var stack in stacks) stack.Draw(gameTime);

            // fixes the z-ordering stuff
            foreach (var item in dragonDrop.Items) {
                var type = item.GetType();
                if (type == typeof(Card)) item.Draw(gameTime);
            }
        }


    }
    

}
