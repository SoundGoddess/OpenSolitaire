// Attribution (a) Jon Wilson https://github.com/jonashw

using System;

namespace MonoGame.Ruge.SillyFun.Confetti {
    public class ResetableArray<T> {
        private readonly T[] _items;
        private int _index;

        public ResetableArray(T[] items) {
            _items = items;
        }

        public void TryDoWithCurrent(Action<T> action) {
            if (_index >= _items.Length) {
                return;
            }
            action(_items[_index]);
            _index++;
        }

        public void Reset() {
            _index = 0;
        }
    }
}