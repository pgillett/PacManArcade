using System;
using System.Collections.Generic;
using System.Text;

namespace PacManArcadeGame.GameItems
{
    public class Speeds
    {
        private int _level;

        public Speeds()
        {

        }

        public void SetLevel(int level)
        {
            _level = level;
        }

        private int[] _frightenedTimes = new int[] {6, 5, 4, 3, 2, 5, 2, 2, 1, 5, 2, 1, 1, 3, 1, 1, 0, 1};
        private int[] _frightenedFlashes = new[] {5, 5, 5, 5, 5, 5, 5, 5, 3, 5, 5, 3, 3, 5, 3, 3};
        public int FrightenedTime => _level < _frightenedTimes.Length ? _frightenedTimes[_level] * 60 : 1;

        private int FrightenedFlashes => FrightenedTime < 2 ? 3 : 5;

        public int FrightenedFlashTime => FrightenedTime - FrightenedFlashes * 28;
    }
}
