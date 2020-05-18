using System;
using System.Collections.Generic;
using System.Linq;
using PacManArcadeGame.GameSetup;
using PacManArcadeGame.Graphics;
using PacManArcadeGame.Helpers;
using PacManArcadeGame.Map;
using PacManArcadeGame.UiStates;
using Random = PacManArcadeGame.Helpers.Random;

namespace PacManArcadeGame.GameItems
{
    public class Game
    {
        private readonly SpriteSet _spriteSet;

        private InputDirection InputDirection => Inputs.Direction;
        private Inputs Inputs => _uiSystem.Inputs;

        private int MapHeight => _map.Height;
        private int MapWidth => _map.Width;

        private PacMan PacMan;

        private Ghost Blinky;
        private Ghost Pinky;
        private Ghost Inky;
        private Ghost Clyde;

        private IEnumerable<Ghost> Ghosts => new[] {Blinky, Pinky, Inky, Clyde};

        private Location _exitGhostHouse;

        private Map.Map _map;

        private readonly Display _display;

        private readonly LevelSetup _levelSetup;

        private int _ticks = 0;

        private readonly ScoreBoard _scoreBoard;

        private GameState _gameState;
        private int _nextTick = 0;

        private int _score = 0;
        private int _pillCount = 0;

        private readonly BonusFruit _bonusFruit;

        private bool _invincible = false;

        private int _lives = 3;

        private readonly UiSystem _uiSystem;

        private int _level = 0;

        private const int BoardYOffset = 3;

        private readonly Animation _powerPillAnimation = new Animation(2, 10);
        private readonly Animation _scoreAnimation = new Animation(2, 16);

        private int _ghostEatenPause = 0;
        private PointsMultiplier _ghostEatenPoints;

        private int _chaseForTicks = 20 * 60;
        private int _scatterForTicks = 7 * 60;

        private bool _chaseMode;
        private int _chaseSwitchTick;

        private int _pinkyLeaveTick = 0;

        private readonly bool _demoMode;
        private int _demoMoveNumber = 0;

        private int _flashCounter = 0;

        private Random _random;

        protected Game(UiSystem uiSystem, LevelSetup levelSetup, bool demoMode)
        {
            _uiSystem = uiSystem;
            _display = uiSystem.Display;
            _spriteSet = uiSystem.SpriteSet;

            _scoreBoard = uiSystem.ScoreBoard;

            _levelSetup = levelSetup;

            if (demoMode)
            {
                _demoMode = true;
                _lives = 0;
                _gameState = GameState.GetReady;
            }
            else
            {
                _lives = 3;
                _gameState = GameState.Intro;
            }

            _bonusFruit= new BonusFruit(_levelSetup.FruitLocation);

            _nextTick = 120;

            StartLevel();
        }

        private void ResetGhostsAndPacMan(bool first)
        {
            _pinkyLeaveTick = first ? 0 : _ticks + 6 * 60;
            Blinky = _levelSetup.InitialBlinky;
            Pinky = _levelSetup.InitialPinky;
            Inky = _levelSetup.InitialInky;
            Clyde = _levelSetup.InitialClyde;
            PacMan = _levelSetup.InitialPacMan;
        }

        private void StartLevel()
        {
            _map = _levelSetup.Map;
            _exitGhostHouse = _levelSetup.ExitGhostHouse;
            ResetGhostsAndPacMan(true);
            DrawMap(false);
            _pillCount = 0;
            _powerPillAnimation.Reset();
            _scoreAnimation.Reset();
            _bonusFruit.SetLevel(_level);
            _random = new Random();

        }

