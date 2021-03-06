﻿using PacManArcadeGame.Graphics;
using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.UiStates
{
    public class UiSystem
    {
        public readonly Inputs Inputs;
        public readonly Display Display;
        public readonly SpriteSet SpriteSet;
        public readonly ScoreBoard ScoreBoard;

        private readonly IRenderer _renderer;

        public int Credits;
        private int _highScore;

        private bool _paused;

        private readonly GameSetup.LevelSetup _levelSetup;

        private IUiMode _uiMode;

        public int GetAndUpdateHighScore(int score)
        {
            if (score > _highScore)
            {
                _highScore = score;
            }

            return _highScore;
        }

        public UiSystem(IRenderer renderer)
        {
            _levelSetup = new GameSetup.LevelSetup();
            SpriteSet = new SpriteSet();
            Display = new Display(_levelSetup.MapHeight + 5, _levelSetup.MapWidth, SpriteSet);
            Inputs=new Inputs();
            ScoreBoard = new ScoreBoard(Display);

            _renderer = renderer;

            _uiMode = new TestMode(this);
        }

        public void Tick()
        {
            Inputs.Coin.On(() => Credits++);

            Inputs.Reset.On(() =>
            {
                _uiMode = new TestMode(this);
                Credits = 0;
            });

            Inputs.Pause.On(() => _paused = !_paused);

            if (!_paused || Inputs.Tick.WasPressed)
            {
                Display.ClearSprites();

                Inputs.FastForward.On(() =>
                {
                    for (int i = 0; i < 7; i++)
                        _uiMode.Tick();
                });

                var alive = _uiMode.Tick();

                switch (_uiMode)
                {
                    case TestMode _:
                    {
                        if (!alive)
                        {
                            _uiMode = new AttractMode(this);
                        }

                        break;
                    }
                    case AttractMode _:
                    {
                        if (!alive)
                        {
                            _uiMode = new DemoMode(this, _levelSetup);
                        }

                        if (Credits > 0)
                        {
                            _uiMode = new CoinsInMode(this);
                        }

                        break;
                    }
                    case CoinsInMode _:
                    {
                        Inputs.Player1Start.On(() =>
                        {
                            Credits--;
                            _uiMode = new PlayMode(this, _levelSetup);
                        });

                        break;
                    }
                    case DemoMode _:
                    {
                        if (!alive)
                        {
                            _uiMode = new AttractMode(this);
                        }

                        if (Credits > 0)
                        {
                            _uiMode = new CoinsInMode(this);
                        }

                        break;
                    }
                    case PlayMode _:
                    {
                        if (!alive)
                        {
                            _uiMode=new AttractMode(this);
                        }

                        break;
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