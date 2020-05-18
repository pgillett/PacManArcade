using PacManArcadeGame.Graphics;

namespace PacManArcadeGame.UiStates
{
    public class CoinsInMode : IUiMode
    {
        private readonly Display _display;
        private readonly UiSystem _uiSystem;

        private readonly ScoreBoard _scoreBoard;

        public CoinsInMode(UiSystem uiSystem)
        {
            _uiSystem = uiSystem;
            _display = uiSystem.Display;
            _display.Blank();
            _scoreBoard = uiSystem.ScoreBoard;
        }

        public bool Tick()
        {
            _scoreBoard.HighScoreText();
            _scoreBoard.Player1Text(true);
            _scoreBoard.Player2Text(true);
            _scoreBoard.Credits(_uiSystem.Credits);
            _scoreBoard.Player1Score(0);
            _scoreBoard.HighScore(_uiSystem.GetAndUpdateHighScore(0));

            _display.WriteLine("PUSH START BUTTON", TextColour.Orange, 6, 17);
            _display.WriteLine("1 PLAYER ONLY", TextColour.Cyan, 8, 21);
            //1 OR 2 PLAYER
            _display.WriteLine("BONUS PAC-MAN FOR 10000 pts", TextColour.Peach, 1, 25);
            _display.WriteLine("c 1980 MIDWAY MFG.CO.", TextColour.Pink, 4, 29);

            return true;
        }

     }
}
