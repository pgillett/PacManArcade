using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PacManArcadeGame.Ai;
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
        private readonly GameDraw _gameDraw;

        private InputDirection InputDirection => Inputs.Direction;
        private Inputs Inputs => _uiSystem.Inputs;

        private int MapHeight => _map.Height;
        private int MapWidth => _map.Width;

        private PacMan _pacMan;

        private Ghost _blinky;
        private Ghost _pinky;
        private Ghost _inky;
        private Ghost _clyde;

        private IEnumerable<Ghost> Ghosts => new[] {_blinky, _pinky, _inky, _clyde};

        private Location _exitGhostHouse;

        private Map.Map _map;

        private readonly LevelSetup _levelSetup;

        private int _score = 0;
        private int _pillCount = 0;

        private readonly BonusFruit _bonusFruit;

        private bool _invincible = false;

        private int _lives;

        private readonly UiSystem _uiSystem;

        private int _level = 0;

        private readonly Animation _powerPillAnimation = new Animation(2, 10);
        private readonly Animation _scoreAnimation = new Animation(2, 16);
        private readonly Animation _invincibleAnimation = new Animation(2, 6);

        private int _ghostEatenPause = 0;
        private PointsMultiplier _ghostEatenPoints;

        private int _pacManPause = 0;

        private readonly bool _demoMode;

        private int _flashCounter = 0;

        private AiBot _aiBot;

        private Random _random;

        private readonly StateMachine<GameState> _stateMachine = new StateMachine<GameState>(GameState.Intro);
        private readonly TickCounter _tick = new TickCounter();

        private readonly ScatterChase _scatterChase = new ScatterChase();
        private readonly Speeds _speeds = new Speeds();

        private readonly SpeedCounter _pacManSpeed = new SpeedCounter();

        private readonly GhostHouse _ghostHouse = new GhostHouse();

        protected Game(UiSystem uiSystem, LevelSetup levelSetup, bool demoMode)
        {
            _uiSystem = uiSystem;

            _gameDraw = new GameDraw(uiSystem.SpriteSet, uiSystem.Display, uiSystem.ScoreBoard);

            _levelSetup = levelSetup;

            if (demoMode)
            {
                _demoMode = true;
                _lives = 0;
                _stateMachine.ChangeState(GameState.GetReady);
            }
            else
            {
                _lives = 3;
                _stateMachine.ChangeState(GameState.Intro);
            }

            _bonusFruit= new BonusFruit(_levelSetup.FruitLocation);

            StartLevel();
        }

        private int PillsRemaining => _map.Pills - _pillCount;

        private void ResetGhostsAndPacMan()
        {
            _blinky = _levelSetup.InitialBlinky;
            _pinky = _levelSetup.InitialPinky;
            _inky = _levelSetup.InitialInky;
            _clyde = _levelSetup.InitialClyde;
            _pacMan = _levelSetup.InitialPacMan;
            _scatterChase.Reset(_level);
        }

        private void StartLevel()
        {
            _map = _levelSetup.Map;
            _map.Reset();
            _aiBot = new AiBot(_map);
            _exitGhostHouse = _levelSetup.ExitGhostHouse;
            ResetGhostsAndPacMan();
            _gameDraw.DrawMap(_map, false);
            _pillCount = 0;
            _powerPillAnimation.Reset();
            _scoreAnimation.Reset();
            _bonusFruit.SetLevel(_level);
            _ghostHouse.SetLevel(_level, Ghosts);
            _random = new Random();
        }

        public bool Tick()
        {
            _stateMachine.Start();

            _tick.Tick();

            _gameDraw.ClearSprites();
            _scoreAnimation.Tick();
            _invincibleAnimation.Tick();

            Inputs.Invincible.On(() => _invincible = !_invincible);

            Inputs.LevelSkip.On(()=> _stateMachine.ChangeState(GameState.Complete));

            _stateMachine
                .OnEntry(GameState.Intro,
                    () => _tick.NextEventAfter(120),
                    StartLevel)
                .During(GameState.Intro,
                  () => _gameDraw.DisplayReadyText(),
                    () => _tick.AtEvent(() => _stateMachine.ChangeState(GameState.StartOfLife))
                );

            _stateMachine.OnEntry(GameState.StartOfLife, () => _lives--);

            _stateMachine.OnEntry(GameState.NewLevel, StartLevel);

            _stateMachine.During(new [] {GameState.StartOfLife, GameState.NewLevel}, 
                () => _stateMachine.ChangeState(GameState.GetReady));

            _stateMachine.OnEntry(GameState.GetReady,
                () => _tick.NextEventAfter(120),
                ResetGhostsAndPacMan);

            _stateMachine.During(GameState.GetReady,
                () => _gameDraw.ClearReadyText(),
                () => _tick.AtEvent(() => _stateMachine.ChangeState(GameState.Playing)),
                () => _scatterChase.Reset(_level),
                () => _speeds.SetLevel(_level));

            _stateMachine.During(new[] {GameState.Playing, GameState.Frightened},
                MainLoop);

            _stateMachine.OnEntry(GameState.Complete,
                () => _tick.NextEventAfter(120));

            _stateMachine.During(GameState.Complete,
                () => _tick.AtEvent(() => _stateMachine.ChangeState(GameState.Flash)));

            _stateMachine
                .OnEntry(GameState.Flash,
                    () => _tick.NextEventAfter(12),
                    () => { _flashCounter = 0; })
                .During(GameState.Flash,
                    () => _tick.AtEvent(FlashMap));

            _stateMachine
                .OnEntry(GameState.Caught,
                    () => _tick.NextEventAfter(60))
                .During(GameState.Caught,
                    () => _tick.AtEvent(() =>
                    {
                        _stateMachine.ChangeState(GameState.Dying);
                        _pacMan.Die();
                    }),
                    () =>
                    {
                        _powerPillAnimation.Tick();
                        DoToGhosts(g=>g.Animate());
                    }
                );

            _stateMachine
                .OnEntry(GameState.Dying,
                    () => _tick.NextEventAfter(4*60))
                .During(GameState.Dying,
                    () => _pacMan.Animation.Tick(),
                    () => _tick.AtEvent(() =>
                    {
                        if (_lives > 0)
                        {
                            _stateMachine.ChangeState(GameState.StartOfLife);
                            ResetGhostsAndPacMan();
                            _ghostHouse.LifeLost();
                        }
                        else
                        {
                            _stateMachine.ChangeState(GameState.GameOver);
                        }
                    }));

            _stateMachine
                .OnEntry(GameState.GameOver,
                    () => _tick.NextEventAfter(4 * 60))
                .During(GameState.GameOver,
                    () => _tick.AtEvent(() => { _stateMachine.End(); }));

            Draw();

            return !_stateMachine.HasEnded;
        }

        private void MainLoop()
        {
            _ghostHouse.Tick();
            _ghostHouse.SetActiveGhost(Ghosts);

            _stateMachine.During(GameState.Playing, CheckAndSwitchChaseMode);

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
                DoToGhosts(g => g.Animate());

                if (CheckPacManPill())
                {
                    _pacManPause = 1;
                    _ghostHouse.PillEaten();
                }

                _bonusFruit.Tick(_pillCount);

                MoveGhosts(false);

                if (CheckPacManPowerPill())
                {
                    _pacManPause = 3;
                    _ghostHouse.PillEaten();
                    DoToGhosts(g => g.SetFrightened(_speeds.FrightenedFlashTime));

                    _stateMachine.ChangeState(GameState.Frightened);
                }

                if (_pacManPause == 0)
                {
                    _pacManSpeed.SetSpeed(_stateMachine.IsCurrent(GameState.Frightened)
                        ? _speeds.PacManFrightenedSpeed
                        : _speeds.PacManSpeed);
                    _pacManSpeed.Tick();
                    MovePacMan(true);
                    if (_pacManSpeed.ExtraFrame)
                    {
                        MovePacMan(false);
                    }
                }
                else
                {
                    _pacManPause--;
                }


                if (_bonusFruit.ShowAsFruit)
                {
                    CheckPacManFruit();
                }

                _stateMachine
                    .OnTrigger(GameState.Frightened,
                        () => _tick.NextEventAfter(_speeds.FrightenedTime),
                        () => { _ghostEatenPoints = 0; })
                    .During(GameState.Frightened,
                        () => _tick.AtEvent(() => _stateMachine.ChangeState(GameState.Playing)))
                    .OnExit(GameState.Frightened,
                        () =>
                        {
                            foreach (var ghost in Ghosts.Where(g => g.Frightened))
                            {
                                ghost.SetNotFrightened();
                            }
                        });

                foreach (var ghost in Ghosts)
                {
                    if (_pacMan.Location.IsNearTo(ghost.Location))
                    {
                        if (ghost.Frightened)
                        {
                            EatGhost(ghost);
                        }
                        else if (!_invincible && ghost.State == GhostState.Alive)
                        {
                            _stateMachine.ChangeState(GameState.Caught);
                        }
                    }
                }
            }

            if (_pillCount == _map.Pills)
            {
                _stateMachine.ChangeState(GameState.Complete);
            }
        }

        private void EatGhost(Ghost ghost)
        {
            ghost.SetEaten(_ghostEatenPoints);
            _ghostEatenPause = 60;
            _tick.PushEvent(60);
            IncreaseScore(_ghostEatenPoints switch
                {
                    PointsMultiplier.Pts200 => 200,
                    PointsMultiplier.Pts400 => 400,
                    PointsMultiplier.Pts800 => 800,
                    PointsMultiplier.Pts1600 => 1600,
                    _ => 0
                }
            );
            _ghostEatenPoints++;
        }

        private void FlashMap()
        {
            _flashCounter = _flashCounter + 1;
            if (_flashCounter < 9)
            {
                _gameDraw.DrawMap(_map, _flashCounter % 2 == 1);
                _tick.NextEventAfter(12);
            }
            else
            {
                _gameDraw.Blank();
                _tick.NextEventAfter(20);
            }

            if (_flashCounter > 9)
            {
                _level++;
                _stateMachine.ChangeState(GameState.NewLevel);
            }
        }

        private void CheckAndSwitchChaseMode()
        {
            if (_scatterChase.Tick())
            {
                DoToGhosts(g=> g.FlipDirection());
            }
        }

        private void DoToGhosts(Action<Ghost> action)
        {
            foreach (var ghost in Ghosts)
                action(ghost);
        }

        private void MoveGhosts(bool onlyEyes)
        {
            foreach (var ghost in Ghosts)
            {
                if (ghost.IsEyesMode)
                {
                    MoveGhost(ghost);
                    MoveGhost(ghost);
                }
                else if (!onlyEyes)
                {
                    ghost.SpeedCounter.Tick();
                    if (_map.Cell(ghost.Location).CellType == CellType.Tunnel)
                    {
                        ghost.SpeedCounter.SetSpeed(_speeds.GhostTunnelSpeed);
                    }
                    else if (ghost.IsSlowMo)
                    {
                        ghost.SpeedCounter.SetSpeed(_speeds.GhostFrightenedSpeed);
                    }
                    else
                    {
                        ghost.SpeedCounter.SetSpeed(ghost.Colour == GhostColour.Red
                            ? _speeds.ElroySpeed(PillsRemaining)
                            : _speeds.GhostSpeed);
                    }

                    if (!ghost.SpeedCounter.SkipFrame)
                        MoveGhost(ghost);
                    if (ghost.SpeedCounter.ExtraFrame)
                        MoveGhost(ghost);
                }
            }
        }

        private void MoveGhost(Ghost ghost)
        {
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
                        if (ghost.Colour == GhostColour.Red || _ghostHouse.ShouldLeave(ghost))
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

                    var inChase = _scatterChase.InChaseMode ||
                                  (ghost.Colour == GhostColour.Red
                                   && _speeds.InElroy(PillsRemaining)
                                   && Ghosts.All(g => g.State == GhostState.Alive));

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
                                target =inChase
                                    ? ghost.GetChaseTarget(_pacMan, _blinky.Location)
                                    : ghost.ScatterTarget;
                                break;
                            case GhostState.Eyes:
                                target = _exitGhostHouse;
                                if (ghost.Location.IsNearTo(_exitGhostHouse, 1.5m))
                                {
                                    ghost.SetTargetToGhostDoor(_exitGhostHouse);
                                }

                                break;

                            default:
                                target = ghost.Location;
                                break;
                        }

                        if (ghost.State != GhostState.Alive || !cell.IsThroughSpace
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
                    }

                    ghost.SetNextDirection(newDirection, nextGhostCell.Move(newDirection));
                }

                ghost.MoveTowardsTarget();
            }

            ghost.KeepInBounds(MapWidth, MapHeight);
        }

        private readonly Direction[] _directions = {Direction.Up, Direction.Left, Direction.Down, Direction.Right};
        
        private void MovePacMan(bool withAnimation)
        {
            var cell = _map.Cell(_pacMan.Location);
            var close = _pacMan.Location.CloseToCell;
            var inputDirection = InputDirection;
            if (_demoMode && close)
            {
                var direction = _aiBot.BestMove(_pacMan.Location, _pacMan.Direction, Ghosts);
                inputDirection = direction switch
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
                var isLeftRight = _pacMan.Direction == Direction.Left || _pacMan.Direction == Direction.Right;
                switch (inputDirection)
                {
                    case InputDirection.Up:

                        if (cell.CellAbove.IsPlayArea)
                        {
                            if (!isLeftRight || close)
                                _pacMan.ChangeDirection(Direction.Up);
                        }

                        break;

                    case InputDirection.Down:
                        if (cell.CellBelow.IsPlayArea)
                        {
                            if (!isLeftRight || close)
                                _pacMan.ChangeDirection(Direction.Down);
                        }

                        break;

                    case InputDirection.Left:
                        if (cell.CellLeft.IsPlayArea)
                        {
                            if (isLeftRight || close)
                                _pacMan.ChangeDirection(Direction.Left);
                        }

                        break;

                    case InputDirection.Right:
                        if (cell.CellRight.IsPlayArea)
                        {
                            if (isLeftRight || close)
                                _pacMan.ChangeDirection(Direction.Right);
                        }

                        break;

                    default:
                        break;
                }
            }

            var target = cell;
            switch (_pacMan.Direction)
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

            _pacMan.MoveTowards(target.Location, withAnimation);

            _pacMan.KeepInBounds(MapWidth, MapHeight);
        }

        private void IncreaseScore(int increase)
        {
            if (!_demoMode)
            {
                _score += increase;
            }
        }

        private void CheckPacManFruit()
        {
            if (_pacMan.Location.IsNearTo(_bonusFruit.Location))
            {
                _bonusFruit.Eaten();
                IncreaseScore(_bonusFruit.Score);
            }
        }
        
        private bool CheckPacManPill()
        {
            var cell = _map.Cell(_pacMan.Location);
            if (cell.CellType != CellType.Pill || cell.PillEaten)
                return false;

            IncreaseScore(10);
            _pillCount++;
            cell.RemovePill();
            _gameDraw.RemovePill(cell.X, cell.Y);
            return true;
        }

        private bool CheckPacManPowerPill()
        {
            var cell = _map.Cell(_pacMan.Location);
            if (cell.CellType != CellType.PowerPill || cell.PillEaten)
                return false;

            IncreaseScore(50);

            _map.RemovePowerPill(cell.Location);
            cell.RemovePill();
            _gameDraw.RemovePill(cell.X, cell.Y);
            return true;
        }

        private void Draw()
        {
            _gameDraw.DrawScore(_score, _uiSystem.GetAndUpdateHighScore(_score),
                _scoreAnimation.IsZero || _demoMode,
                _stateMachine.IsCurrent(GameState.GameOver) || _demoMode,
                _uiSystem.Credits, _lives,
                _bonusFruit.FruitList,
                _map.Height);

            _gameDraw.DrawFruit(_bonusFruit);

            if (_stateMachine.IsCurrent(GameState.Intro, GameState.GetReady)
                || _powerPillAnimation.IsZero)
            {
                _gameDraw.DrawPowerPills(_map.PowerPills);
            }

            if (!_stateMachine.IsCurrent(GameState.Intro, GameState.GameOver)
                && _ghostEatenPause == 0
                && (!_invincible || _invincibleAnimation.IsZero))
            {
                _gameDraw.DrawPacMan(_pacMan);
            }

            _stateMachine.During(new[]
            {
                GameState.GetReady, GameState.Playing, GameState.Frightened, GameState.Caught
            }, () => _gameDraw.DrawGhosts(Ghosts));
        }
    }
}
