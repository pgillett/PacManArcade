﻿using System;
using System.Collections.Generic;
using System.Linq;
using PacManArcadeGame.Graphics;
using PacManArcadeGame.Helpers;
using PacManArcadeGame.Map;

namespace PacManArcadeGame.UiStates
{
    public class GameMode : IUiMode
    {
        private readonly SpriteSet _spriteSet;

        public InputDirection _inputDirection => _uiSystem.Inputs.Direction;

        public int Height => Map.Height;
        public int Width => Map.Width;

        public PacMan PacMan;

        public Ghost Blinky;
        public Ghost Pinky;
        public Ghost Inky;
        public Ghost Clyde;

        public IEnumerable<Ghost> Ghosts => new[] {Blinky, Pinky, Inky, Clyde};

        public Location ExitGhostHouse;

        public Map.Map Map;

        public Display Display;

        private GameSetup.GameSetup _gameSetup;

        private int Ticks = 0;

        private ScoreBoard _scoreBoard;

        private GameState _gameState;
        private int _nextTick = 0;

        private int _score = 0;

        private UiSystem _uiSystem;

        public GameMode(UiSystem uiSystem, GameSetup.GameSetup gameSetup)
        {
            _uiSystem = uiSystem;
            Display = uiSystem.Display;
            _spriteSet = uiSystem.SpriteSet;

            _scoreBoard = uiSystem.ScoreBoard;

            _gameSetup = gameSetup;

            _gameState = GameState.Intro;
            _nextTick = 120;

            ResetGame();
        }

        private void ResetGame()
        {
            Map = _gameSetup.Map;
            ResetGhostsAndPacMan();

            DrawMap();
        }

        private void ResetGhostsAndPacMan()
        {
            Blinky = _gameSetup.InitialBlinky.Ghost;
            Pinky = _gameSetup.InitialPinky.Ghost;
            Inky = _gameSetup.InitialInky.Ghost;
            Clyde = _gameSetup.InitialClyde.Ghost;
            PacMan = _gameSetup.InitialPacMan.PacMan;
            ExitGhostHouse = _gameSetup.ExitGhostHouse;
        }

        private const int BoardYOffset = 3;
        public Action UiChangeState;

        private Animation _powerPillAnimation = new Animation(2,10);
        private Animation _scoreAnimation = new Animation(2, 16);

        public bool Tick()
        {
            Ticks++;
            Display.ClearSprites();
            _scoreAnimation.Tick();

            if (_gameState == GameState.Intro)
            {
                Display.WriteLine("PLAYER ONE", TextColour.Cyan, 9, 14);
                Display.WriteLine("READY!", TextColour.White, 11, 20); // yellow
                if (Ticks > _nextTick)
                {
                    _gameState = GameState.GetReady;
                    _nextTick = Ticks + 120;
                }
            }
            else if (_gameState == GameState.GetReady)
            {
                Display.WriteLine("          ", TextColour.Cyan, 9, 14);
                Display.WriteLine("      ", TextColour.White, 11, 20); // yellow
                if (Ticks > _nextTick)
                {
                    _gameState = GameState.Playing;
                    SwitchChaseMode(true);
                }
            }
            else
            {
                SwitchChaseMode(false);
                
                _powerPillAnimation.Tick();

                if (_ghostEatenPause > 0)
                {
                    _ghostEatenPause--;
                    if (_ghostEatenPause == 0)
                    {
                        foreach (var ghost in Ghosts.Where(g => g.State == GhostState.Eaten))
                            ghost.SetEyes();
                    }
                }
                else
                {
                    if (!CheckPacManPill())
                    {
                        MovePacMan();
                    }

                    MoveGhosts();
                    if (CheckPacManPowerPill())
                    {
                        foreach (var ghost in Ghosts)
                        {
                            ghost.SetFrightened();
                        }

                        _gameState = GameState.Frightened;
                        _nextTick = Ticks + 7 * 60;
                        _ghostEatenPoints = 0;
                    }

                    if (_gameState == GameState.Frightened && Ticks > _nextTick)
                    {
                        _gameState = GameState.Playing;
                        SwitchChaseMode(true);
                        foreach (var ghost in Ghosts.Where(g => g.State == GhostState.Frightened))
                        {
                            ghost.SetAlive();
                        }
                    }

                    foreach (var ghost in Ghosts)
                    {
                        if (PacMan.Location.IsNearTo(ghost.Location))
                        {
                            if (ghost.State == GhostState.Frightened)
                            {
                                ghost.SetEaten(_ghostEatenPoints);
                                _ghostEatenPause = 60;
                                _ghostEatenPoints++;
                                _score += _ghostEatenPoints switch
                                {
                                    0 => 200,
                                    1 => 400,
                                    2 => 800,
                                    3 => 1600,
                                    _ => 0
                                };
                            }
                        }
                    }
                }
            }

            DrawScore();
            DrawSprites();

            return true;
        }

