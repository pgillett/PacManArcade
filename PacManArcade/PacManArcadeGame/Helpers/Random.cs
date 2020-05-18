using System;
using System.Collections.Generic;
using System.Text;

namespace PacManArcadeGame.Helpers
{
    public class Random
    {
        private int _randomSeed = 0;

        public int Get(int range)
        {
            _randomSeed = (_randomSeed * 13) % 123;
            return _randomSeed % range;
        }
    }
}
