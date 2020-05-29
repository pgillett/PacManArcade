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
        private int FrightenedFlashes => FrightenedTime < 2 ? 3 : 5;

        public int FrightenedTime => _level < _frightenedTimes.Length ? _frightenedTimes[_level] * 60 : 1;

        public int FrightenedFlashTime => FrightenedTime - FrightenedFlashes * 28;

        public int PacManSpeed
        {
            get
            {
                if (_level == 0) return 80;
                if (_level >= 4 && _level <= 19) return 90;
                return 100;
            }
        }

        public int PacManFrightenedSpeed
        {
            get
            {
                if (_level == 0) return 90;
                if (_level >= 4 && _level <= 19) return 100;
                return 95;
            }
        }

        public int GhostSpeed
        {
            get
            {
                if (_level == 0) return 75;
                if (_level < 4 ) return 85;
                return 95;
            }
        }

        public int ElroySpeed(int pillsLeft)
        {
            var elroy = ElroyPoint;
            if (pillsLeft <= elroy / 2)
            {
                if (_level == 0) return 85;
                if (_level < 4) return 95;
                return 105;
            }

            if (pillsLeft < elroy)
            {
                if (_level == 0) return 80;
                if (_level < 4) return 90;
                return 100;
            }

            return GhostSpeed;
        }

        public bool InElroy(int pillsLeft) => pillsLeft <= ElroyPoint;

        private int ElroyPoint
        {
            get
            {
                switch (_level)
                {
                    case 0:
                        return 20;
                    case 1:
                        return 30;
                    case 2:
                    case 3:
                    case 4:
                        return 40;
                    case 5:
                    case 6:
                    case 7:
                        return 50;
                    case 8:
                    case 9:
                    case 10:
                        return 60;
                    case 11:
                    case 12:
                    case 13:
                        return 80;
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                        return 100;
                    default:
                        return 120;
                }
            }
        }


        public int GhostFrightenedSpeed
        {
            get
            {
                if (_level == 0) return 50;
                if (_level < 4) return 55;
                return 60;
            }
        }

        public int GhostTunnelSpeed
        {
            get
            {
                if (_level == 0) return 40;
                if (_level < 4) return 45;
                return 50;
            }
        }
    }
}
