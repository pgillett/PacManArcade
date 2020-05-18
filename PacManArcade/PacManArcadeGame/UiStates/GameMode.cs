using System;
using System.Collections.Generic;
using System.Linq;
using PacManArcadeGame.GameItems;
using PacManArcadeGame.Graphics;
using PacManArcadeGame.Helpers;
using PacManArcadeGame.Map;
using Random = PacManArcadeGame.Helpers.Random;

namespace PacManArcadeGame.UiStates
{
    public class GameMode : IUiMode
    {
        private readonly SpriteSet _spriteSet;

        private InputDirection _inputDirection => _inputs.Direction;
        private Inputs _inputs => _uiSystem.Inputs;

        public int MapHeight => Map.Height;
        public int MapWidth => Map.Width;

        public PacMan PacMan;

        private Ghost Blinky;
        private Ghost Pinky;
        private Ghost Inky;
        private Ghost Clyde;

        public IEnumerable<Ghost> Ghosts => new[] {Blinky, Pinky, Inky, Clyde};

        public Location ExitGhostHouse;
        public Location FruitLocation;

        public Map.Map Map;

        public Display Display;

        private GameSetup.GameSetup _gameSetup;

        private int Ticks = 0;

        private ScoreBoard _scoreBoard;

        private GameState _gameState;
        private int _nextTick = 0;

        private int _score = 0;
        private int _pillCount = 0;
        private int _fruitTick = 0;
        private bool _fruitScore = false;

        private bool _invincible = false;

        private int _lives = 3;

        private UiSystem _uiSystem;

        private int _level = 0;

        private const int BoardYOffset = 3;

        private Animation _powerPillAnimation = new Animation(2, 10);
        private Animation _scoreAnimation = new Animation(2, 16);

        private int _ghostEatenPause = 0;
        private int _ghostEatenPoints = 0;

        private int _chaseForTicks = 20 * 60;
        private int _scatterForTicks = 7 * 60;

        private bool _chaseMode;
        private int _chaseSwitchTick;

        private int _pinkyLeaveTick = 0;

        private bool _demoMode = false;
        private int _demoMoveNumber = 0;

        private Random _random;

        public GameMode(UiSystem uiSystem, GameSetup.GameSetup gameSetup, bool demoMode)
        {
            _uiSystem = uiSystem;
            Display = uiSystem.Display;
            _spriteSet = uiSystem.SpriteSet;

            _scoreBoard = uiSystem.ScoreBoard;

            _gameSetup = gameSetup;

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

            _random =new Random();

            _nextTick = 120;

            Map = _gameSetup.Map;
            ResetGhostsAndPacMan(true);
            DrawMap();
        }

        private void ResetGhostsAndPacMan(bool first)
        {
            _pinkyLeaveTick = first ? 0 : Ticks + 6 * 60;
            Blinky = _gameSetup.InitialBlinky.Ghost;
            Pinky = _gameSetup.InitialPinky.Ghost;
            Inky = _gameSetup.InitialInky.Ghost;
            Clyde = _gameSetup.InitialClyde.Ghost;
            PacMan = _gameSetup.InitialPacMan.PacMan;
            ExitGhostHouse = _gameSetup.ExitGhostHouse;
            FruitLocation = _gameSetup.FruitLocation;
        }

