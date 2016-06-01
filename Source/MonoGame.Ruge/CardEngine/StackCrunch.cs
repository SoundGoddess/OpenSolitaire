using Microsoft.Xna.Framework;

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
