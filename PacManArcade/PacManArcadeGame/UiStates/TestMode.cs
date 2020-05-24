using System.Runtime.CompilerServices;
using PacManArcadeGame.Graphics;

namespace PacManArcadeGame.UiStates
{
    public class TestMode : IUiMode
    {
        private readonly Display _display;
        private readonly SpriteSet _spriteSet;

        private readonly SpriteSource[] _allSpites;

        public TestMode(UiSystem uiSystem)
        {
            _display = uiSystem.Display;
            _spriteSet = uiSystem.SpriteSet;
            _tick = 0;

            _allSpites = new SpriteSource[10 * 32];
            for(int y=0;y<10;y++)
            for (int x = 0; x < 32; x++)
                _allSpites[y * 32 + x] = new SpriteSource(x, y, 1);
        }

        public bool Tick()
        {
            _tick++;

            if (_tick < 30)
            {
                for (int y = 0; y < _display.Height; y++)
                {
                    for (int x = 0; x < _display.Width; x++)
                    {
                        var c = (y * _display.Width + x)*7 + _tick * 61;
                        var s = c % (10 * 32);
                        _display.Update(_allSpites[s], x, y);
                    }
                }
            }
            else
            {
                for (int y = 0; y < _display.Height; y++)
                {
                    for (int x = 0; x < _display.Width; x++)
                    {
                        _display.Update(_spriteSet.TestBox[x%2+2*(y%2)],x,y);
                    }
                }
            }

            return _tick < 120;
        }

        private int _tick;
    }
}
