// Attribution (a) Jon Wilson https://github.com/jonashw

using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Ruge.SillyFun.Confetti {
    public class EasyTimer {
        private readonly TimeSpan _duration;
        private TimeSpan _startTime = new TimeSpan(0);

        public EasyTimer(TimeSpan duration) {
            _duration = duration;
        }

        public void Reset(GameTime startTime) {
            _startTime = startTime.TotalGameTime;
        }

        public bool IsFinished(GameTime currentTime) {
            return currentTime.TotalGameTime - _startTime >= _duration;
        }
    }
}