        private int _ghostEatenPause = 0;
        private int _ghostEatenPoints = 0;

        private void SwitchChaseMode(bool force)
        {
            if (force)
            {
                _chaseMode = false;
                _chaseSwitchTick = Ticks + _scatterForTicks;
            }

            if (_gameState == GameState.Playing && Ticks > _chaseSwitchTick)
            {
                _chaseMode = !_chaseMode;
                _chaseSwitchTick = Ticks + (_chaseMode ? _chaseForTicks: _scatterForTicks);
                foreach (var ghost in Ghosts)
                {
                    ghost.FlipDirection();
                }
            }
        }

        private int _chaseForTicks = 20 * 60;
        private int _scatterForTicks = 7 * 60;

        private bool _chaseMode;
        private int _chaseSwitchTick;

        private int _randomSeed = 0;

        private int Random(int range)
        {
            _randomSeed = (_randomSeed * 13) % 123;
            return _randomSeed % range;
        }

        private bool SkipMove(int skipTickEvery) => (Ticks % skipTickEvery) == 0;

        private void MoveGhosts()
        {
            foreach (var ghost in Ghosts)
            {
                if(SkipMove(16)) continue;
                if (ghost.State == GhostState.Frightened && SkipMove(2)) continue;

                if (ghost.IsForcedMovement)
                {
                    ghost.MoveTowardsTarget();
                    if (ghost.IsAtTarget)
                    {
                        if (ghost.State == GhostState.IntoHouse)
                        {
                            ghost.SetInHouse();
                        }
                    }
                }
                else
                {
                    if (ghost.IsAtTarget)
                    {
                        ghost.ChangeDirection();

                        var nextGhostCell = ghost.CurrentTarget.Cell;
                        var cell = Map.Cell(nextGhostCell);

                        var newDirection = ghost.Direction;

                        if (ghost.State == GhostState.Frightened)
                        {
                            var directions = _directions
                                .Where(d => d != newDirection.Opposite())
                                .Where(d => cell.InDirection(d).IsPlayArea)
                                .ToArray();
                            newDirection = directions[Random(directions.Length)];
                        }
                        else if (ghost.State == GhostState.LeaveHouse)
                        {
                            ghost.SetAlive();
                            ghost.ChangeNextDirection(Direction.Left, nextGhostCell.Move(Direction.Left));
                        }
                        else
                        {
                            Location target;
                            switch (ghost.State)
                            {
                                case GhostState.Alive:
                                    target = _chaseMode
                                        ? ghost.GetChaseTarget(PacMan, Blinky.Location)
                                        : ghost.ScatterTarget;
                                    break;
                                case GhostState.Eyes:
                                    target = ExitGhostHouse;
                                    if (ghost.Location.IsSameCell(ExitGhostHouse))
                                    {
                                        ghost.SetToIntoHouse();
                                    }

                                    break;

                                default:
                                    target = ghost.Location;
                                    break;
                            }

                            if (ghost.State != GhostState.Alive || !cell.IsThrough
                                                                || !cell.InDirection(newDirection).IsPlayArea)
                            {
                                var possibles = _directions
                                    .Where(d => d != newDirection.Opposite())
                                    .Where(d => ghost.State == GhostState.Eyes
                                        ? cell.InDirection(d).IsGhostEyeArea
                                        : cell.InDirection(d).IsPlayArea)
                                    .OrderBy(d => nextGhostCell.Move(d).DistanceTo(target))
                                    .ToArray();

                                if (possibles.Length > 0)
                                    newDirection = possibles[0];
                            }

                            //if (ghost.State == GhostState.Alive && !cell.IsPlayArea)
                            //{
                            //    ghost.ForceTo(ExitGhostHouse);
                            //}
                        }

                        ghost.ChangeNextDirection(newDirection, nextGhostCell.Move(newDirection));
                    }

                    ghost.MoveTowardsTarget();
                    if (ghost.State == GhostState.Eyes)
                    {
                        ghost.MoveTowardsTarget();
                    }
                }
            }
        }

