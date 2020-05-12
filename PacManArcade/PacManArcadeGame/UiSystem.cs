namespace PacManArcadeGame
{
    public class UiSystem
    {
        public Inputs Inputs;
        public Display Display;
        public Sprites Sprites;

        private IRenderer _renderer;

        private TestMode _testMode;
        private AttractMode _attractMode;
        private CoinsInMode _coinsInMode;
        private Game _game;

        public int Credits;

        public int Height => _gameSetup.MapHeight;
        public int Width => _gameSetup.MapWidth;

        private int Ticks = 0;

        private GameSetup _gameSetup;

        private IUiMode UiMode;

        public UiSystem(IRenderer renderer)
        {
            _gameSetup = new GameSetup();
            Sprites = new Sprites();
            Display = new Display(Height + 5, Width, Sprites);
            Inputs=new Inputs();

            _renderer = renderer;

            _game = new Game(_gameSetup, Display, Sprites);

            UiMode = new TestMode(this);
        }

        public void Tick()
        {
            Ticks++;
            Display.ClearSprites();

            if (Inputs.Coin)
            {
                Credits++;
                Inputs.Coin = false;
            }

            if (Inputs.Reset)
            {
                UiMode = new TestMode(this);
                Credits= 0;
                Inputs.Reset = false;
            }

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
                    UiMode = new AttractMode(this);
                }
                if (Credits > 0)
                {
                    UiMode = new CoinsInMode(this);
                }
            }
           // else if (UiMode is PlayingGame)
            {
               
            }
          //  else // UiState == UiState.HighScore
            {

            }

            if (_inRender) return;
            _inRender = true;
            _renderer.Render(Display);
            _inRender = false;
            RenderTick++;
        }
        private bool _inRender;
        private int RenderTick;
    }
}