        public bool Tick()
        {
            _ticks++;
            _display.ClearSprites();
            _scoreAnimation.Tick();

            if (_gameState == GameState.Intro)
            {
                _display.WriteLine("PLAYER ONE", TextColour.Cyan, 9, 14);
                _display.WriteLine("READY!", TextColour.Yellow, 11, 20); 
                if (_ticks > _nextTick)
                {
                    _gameState = GameState.GetReady;
                    _nextTick = _ticks + 120;
                    _lives--;
                }
            }
            else if (_gameState == GameState.GetReady)
            {
                _display.WriteLine("          ", TextColour.Cyan, 9, 14);
                _display.WriteLine("      ", TextColour.Yellow, 11, 20);
                if (_ticks > _nextTick)
                {
                    _gameState = GameState.Playing;
                    SwitchChaseMode(true);
                }
            }
            else if(_gameState == GameState.Playing || _gameState == GameState.Frightened)
            {
                if (Inputs.Invincible)
                {
                    _invincible = !_invincible;
                    Inputs.Invincible = false;
                }

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
                    MoveGhosts(true);
                }
                else
                {
                    if (!CheckPacManPill())
                    {
                        MovePacMan();
                    }

                    _bonusFruit.Tick(_pillCount);

                    MoveGhosts(false);
                    if (CheckPacManPowerPill())
                    {
                        foreach (var ghost in Ghosts)
                        {
                            ghost.SetFrightened();
                        }

                        _gameState = GameState.Frightened;
                        _nextTick = _ticks + 7 * 60;
                        _ghostEatenPoints = 0;
                    }

                    if(_bonusFruit.ShowAsFruit)
                    {
                        CheckPacManFruit();
                    }

                    if (_gameState == GameState.Frightened)
                    {
                        if (_ticks + (3 * 60) > _nextTick)
                        {
                            foreach (var ghost in Ghosts.Where(g=>g.Frightened && !g.FlashAnimation.Active))
                            {
                                ghost.SetFrightenedFlash();
                            }
                        }

                        if (_ticks > _nextTick)
                        {
                            _gameState = GameState.Playing;
                            SwitchChaseMode(true);
                            foreach (var ghost in Ghosts.Where(g => g.Frightened))
                            {
                                ghost.SetNotFrightened();
                            }
                        }
                    }

                    foreach (var ghost in Ghosts)
                    {
                        if (PacMan.Location.IsNearTo(ghost.Location))
                        {
                            if (ghost.Frightened)
                            {
                                ghost.SetEaten(_ghostEatenPoints);
                                _ghostEatenPause = 60;
                                _ghostEatenPoints++;
                                if (!_demoMode)
                                {
                                    _score += _ghostEatenPoints switch
                                    {
                                        PointsMultiplier.Pts200 => 200,
                                        PointsMultiplier.Pts400 => 400,
                                        PointsMultiplier.Pts800 => 800,
                                        PointsMultiplier.Pts1600 => 1600,
                                        _ => 0
                                    };
                                }
                            }
                            else if (!_invincible && ghost.State == GhostState.Alive)
                            {
                                _gameState = GameState.Caught;
                                _nextTick = _ticks + 60;
                            }
                        }
                    }
                }

                if (_pillCount == _map.Pills)
                {
                    _gameState = GameState.Complete;
                    _nextTick = _ticks + 120;
                }
            }
            else if (_gameState == GameState.Complete)
            {
                if (_ticks > _nextTick)
                {
                    _gameState = GameState.Flash;
                    _nextTick = _ticks + 12;
                    _flashCounter = 0;
                }
            }
            else if (_gameState == GameState.Flash)
            {
                if (_ticks > _nextTick)
                {
                    _flashCounter = _flashCounter + 1;
                    if (_flashCounter < 9)
                    {
                        DrawMap(_flashCounter % 2 == 1);
                        _nextTick = _ticks + 12;
                    }
                    else
                    {
                        _display.Blank();
                        _nextTick = _ticks + 20;
                    }

                    if (_flashCounter > 9)
                    {
                        _level++;
                        _gameState = GameState.GetReady;
                        StartLevel();
                        _nextTick = _ticks + 120;
                    }
                }
            }
            else if (_gameState==GameState.Caught)
            {
                if (_ticks > _nextTick)
                {
                    _gameState = GameState.Dying;
                    _nextTick = _ticks + 4 * 60;
                    PacMan.Die();
                }
                _powerPillAnimation.Tick();
                foreach(var ghost in Ghosts)
                    ghost.Animation.Tick();
            }
            else if (_gameState==GameState.Dying)
            {
                PacMan.Animation.Tick();
                if (_ticks > _nextTick)
                {
                    if (_lives > 0)
                    {
                        _gameState = GameState.Intro;
                        ResetGhostsAndPacMan(false);
                    }
                    else
                    {
                        _gameState = GameState.GameOver;
                        _nextTick = _ticks + 4 * 60;
                    }
                }
            }
            else if(_gameState == GameState.GameOver)
            {
                if (_ticks > _nextTick)
                    return false;
            }

            DrawScore();
            DrawSprites();

            return true;
        }

