using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PacManArcadeGame
{
    public class CoinsInMode : IUiMode
    {
        private Display _display;
        private UiSystem _uiSystem;

        private ScoreBoard _scoreBoard;

        public CoinsInMode(UiSystem uiSystem)
        {
            _uiSystem = uiSystem;
            _display = uiSystem.Display;
            _display.Blank();
            _scoreBoard = new ScoreBoard(_display);
        }

        public bool Tick()
        {
            _display.WriteLine("HIGH SCORE", TextColour.White, 9, 0);
            _display.WriteLine( "00", TextColour.White, 5, 1);
            _display.WriteLine( "1UP", TextColour.White, 3, 0);
            _display.WriteLine( "2UP", TextColour.White, 22, 0);
            _display.WriteLine( $"CREDIT {_uiSystem.Credits.ToString().PadLeft(2)}", TextColour.White, 2, _display.Height - 1);

            _display.WriteLine("PUSH START BUTTON", TextColour.Orange, 6, 17);
            _display.WriteLine("1 PLAYER ONLY", TextColour.Cyan, 8, 21);
            //1 OR 2 PLAYER
            _display.WriteLine("BONUS PAC-MAN FOR 10000 pt", TextColour.Pink, 1, 25);
            _display.WriteLine("c 1980 MIDWAY MFG.CO.", TextColour.Pink, 4, 29);

            return true;
        }

     }
}
