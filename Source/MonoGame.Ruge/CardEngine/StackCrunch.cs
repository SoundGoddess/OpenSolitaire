// Attribution (a) 2016 The Ruge Project (http://ruge.metasmug.com/) 
// Licensed under NWO-CS (see License.txt)

namespace MonoGame.Ruge.CardEngine {
    public class StackCrunch {

        // define the number of items in a stack before attempting to crunch
        public int crunchItemMin { get; set; } = 10;
        public int faceDownCrunch { get; set; }
        public int faceUpCrunch { get; set; }

        public StackCrunch(int stackOffset) {

            faceDownCrunch = stackOffset/3;
            faceUpCrunch = stackOffset/2;

        }

    }
}