        private void SwitchChaseMode(bool force)
        {
            if (force)
            {
                _chaseMode = false;
                _chaseSwitchTick = _ticks + _scatterForTicks;
            }

            if (_gameState == GameState.Playing && _ticks > _chaseSwitchTick)
            {
                _chaseMode = !_chaseMode;
                _chaseSwitchTick = _ticks + (_chaseMode ? _chaseForTicks: _scatterForTicks);
                foreach (var ghost in Ghosts)
                {
                    ghost.FlipDirection();
                }
            }
        }
        
        private bool SkipMove(int skipTickEvery) => (_ticks % skipTickEvery) == 0;

        private void MoveGhosts(bool onlyEyes)
        {
            foreach (var ghost in Ghosts)
            {
                if (onlyEyes && !ghost.IsEyesMode) continue;
                if(SkipMove(16)) continue;
                if ((ghost.IsSlowMo || _map.Cell(ghost.Location).CellType == CellType.Tunnel)
                    && SkipMove(2)) continue;

                if (ghost.IsForcedMovement)
                {
                    ghost.MoveTowardsTarget();
                    if (ghost.IsAtTarget)
                    {
                        if (ghost.State == GhostState.GhostDoor)
                        {
                            ghost.SetToIntoHouse();
                        }
                        else if (ghost.State == GhostState.IntoHouse)
                        {
                            ghost.SetInHouse();
                        }
                        else if (ghost.State == GhostState.InHouse)
                        {
                            if (ghost.Colour == GhostColour.Red || (ghost.Colour == GhostColour.Pink && _ticks>_pinkyLeaveTick)
                                                                || (ghost.Colour == GhostColour.Cyan && _pillCount > 30)
                                                                || (ghost.Colour == GhostColour.Orange &&
                                                                    ((decimal) _pillCount) / _map.Pills > 0.3m))
                            {
                                ghost.SetToLeave(_exitGhostHouse);
                            }
                            else
                            {
                                ghost.JiggleInHouse();
                            }
                        }
                        else if (ghost.State == GhostState.LeaveHouse)
                        {
                            ghost.SetAlive();
                        }
                    }
                }
                else
                {
                    if (ghost.IsAtTarget)
                    {
                        ghost.ChangeDirection();

                        var nextGhostCell = ghost.CurrentTarget.Cell;
                        var cell = _map.Cell(nextGhostCell);

                        var newDirection = ghost.NextDirection;

                        if (ghost.Frightened)
                        {
                            var directions = _directions
                                .Where(d => d != newDirection.Opposite())
                                .Where(d => cell.InDirection(d).IsPlayArea)
                                .ToArray();
                            newDirection = directions[_random.Get(directions.Length)];
                        }
                        else if (ghost.State == GhostState.LeaveHouse)
                        {
                            ghost.SetAlive();
                            ghost.SetNextDirection(Direction.Left, nextGhostCell.Move(Direction.Left));
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
                                    target = _exitGhostHouse;
                                    if (ghost.Location.IsNearTo(_exitGhostHouse,1.5m))
                                    {
                                        ghost.SetTargetToGhostDoor(_exitGhostHouse);
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

                        ghost.SetNextDirection(newDirection, nextGhostCell.Move(newDirection));
                    }

                    ghost.MoveTowardsTarget();
                    if (ghost.IsEyesMode)
                    {
                        ghost.MoveTowardsTarget();
                    }
                }
                ghost.KeepInBounds(MapWidth, MapHeight);
            }
        }

        private readonly Direction[] _directions = {Direction.Up, Direction.Left, Direction.Down, Direction.Right};
        
        private void MovePacMan()
        {
            var cell = _map.Cell(PacMan.Location);
            var close = PacMan.Location.CloseToCell;
            var inputDirection = InputDirection;
            if (_demoMode)
            {
                var move = _levelSetup.DemoMoves[_demoMoveNumber];
                if (close && cell.Location.IsSameCell(move.Location))
                {
                    _demoMoveNumber++;
                }

                inputDirection = move.Direction switch
                {
                    Direction.Up => InputDirection.Up,
                    Direction.Down => InputDirection.Down,
                    Direction.Left => InputDirection.Left,
                    Direction.Right => InputDirection.Right,
                    _ => throw new NotImplementedException()
                };
            }

            if (inputDirection != InputDirection.None)
            {
                var isLeftRight = PacMan.Direction == Direction.Left || PacMan.Direction == Direction.Right;
                switch (inputDirection)
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

            PacMan.KeepInBounds(MapWidth, MapHeight);
        }

        private void CheckPacManFruit()
        {
            if (PacMan.Location.IsNearTo(_bonusFruit.Location))
            {
                _bonusFruit.Eaten();
                if (!_demoMode)
                {
                    _score += _bonusFruit.Score;
                }
            }
        }

       

        private bool CheckPacManPill()
        {
            var cell = _map.Cell(PacMan.Location);
            if (cell.CellType != CellType.Pill && cell.CellType != CellType.ThroughSpacePill)
                return false;

            if (!_demoMode)
            {
                _score += 10;
            }
            _pillCount++;
            cell.RemovePill();
            _display.Update(_spriteSet.Blank, cell.X, cell.Y+BoardYOffset);
            return true;
        }

        private bool CheckPacManPowerPill()
        {
            var cell = _map.Cell(PacMan.Location);
            if (cell.CellType != CellType.PowerPill)
                return false;

            if (!_demoMode)
            {
                _score += 50;
            }

            _map.RemovePowerPill(cell.Location);
            cell.RemovePill();
            _display.Update(_spriteSet.Blank, cell.X, cell.Y + BoardYOffset);
            return true;
        }

        private void DrawScore()
        {
            _scoreBoard.Player1Text(_scoreAnimation.IsZero || _demoMode);
            _scoreBoard.HighScoreText();
            _scoreBoard.Player1Score(_score);
            _scoreBoard.HighScore(_uiSystem.GetAndUpdateHighScore(_score));


            if (_gameState == GameState.GameOver || _demoMode)
            {
                _scoreBoard.Credits(_uiSystem.Credits);
                _display.WriteLine("GAME  OVER", TextColour.Red, 9, 20);
            }
            else
            {
                for (int l = 0; l < _lives; l++)
                {
                    DrawBoardSprite(_spriteSet.PacMan(Direction.Left, 1, false),
                        new Location(2.5m + l * 2, MapHeight + 0.5m));
                }

                {
                    var fl = _bonusFruit.FruitList;
                    for(int i = 0;i<fl.Count;i++)
                    {
                        DrawBoardSprite(_spriteSet.Fruit[fl[i]],
                            new Location(24.5m - 2 * i, MapHeight + 0.5m));
                    }
                }
            }
        }
        
        private void DrawSprites()
        {
            if (_bonusFruit.ShowAsFruit)
            {
                DrawBoardSprite(_spriteSet.Fruit[_bonusFruit.Type], _bonusFruit.Location);
            }
            if (_bonusFruit.ShowAsScore)
            {
                var set = _spriteSet.BonusScores[_bonusFruit.Type];
                var loc = _bonusFruit.Location.Add(-(set.Count - 1) * 0.5m, 0);
                for (int x = 0; x < set.Count; x++)
                {
                    DrawBoardSprite(set[x], loc.Add(x, 0));
                }
            }


            if (_gameState == GameState.Intro
                || _gameState == GameState.GetReady
                || _powerPillAnimation.IsZero)
            {
                foreach (var power in _map.PowerPills)
                {
                    DrawBoardSprite(_spriteSet.PowerPill, power);
                }
            }

            if (_gameState != GameState.Intro
                && _gameState != GameState.GameOver
                && _ghostEatenPause == 0
                && (!_invincible || (_ticks % 6) < 3))
            {
                DrawBoardSprite(_spriteSet.PacMan(PacMan), PacMan.Location);
            }

            if (_gameState == GameState.GetReady
                || _gameState == GameState.Playing
                || _gameState == GameState.Frightened
                || _gameState == GameState.Caught)
            {
                foreach (var ghost in Ghosts)
                {
                    DrawBoardSprite(_spriteSet.Ghost(ghost), ghost.Location);
                }
            }
        }

        private void DrawScreenSprite(SpriteSource sprite, Location location)
        {
            _display.AddSprite(sprite, location);
        }

        private void DrawBoardSprite(SpriteSource sprite, Location location)
        {
            DrawScreenSprite(sprite, location.Add(0, BoardYOffset));
        }

        private void DrawMap(bool white)
        {
            _display.Fill(_spriteSet.Blank);
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    _display.Update(_spriteSet.Map(_map.Cell(x, y).Piece, white), x, y + BoardYOffset);
                }
            }
        }
    }
}
