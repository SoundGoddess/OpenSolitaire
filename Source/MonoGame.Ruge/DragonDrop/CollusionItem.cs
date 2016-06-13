// Attribution (a) 2016 The Ruge Project (http://ruge.metasmug.com/) 
// Licensed under NWO-CS (see License.txt)

namespace MonoGame.Ruge.DragonDrop {

    public class CollusionItem {

        public IDragonDropItem item;
        public bool UnderMouse { get; set; } = false;


        public CollusionItem(IDragonDropItem item) {

            this.item = item;

        }

    }

}
