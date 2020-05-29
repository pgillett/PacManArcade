using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacManArcadeGame.GameItems
{
    public class GhostHouse
    {
        private Dictionary<GhostColour, int> _counters;
        private int _globalCounter;
        private bool _onGlobal;

        private Ghost _activeGhost;

        private int _level;

        private int _tick;

        public void SetLevel(int level, IEnumerable<Ghost> ghosts)
        {
            _level = level;
            _counters = new[] {GhostColour.Red, GhostColour.Pink, GhostColour.Cyan, GhostColour.Orange}
                    .ToDictionary(c => c, CounterStart);
            _globalCounter = 0;
            _onGlobal = false;
        }

        public void LifeLost()
        {
            _globalCounter = 0;
            _tick = 0;
            _onGlobal = true;
        }

        private int CounterStart(GhostColour colour)
        {
            if (colour == GhostColour.Red) return 0;
            if (colour == GhostColour.Pink) return 0;
            if (colour == GhostColour.Cyan) return _level == 0 ? 30 : 0;
            return _level switch
            {
                0 => 60,
                1 => 50,
                _ => 0
            };
        }

        public void Tick()
        {
            _tick++;
        }

        public void SetActiveGhost(IEnumerable<Ghost> ghosts)
        {
            var inHouse = ghosts
                .Where(g => g.State == GhostState.InHouse)
                .OrderBy(g => (int) (g.Colour))
                .ToList();
            _activeGhost = inHouse.Count == 0 ? null : inHouse[0];
        }

        public void PillEaten()
        {
            _tick = 0;
            if (_activeGhost != null)
                _counters[_activeGhost.Colour]--;
        }

        public bool ShouldLeave(Ghost ghost)
        {
            if (ghost.Colour == GhostColour.Red) return true;
            if (_onGlobal)
            {
                if (_tick > (_level < 4 ? 60 * 4 : 60 * 3))
                {
                    _tick = 0;
                    return true;
                }
                switch (ghost.Colour)
                {
                    case GhostColour.Pink:
                        return _globalCounter == 7;
                    case GhostColour.Cyan:
                        return _globalCounter == 17;
                    case GhostColour.Orange:
                        _onGlobal = false;
                        return _globalCounter == 32;
                    default:
                        return true;
                }
            }
            return _counters[ghost.Colour] <= 0;
        }

    }
}
