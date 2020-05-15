using System;
using PacManArcadeGame.Graphics;
using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.UiStates
{
    public class AttractMode : IUiMode
    {
        private Display _display;
        private readonly SpriteSet _spriteSet;
        private UiSystem _uiSystem;
        private ScoreBoard _scoreBoard;

        public AttractMode(UiSystem uiSystem)
        {
            _uiSystem = uiSystem;
            _display = uiSystem.Display;
            _spriteSet = uiSystem.SpriteSet;
            _scoreBoard=uiSystem.ScoreBoard;
            _display.Blank();
            _attractTick = 0;
        }

        public bool Tick()
        {
            ShowText(1, _scoreBoard.HighScoreText);
            ShowText(1, () => _scoreBoard.Player1Score(0));

            ShowText(2, () =>_scoreBoard.Player1Text(true));
            ShowText(2, () =>_scoreBoard.Player2Text(true));
            ShowText(2, ()=>_scoreBoard.Credits(0));

            ShowText(3, "CHARACTER / NICKNAME", TextColour.White, 7, 5);

            DrawGhostAfter(63, GhostColour.Red, 7,
                TextColour.Red, "-SHADOW", "\"BLINKY\"");

            DrawGhostAfter(183, GhostColour.Pink, 10,
                TextColour.Pink, "-SPEEDY", "\"PINKY\"");

            DrawGhostAfter(303, GhostColour.Cyan, 13,
                TextColour.Cyan, "-BASHFUL", "\"INKY\"");

            DrawGhostAfter(423, GhostColour.Orange, 16,
                TextColour.Orange, "-POKEY", "\"CLYDE\"");

            ShowText(573, "10 pt",TextColour.White,12,24);
            ShowText(573, "50 pt", TextColour.White, 12, 26);
            ShowIcon(573, _spriteSet.Pill, 10, 24);
            ShowIcon(573, _spriteSet.PowerPill(false), 10, 26);

            ShowText(633, "c 1980 MIDWAY MFG.CO.", TextColour.Pink, 4, 31);
            ShowIcon(633, _spriteSet.PowerPill(false), 4, 20);

            if (_attractTick > 690)
            {
                if (_attractTick < 694)
                {
                    _pacMan= new PacMan(new Location(_display.Width + 1, 20), Direction.Left);
                    _ghosts = new[]
                    {
                        new Ghost(GhostColour.Red, new Location(_display.Width + 5, 20), Direction.Left,
                            new Location(0, 0), new Location(0,0)),
                        new Ghost(GhostColour.Pink, new Location(_display.Width + 7, 20), Direction.Left,
                            new Location(0, 0), new Location(0,0)),
                        new Ghost(GhostColour.Cyan, new Location(_display.Width + 9, 20), Direction.Left,
                            new Location(0, 0), new Location(0,0)),
                        new Ghost(GhostColour.Orange, new Location(_display.Width + 11, 20), Direction.Left,
                            new Location(0, 0), new Location(0,0))
                    };
                    _showPowerPill = true;
                    _pointsCounter = 0;
                    _points = 0;
                }
                else
                {
                    if ((_attractTick / 8) % 2 == 0)
                    {
                        _display.Update(_spriteSet.PowerPill(false), 10, 26);
                        if (_showPowerPill)
                            _display.Update(_spriteSet.PowerPill(false), 4, 20);
                    }
                    else
                    {
                        _display.Update(_spriteSet.Blank, 10, 26);
                        _display.Update(_spriteSet.Blank, 4, 20);
                    }

                    if (_pointsCounter == 0)
                    {
                        if (_pacMan.Location.X < 4)
                        {
                            _pacMan.ChangeDirection(Direction.Right);
                        }

                        if (_pacMan.Location.X < 5)
                        {
                            _showPowerPill = false;
                            foreach (var ghost in _ghosts)
                            {
                                ghost.ChangeState(GhostState.Frightened);
                                ghost.ChangeNextDirection(Direction.Right, new Location(_display.Width+1, 20));
                            }
                        }

                        _pacMan.Move(_pacMan.Direction == Direction.Left ? -0.125m : 0.125m, 0);

                        foreach (var ghost in _ghosts)
                        {
                            if (ghost.Direction == Direction.Left)
                            {
                                ghost.Move(-0.125m, 0);
                                if (_attractTick % 12 == 0)
                                    ghost.Move(-0.125m, 0);
                            }
                            else
                            {
                                if (_attractTick % 2 == 0)
                                    ghost.Move(0.125m, 0);
                            }

                            if (ghost.State == GhostState.Frightened && _pacMan.Location.IsNearTo(ghost.Location))
                            {
                                _pointsCounter = 50;
                                ghost.ChangeState(GhostState.Dead);
                            }
                        }
                    }
                    else
                    {
                        _pointsCounter--;
                        if (_pointsCounter == 0)
                            _points++;
                    }

                    if (_pointsCounter > 0)
                    {
                        DrawSprite(_spriteSet.GhostPoints(_points), _pacMan.Location);
                    }
                    else
                    {
                        DrawSprite(_spriteSet.PacMan(_pacMan), _pacMan.Location);
                    }


                    foreach (var ghost in _ghosts)
                    {
                        if(ghost.State != GhostState.Dead)
                            DrawSprite(_spriteSet.Ghost(ghost), ghost.Location);
                    }

                    //pts 50
                }
            }

            _attractTick++;

            return _attractTick < 750 || _pacMan.Location.X < _display.Width+2;
        }

        private PacMan _pacMan;
        private Ghost[] _ghosts;
        private bool _showPowerPill;
        private int _pointsCounter;
        private int _points;
        
        private int _attractTick;

        private void ShowText(int fromTick, string text, TextColour colour, int x, int y)
        {
            if (_attractTick >= fromTick && _attractTick < fromTick + 10)
            {
                _display.WriteLine(text, colour, x, y);
            }
        }

        private void ShowText(int fromTick, Action action)
        {
            if (_attractTick >= fromTick && _attractTick < fromTick + 10)
            {
                action();
            }
        }

        private void ShowIcon(int fromTick, SpriteSource sprite, int x, int y)
        {
            if (_attractTick >= fromTick && _attractTick < fromTick + 10)
            {
                _display.Update(sprite, x, y);
            }
        }
        
        private void DrawGhostAfter(int fromTick, GhostColour colour, int y, 
            TextColour textColour, string text1, string text2)
        {
            if(_attractTick>=fromTick)   
                _display.AddSprite(_spriteSet.Ghost(colour, Direction.Right, false), new Location(4.5m, y));
            ShowText(fromTick + 60, text1, textColour, 7, y);
            ShowText(fromTick + 90, text2, textColour, 18, y);
        }

        private void DrawSprite(SpriteSource sprite, Location location)
        {
            _display.AddSprite(sprite, location);
        }
    }
}