        public bool Tick()
        {
            Ticks++;
            Display.ClearSprites();
            _scoreAnimation.Tick();

            if (_gameState == GameState.Intro)
            {
                Display.WriteLine("PLAYER ONE", TextColour.Cyan, 9, 14);
                Display.WriteLine("READY!", TextColour.Yellow, 11, 20); 
                if (Ticks > _nextTick)
                {
                    _gameState = GameState.GetReady;
                    _nextTick = Ticks + 120;
                    _lives--;
                }
            }
            else if (_gameState == GameState.GetReady)
            {
                Display.WriteLine("          ", TextColour.Cyan, 9, 14);
                Display.WriteLine("      ", TextColour.Yellow, 11, 20);
                if (Ticks > _nextTick)
                {
                    _gameState = GameState.Playing;
                    SwitchChaseMode(true);
                }
            }
            else if(_gameState == GameState.Playing || _gameState == GameState.Frightened)
            {
                if (_inputs.Invincible)
                {
                    _invincible = !_invincible;
                    _inputs.Invincible = false;
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
                    if (CheckPacManPill())
                    {
                        if (_pillCount == 70 || _pillCount == 170)
                        {
                            _fruitTick = Ticks + 8 * 60;
                            _fruitScore = false;
                        }
                    }
                    else
                    {
                        MovePacMan();
                    }

                    MoveGhosts(false);
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

                    if (Ticks < _fruitTick)
                    {
                        if (!_fruitScore)
                        {
                            CheckPacManFruit();
                        }
                    }

                    if (_gameState == GameState.Frightened)
                    {
                        if (Ticks + (3 * 60) > _nextTick)
                        {
                            foreach (var ghost in Ghosts.Where(g=>g.Frightened && !g.FlashAnimation.Active))
                            {
                                ghost.SetFrightenedFlash();
                            }
                        }

                        if (Ticks > _nextTick)
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
                                        0 => 200,
                                        1 => 400,
                                        2 => 800,
                                        3 => 1600,
                                        _ => 0
                                    };
                                }
                            }
                            else if (!_invincible && ghost.State == GhostState.Alive)
                            {
                                _gameState = GameState.Caught;
                                _nextTick = Ticks + 60;
                            }
                        }
                    }
                }
            }
            else if (_gameState==GameState.Caught)
            {
                if (Ticks > _nextTick)
                {
                    _gameState = GameState.Dying;
                    _nextTick = Ticks + 4 * 60;
                    PacMan.Die();
                }
                _powerPillAnimation.Tick();
                foreach(var ghost in Ghosts)
                    ghost.Animation.Tick();
            }
            else if (_gameState==GameState.Dying)
            {
                PacMan.Animation.Tick();
                if (Ticks > _nextTick)
                {
                    if (_lives > 0)
                    {
                        _gameState = GameState.Intro;
                        ResetGhostsAndPacMan(false);
                    }
                    else
                    {
                        _gameState = GameState.GameOver;
                        _nextTick = Ticks + 4 * 60;
                    }
                }
            }
            else if(_gameState == GameState.GameOver)
            {
                if (Ticks > _nextTick)
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

       

        private bool SkipMove(int skipTickEvery) => (Ticks % skipTickEvery) == 0;

        private void MoveGhosts(bool onlyEyes)
        {
            foreach (var ghost in Ghosts)
            {
                if (onlyEyes && !ghost.IsEyesMode) continue;
                if(SkipMove(16)) continue;
                if ((ghost.IsSlowMo || Map.Cell(ghost.Location).CellType == CellType.Tunnel)
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
                            if (ghost.Colour == GhostColour.Red || (ghost.Colour == GhostColour.Pink && Ticks>_pinkyLeaveTick)
                                                                || (ghost.Colour == GhostColour.Cyan && _pillCount > 30)
                                                                || (ghost.Colour == GhostColour.Orange &&
                                                                    ((decimal) _pillCount) / Map.Pills > 0.3m))
                            {
                                ghost.SetToLeave(ExitGhostHouse);
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
                        var cell = Map.Cell(nextGhostCell);

                        var newDirection = ghost.Direction;

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
                                    if (ghost.Location.IsNearTo(ExitGhostHouse,1.5m))
                                    {
                                        ghost.SetToGhostDoor(ExitGhostHouse);
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
                    if (ghost.IsEyesMode)
                    {
                        ghost.MoveTowardsTarget();
                    }
                }
                ghost.Bounds(MapWidth, MapHeight);
            }
        }

        private readonly Direction[] _directions = new[] {Direction.Up, Direction.Left, Direction.Down, Direction.Right};


        private void MovePacMan()
        {
            var cell = Map.Cell(PacMan.Location);
            var close = PacMan.Location.CloseToCell;
            var inputDirection = _inputDirection;
            if (_demoMode)
            {
                var move = _gameSetup.DemoMoves[_demoMoveNumber];
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

            PacMan.Bounds(MapWidth, MapHeight);
        }

        private void CheckPacManFruit()
        {
            if (PacMan.Location.IsNearTo(FruitLocation))
            {
                _fruitScore = true;
                _fruitTick = Ticks + 120;
                if (!_demoMode)
                {
                    _score += _fruitScores[Bonus(_level)];
                }
            }
        }

        private int[] _fruitScores = new[] {100, 300, 500, 700, 1000, 2000, 3000, 5000};

        private bool CheckPacManPill()
        {
            var cell = Map.Cell(PacMan.Location);
            if (cell.CellType != CellType.Pill && cell.CellType != CellType.ThroughSpacePill)
                return false;

            if (!_demoMode)
            {
                _score += 10;
            }
            _pillCount++;
            cell.RemovePill();
            Display.Update(_spriteSet.Blank, cell.X, cell.Y+BoardYOffset);
            return true;
        }

        private bool CheckPacManPowerPill()
        {
            var cell = Map.Cell(PacMan.Location);
            if (cell.CellType != CellType.PowerPill)
                return false;

            if (!_demoMode)
            {
                _score += 50;
            }

            Map.RemovePowerPill(cell.Location);
            cell.RemovePill();
            Display.Update(_spriteSet.Blank, cell.X, cell.Y + BoardYOffset);
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
                Display.WriteLine("GAME  OVER", TextColour.Red, 9, 20);
            }
            else
            {
                for (int l = 0; l < _lives; l++)
                {
                    DrawBoardSprite(_spriteSet.PacMan(Direction.Left, 1, false),
                        new Location(2.5m + l * 2, MapHeight + 0.5m));
                }

                {
                    var col = 0;
                    for (int l = Math.Max(0, _level - 6); l <= _level; l++)
                    {
                        DrawBoardSprite(_spriteSet.Bonus[Bonus(l)],
                            new Location(24.5m - 2 * col, MapHeight + 0.5m));
                        col++;
                    }
                }
            }
        }

        private int Bonus(int level)
        {
            if (level < 2) return level;
            if (level > 12) return 7;
            return (level / 2) + 1;
        }

        private void DrawSprites()
        {
            if (Ticks < _fruitTick)
            {
                if (_fruitScore)
                {
                    var set = _spriteSet.BonusScores[Bonus(_level)];
                    var loc = FruitLocation.Add(-((set.Count - 1) / 2), 0);
                    for (int x = 0; x < set.Count; x++)
                    {
                        DrawBoardSprite(set[x], loc.Add(x, 0));
                    }
                }
                else
                {
                    DrawBoardSprite(_spriteSet.Bonus[_level], FruitLocation);
                }
            }

            if (_gameState == GameState.Intro
                || _gameState == GameState.GetReady
                || _powerPillAnimation.IsZero)
            {
                foreach (var power in Map.PowerPills)
                {
                    DrawBoardSprite(_spriteSet.PowerPill, power);
                }
            }

            if (_gameState != GameState.Intro
                && _gameState != GameState.GameOver
                && _ghostEatenPause == 0
                && (!_invincible || (Ticks % 6) < 3))
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
            Display.AddSprite(sprite, location);
        }

        private void DrawBoardSprite(SpriteSource sprite, Location location)
        {
            DrawScreenSprite(sprite, location.Add(0, BoardYOffset));
        }

        private void DrawMap()
        {
            Display.Fill(_spriteSet.Blank);
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    Display.Update(_spriteSet.Map(Map.Cell(x, y).Piece), x, y + BoardYOffset);
                }
            }
        }
    }
}
