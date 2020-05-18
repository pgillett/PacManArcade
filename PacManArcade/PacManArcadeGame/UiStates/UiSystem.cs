using PacManArcadeGame.Graphics;
using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.UiStates
{
    public class UiSystem
    {
        public Inputs Inputs;
        public Display Display;
        public SpriteSet SpriteSet;
        public ScoreBoard ScoreBoard;

        private IRenderer _renderer;

        public int Credits;
        private int _highScore;

        private bool _paused;

        public int GetAndUpdateHighScore(int score)
        {
            if (score > _highScore)
            {
                _highScore = score;
            }

            return _highScore;
        }

        public int Height => _gameSetup.MapHeight;
        public int Width => _gameSetup.MapWidth;

        private int Ticks = 0;

        private GameSetup.GameSetup _gameSetup;

        private IUiMode UiMode;

        public UiSystem(IRenderer renderer)
        {
            _gameSetup = new GameSetup.GameSetup();
            SpriteSet = new SpriteSet();
            Display = new Display(Height + 5, Width, SpriteSet);
            Inputs=new Inputs();
            ScoreBoard = new ScoreBoard(Display);

            _renderer = renderer;

            UiMode = new TestMode(this);
        }

        public void Tick()
        {
            if (Inputs.Coin)
            {
                Credits++;
                Inputs.Coin = false;
            }

            if (Inputs.Reset)
            {
                UiMode = new TestMode(this);
                Credits = 0;
                Inputs.Reset = false;
            }

            if (Inputs.Pause)
            {
                Inputs.Pause = false;
                _paused = !_paused;
            }

            if (!_paused || Inputs.Tick == true)
            {
                Ticks++;
                Display.ClearSprites();

                Inputs.Tick = false;

                var alive = UiMode.Tick();

                if (UiMode is TestMode)
                {
                    if (!alive)
                    {
                        UiMode = new AttractMode(this);
                    }
                }
                else if (UiMode is AttractMode)
                {
                    if (!alive)
                    {
                        UiMode = new GameMode(this, _gameSetup, true);
                    }

                    if (Credits > 0)
                    {
                        UiMode = new CoinsInMode(this);
                    }
                }
                else if (UiMode is CoinsInMode)
                {
                    if (Inputs.Player1Start)
                    {
                        Credits--;
                        Inputs.Player1Start = false;
                        UiMode = new GameMode(this, _gameSetup, false);
                    }
                }
                else if (UiMode is GameMode)
                {
                    if (!alive)
                    {
                        UiMode=new AttractMode(this);
                    }
                }
            }

            if (_inRender) return;
            _inRender = true;
            _renderer.Render(Display);
            _inRender = false;
        }

        private bool _inRender;
    }
}