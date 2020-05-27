using System;
using System.Collections.Generic;
using System.Text;
using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.GameItems
{
    public class ScatterChase
    {
        private int _counter;

        private int _switch;

        private int _level;

        public bool Tick()
        {
            _counter--;
            if (_counter < 0)
            {
                _switch++;
                _counter = NewTime();
            }

            return _counter == 0;
        }

        public void Reset(int level)
        {
            _level = level;
            _counter = NewTime();
        }

        public bool InChaseMode => _switch % 2 == 1;

        private int NewTime()
        {
            switch (_switch)
            {
                case 0:
                case 2:
                    return (_level < 4 ? 7 : 5) * 60;
                case 1:
                case 3:
                    return 20 * 60;
                case 4:
                    return 5 * 60;
                case 5:
                    return _level == 0 ? 20 * 60 : 1033;
                case 6:
                    return _level == 0 ? 5 * 60 : 1;
                default:
                    return int.MaxValue;
            }
        }
    }
}
