using System;
using System.Collections.Generic;
using System.Text;

namespace PacManArcadeGame
{
    public class ScoreBoard
    {
        private readonly Display _display;

        public ScoreBoard(Display display)
        {
            _display = display;
        }

        public void Credits(int credits)
        {
            _display.WriteLine($"CREDIT {credits.ToString().PadLeft(2)}", TextColour.White, 2, _display.Height - 1);
        }

        public void Player1Score(int score)
        {
            _display.WriteLine(FormatScore(score), TextColour.White, 1, 1);
        }

        public void Player2Score(int score)
        {
            _display.WriteLine(FormatScore(score), TextColour.White, 20, 1);
        }

        public void HighScore(int score)
        {
            _display.WriteLine(FormatScore(score), TextColour.White, 11, 1);
        }

        public void Player1Text()
        {
            _display.WriteLine("1UP", TextColour.White, 3, 0);
        }

        public void Player2Text()
        {
            _display.WriteLine("2UP", TextColour.White, 22, 0);
        }

        public void HighScoreText()
        {
            _display.WriteLine("HIGH SCORE", TextColour.White, 9, 0);
        }

        private string FormatScore(int score) => score == 0 ? "    00" : score.ToString().PadLeft(6);

       
    }
}
