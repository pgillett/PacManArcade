using PacManArcadeGame.Graphics;

namespace PacManArcadeGame.UiStates
{
    public class TestMode : IUiMode
    {
        private readonly Display _display;
        private readonly SpriteSet _spriteSet;

        public TestMode(UiSystem uiSystem)
        {
            _display = uiSystem.Display;
            _spriteSet = uiSystem.SpriteSet;
            _tick = 0;
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
                        var c = y * _display.Height + x + _tick * 10;
                        var chr = (char) ('A' + (c % 26));
                        c = c / 26;
                        var col = (TextColour) (c % 7);
                        _display.Update(_spriteSet.Character(col, chr), x, y);
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
