using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PacManArcadeGame
{
    public class TestMode : IUiMode
    {
        private Display _display;
        private readonly Sprites _sprites;

        public TestMode(UiSystem uiSystem)
        {
            _display = uiSystem.Display;
            _sprites = uiSystem.Sprites;
            _tick = 0;
        }

        public bool Tick()
        {
            _tick++;

            if (_tick < 60)
            {
                for (int y = 0; y < _display.Height; y++)
                {
                    for (int x = 0; x < _display.Width; x++)
                    {
                        var c = y * _display.Height + x + _tick * 10;
                        var chr = (char) ('A' + (c % 26));
                        c = c / 26;
                        var col = (TextColour) (c % 5);
                        _display.Update(_sprites.Character(col, chr), x, y);
                    }
                }
            }
            else
            {
                for (int y = 0; y < _display.Height; y++)
                {
                    for (int x = 0; x < _display.Width; x++)
                    {
                        _display.Update(_sprites.TestBox[x%2+2*(y%2)],x,y);
                    }
                }
            }

            return _tick < 120;
        }

        private int _tick;
    }
}