        private Direction[] _directions = new[] {Direction.Up, Direction.Left, Direction.Down, Direction.Right};


        private void MovePacMan()
        {
            var cell = Map.Cell(PacMan.Location);
            var close = PacMan.Location.CloseToCell;
            if (_inputDirection != InputDirection.None)
            {
                var isLeftRight = PacMan.Direction == Direction.Left || PacMan.Direction == Direction.Right;
                switch (_inputDirection)
                {
                    case InputDirection.Up:

                        if (cell.CellAbove.IsPlayArea)
                        {
                            if (!isLeftRight || close)
                                PacMan.ChangeDirection(Direction.Up);
                        }

                        break;

                    case InputDirection.Down:
                        if (cell.CellBelow.IsPlayArea)
                        {
                            if (!isLeftRight || close)
                                PacMan.ChangeDirection(Direction.Down);
                        }

                        break;

                    case InputDirection.Left:
                        if (cell.CellLeft.IsPlayArea)
                        {
                            if (isLeftRight || close)
                                PacMan.ChangeDirection(Direction.Left);
                        }

                        break;

                    case InputDirection.Right:
                        if (cell.CellRight.IsPlayArea)
                        {
                            if (isLeftRight || close)
                                PacMan.ChangeDirection(Direction.Right);
                        }

                        break;

                    default:
                        break;
                }
            }

            var target = cell;
            switch (PacMan.Direction)
            {
                case Direction.Up:
                    if (cell.CellAbove.IsPlayArea)
                        target = cell.CellAbove;
                    break;
                case Direction.Down:
                    if (cell.CellBelow.IsPlayArea)
                        target = cell.CellBelow;
                    break;
                case Direction.Left:
                    if (cell.CellLeft.IsPlayArea)
                        target = cell.CellLeft;
                    break;
                case Direction.Right:
                    if (cell.CellRight.IsPlayArea)
                        target = cell.CellRight;
                    break;
                default:
                    break;
            }

            PacMan.MoveTowards(target.Location);
        }

        private bool CheckPacManPill()
        {
            var cell = Map.Cell(PacMan.Location);
            if (cell.CellType != CellType.Pill && cell.CellType != CellType.ThroughSpacePill)
                return false;

            _score += 10;
            cell.RemovePill();
            Display.Update(_spriteSet.Blank, cell.X, cell.Y+BoardYOffset);
            return true;
        }

        private bool CheckPacManPowerPill()
        {
            var cell = Map.Cell(PacMan.Location);
            if (cell.CellType != CellType.PowerPill)
                return false;

            _score += 50;

            Map.RemovePowerPill(cell.Location);
            cell.RemovePill();
            Display.Update(_spriteSet.Blank, cell.X, cell.Y + BoardYOffset);
            return true;
        }

        private void DrawScore()
        {
            _scoreBoard.Player1Text(_scoreAnimation.IsZero);
            _scoreBoard.HighScoreText();
            _scoreBoard.Player1Score(_score);
            _scoreBoard.Credits(_uiSystem.Credits);
            _scoreBoard.HighScore(_uiSystem.GetAndUpdateHighScore(_score));
        }

        private void DrawSprites()
        {
            foreach (var power in Map.PowerPills)
            {
                var anim = _gameState == GameState.Intro || _gameState == GameState.GetReady
                                                         || _powerPillAnimation.IsZero;

                DrawBoardSprite(_spriteSet.PowerPill(anim), power);
            }

            if(_ghostEatenPause==0)
                DrawBoardSprite(_spriteSet.PacMan(PacMan), PacMan.Location);

            if (_gameState != GameState.Intro)
            {
                foreach (var ghost in Ghosts)
                {
                    DrawBoardSprite(_spriteSet.Ghost(ghost), ghost.Location);
                }

               
            }
        }

        private const int PacManDeadAnim = 31 + 7;

        private void DrawScreenSprite(SpriteSource sprite, Location location)
        {
            Display.AddSprite(sprite, location);
        }

        private void DrawBoardSprite(SpriteSource sprite, Location location)
        {
            DrawScreenSprite(sprite, location.Add(0, BoardYOffset));
        }

        private void DrawMap()
        {
            Display.Fill(_spriteSet.Blank);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Display.Update(_spriteSet.Map(Map.Cell(x, y).Piece), x, y + BoardYOffset);
                }
            }
        }
    }

    public enum GameState
    {
        Intro, GetReady, Playing,
        Frightened
    }
}
