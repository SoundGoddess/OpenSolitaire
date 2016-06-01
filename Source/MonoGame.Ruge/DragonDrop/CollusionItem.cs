namespace MonoGame.Ruge.DragonDrop {

    public class CollusionItem {

        public IDragonDropItem item;
        public bool UnderMouse { get; set; } = false;


        public CollusionItem(IDragonDropItem item) {

            this.item = item;

        }

    }

}
