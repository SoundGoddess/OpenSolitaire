/* Attribution (a) 2016 The Ruge Project (http://ruge.metasmug.com/) 
 * Unlicensed under NWO-CS (see UNLICENSE)
 */

namespace MonoGame.Ruge.DragonDrop {

    public class CollusionItem {

        public IDragonDropItem item;
        public bool UnderMouse { get; set; } = false;


        public CollusionItem(IDragonDropItem item) {

            this.item = item;

        }

    }

}
