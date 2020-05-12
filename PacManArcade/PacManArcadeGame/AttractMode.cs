using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PacManArcadeGame
{
    public class AttractMode : IUiMode
    {
        private Display _display;
        private readonly Sprites _sprites;
        private UiSystem _uiSystem;
        private ScoreBoard _scoreBoard;

        public AttractMode(UiSystem uiSystem)
        {
            _uiSystem = uiSystem;
            _display = uiSystem.Display;
            _sprites = uiSystem.Sprites;
            _scoreBoard=new ScoreBoard(_display);
            _display.Blank();
            AttractTick = 0;
        }

        public bool Tick()
        {
            ShowText(1, _scoreBoard.HighScoreText);
            ShowText(1, () => _scoreBoard.Player1Score(0));
//            ShowText(1, "HIGH SCORE", TextColour.White, 9, 0);
            //ShowText(1, "00", TextColour.White, 5, 1);

            ShowText(2, _scoreBoard.Player1Text);
            ShowText(2, _scoreBoard.Player2Text);
            ShowText(2, ()=>_scoreBoard.Credits(0));
//            ShowText(2, "1UP", TextColour.White, 3, 0);
//            ShowText(2, "2UP", TextColour.White, 22, 0);
 //           ShowText(2, "CREDIT 0", TextColour.White, 2, _display.Height - 1);

            ShowText(3, "CHARACTER / NICKNAME", TextColour.White, 7, 5);

            DrawGhostAfter(63,GhostColour.Red, new Location(4.5m, 7));

            ShowText(123, "-SHADOW", TextColour.Red, 7, 7);
            ShowText(153, "\"BLINKY\"", TextColour.Red, 18, 7);

            DrawGhostAfter(183,GhostColour.Pink, new Location(4.5m, 10));

            ShowText(243, "-SPEEDY", TextColour.Pink, 7, 10);
            ShowText(273, "\"PINKY\"", TextColour.Pink, 18, 10);

            DrawGhostAfter(303, GhostColour.Cyan, new Location(4.5m, 13));

            ShowText(363, "-BASHFUL", TextColour.Cyan, 7, 13);
            ShowText(393, "\"INKY\"", TextColour.Cyan, 18, 13);

            DrawGhostAfter(423, GhostColour.Orange, new Location(4.5m, 16));

            ShowText(483, "-POKEY", TextColour.Orange, 7, 16);
            ShowText(513, "\"CLYDE\"", TextColour.Orange, 18, 16);

            ShowText(573, "10 pt",TextColour.White,12,24);
            ShowText(573, "50 pt", TextColour.White, 12, 26);
            ShowIcon(573, _sprites.Pill, 10, 24);
            ShowIcon(573, _sprites.PowerPill(false), 10, 26);

            ShowText(633, "c 1980 MIDWAY MFG.CO.", TextColour.Pink, 4, 31);
            ShowIcon(633, _sprites.PowerPill(false), 4, 20);

            if (AttractTick > 690)
            {
                if (AttractTick < 694)
                {
                    PacMan= new PacMan(new Location(_display.Width + 1, 20), Direction.Left);
                    Ghosts = new[]
                    {
                        new Ghost(GhostColour.Red, new Location(_display.Width + 5, 20), Direction.Left,
                            new Location(0, 0)),
                        new Ghost(GhostColour.Pink, new Location(_display.Width + 7, 20), Direction.Left,
                            new Location(0, 0)),
                        new Ghost(GhostColour.Cyan, new Location(_display.Width + 9, 20), Direction.Left,
                            new Location(0, 0)),
                        new Ghost(GhostColour.Orange, new Location(_display.Width + 11, 20), Direction.Left,
                            new Location(0, 0))
                    };
                    ShowPowerPill = true;
                    pointsCounter = 0;
                    Points = 0;
                }
                else
                {
                    if ((AttractTick / 8) % 2 == 0)
                    {
                        _display.Update(_sprites.PowerPill(false), 10, 26);
                        if (ShowPowerPill)
                            _display.Update(_sprites.PowerPill(false), 4, 20);
                    }
                    else
                    {
                        _display.Update(_sprites.Blank, 10, 26);
                        _display.Update(_sprites.Blank, 4, 20);
                    }

                    if (pointsCounter == 0)
                    {
                        if (PacMan.Location.X < 4)
                        {
                            PacMan.Direction = Direction.Right;
                        }

                        if (PacMan.Location.X < 5)
                        {
                            ShowPowerPill = false;
                            foreach (var ghost in Ghosts)
                            {
                                ghost.State = GhostState.Frightened;
                                ghost.Direction = Direction.Right;
                            }
                        }

                        PacMan.Location = PacMan.Location.Add(PacMan.Direction == Direction.Left ? -0.125m : 0.125m, 0);
                        PacMan.Animate();
                       
                        foreach (var ghost in Ghosts)
                        {
                            if (ghost.Direction == Direction.Left)
                            {
                                ghost.Location = ghost.Location.Add(-0.125m, 0);
                                if (AttractTick % 12 == 0)
                                    ghost.Location = ghost.Location.Add(-0.125m, 0);
                            }
                            else
                            {
                                if (AttractTick % 2 == 0)
                                    ghost.Location = ghost.Location.Add(0.125m, 0);
                            }

                            if (ghost.State == GhostState.Frightened && PacMan.Location.CellX == ghost.Location.CellX)
                            {
                                pointsCounter = 50;
                                ghost.State = GhostState.Dead;
                            }

                            ghost.Animate();
                        }
                    }
                    else
                    {
                        pointsCounter--;
                        if (pointsCounter == 0)
                            Points++;
                    }

                    if (pointsCounter > 0)
                    {
                        DrawSprite(_sprites.GhostPoints(Points), PacMan.Location);
                    }
                    else
                    {
                        DrawSprite(_sprites.PacMan(PacMan), PacMan.Location);
                    }


                    foreach (var ghost in Ghosts)
                    {
                        if(ghost.State != GhostState.Dead)
                            DrawSprite(_sprites.Ghost(ghost), ghost.Location);
                    }

                    //pts 50
                }
            }

            AttractTick++;

            return AttractTick < 750 || PacMan.Location.X < _display.Width+2;
        }

        private PacMan PacMan;
        private Animation PacManAnimation = new Animation(4,2);
        private Ghost[] Ghosts;
        private bool ShowPowerPill;
        private int pointsCounter;
        private int Points;


        private int AttractTick;

        private void ShowText(int fromTick, string text, TextColour colour, int x, int y)
        {
            if (AttractTick >= fromTick && AttractTick < fromTick + 10)
            {
                WriteLine(text, colour, x, y);
            }
        }

        private void ShowText(int fromTick, Action action)
        {
            if (AttractTick >= fromTick && AttractTick < fromTick + 10)
            {
                action();
            }
        }

        private void ShowIcon(int fromTick, SpriteSource sprite, int x, int y)
        {
            if (AttractTick >= fromTick && AttractTick < fromTick + 10)
            {
                _display.Update(sprite, x, y);
            }
        }

        private void WriteLine(string text, TextColour colour, int x, int y)
        {
            foreach (var c in text)
            {
                _display.Update(_sprites.Character(colour, c), x, y);
                x++;
            }
        }

        private void DrawGhostAfter(int fromTick, GhostColour colour, Location location)
        {
            if(AttractTick>=fromTick)   
                _display.AddSprite(_sprites.Ghost(colour, Direction.Right, false), location);
        }

        private void DrawSprite(SpriteSource sprite, Location location)
        {
            _display.AddSprite(sprite, location);
        }
    }
}
