using PacManArcadeGame.Graphics;
using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.GameItems
{
    public class Ghost
    {
        public readonly GhostColour Colour;
        public readonly Location HomeLocation;
        public readonly Location ScatterTarget;
        public readonly Animation FlashAnimation;
        public readonly Animation Animation;

        public Location Location { get; private set; }
        public Direction NextDirection { get; private set; }
        public Location NextTarget { get; private set; }
        public GhostState State { get; private set; }
        public Location CurrentTarget { get; private set; }
        public Direction CurrentDirection { get; private set; }
        public int SkipTickEvery { get; private set; }
        public PointsMultiplier ShowAsPoints { get; private set; }
        public bool Frightened { get; private set; }

        public Ghost(GhostColour colour, Location location, Direction direction)
            : this(colour, location, direction, new Location(0, 0), new Location(0, 0), false)
        {

        }

        public Ghost(GhostColour colour, Location location, Direction direction, 
            Location scatterTarget, Location homeLocation, bool startInHouse)
        {
            Colour = colour;
            Location = location;
            NextDirection = direction;
            ScatterTarget = scatterTarget;
            Animation = new Animation(2, 10);
            FlashAnimation = new Animation(2, 7, true);
            FlashAnimation.Stop();
            State = startInHouse ? GhostState.InHouse : GhostState.Alive;
            HomeLocation = homeLocation;
            NextTarget = Location.Cell;
            ChangeDirection();
            NextTarget = Location.Cell.Move(CurrentDirection);
            SkipTickEvery = 16;
        }

        public void Move(decimal x, decimal y)
        {
            Location = Location.Add(x, y);
            Animation.Tick();
            FlashAnimation.Tick();
        }

        public void MoveTowardsTarget()
        {
            var dx = (Location.X < CurrentTarget.X ? 0.125m : 0) +
                     (Location.X > CurrentTarget.X ? -0.125m : 0);
            var dy = (Location.Y < CurrentTarget.Y ? 0.125m : 0) +
                     (Location.Y > CurrentTarget.Y ? -0.125m : 0);
            if (State == GhostState.IntoHouse)
            {
                Move(dy != 0 ? 0 : dx, dy);
            }
            else
            {
                Move(dx, dx != 0 ? 0 : dy);
            }
        }

        public void KeepInBounds(decimal width, decimal height)
        {
            if (Location.IsOutOfBounds(width, height, out var dx, out var dy))
            {
                Location = Location.Add(dx, dy);
                NextTarget = NextTarget.Add(dx, dy);
                CurrentTarget = CurrentTarget.Add(dx, dy);
            }
        }

        public void ChangeState(GhostState state)
        {
            State = state;
        }

        public void ChangeDirection()
        {
            CurrentDirection = NextDirection;
            CurrentTarget = NextTarget;
        }

        public void SetNextDirection(Direction direction, Location target)
        {
            NextDirection = direction;
            NextTarget = target;
        }

        public void FlipDirection()
        {
            NextDirection = NextDirection.Opposite();
            NextTarget = NextTarget.Move(NextDirection);
        }

        public bool IsAtTarget => Location.X == CurrentTarget.X && Location.Y == CurrentTarget.Y;
        
        public Location GetChaseTarget(PacMan pacMan, Location blinky)
        {
            var pacCell = pacMan.Location.Cell;
            blinky = blinky.Cell;
            switch (Colour)
            {
                case GhostColour.Red:
                    return pacCell;
                case GhostColour.Pink:
                    return pacCell
                        .Move(pacMan.Direction)
                        .Move(pacMan.Direction)
                        .Move(pacMan.Direction)
                        .Move(pacMan.Direction);
                case GhostColour.Cyan:
                    var dx = pacCell.X - blinky.X;
                    var dy = pacCell.Y - blinky.Y;
                    return blinky.Add(2 * dx, 2 * dy);
                case GhostColour.Orange:
                    return Location.Cell.DistanceTo(pacCell) < 64 
                        ? ScatterTarget 
                        : pacCell;
                default:
                    return Location.Cell;
            }
        }
        
        public void SetToLeave(Location exitGhostHouse)
        {
            State = GhostState.LeaveHouse;
            CurrentTarget = exitGhostHouse;
            NextDirection = Direction.Up;
        }

        public void SetFrightened()
        {
            if (State == GhostState.Alive || State == GhostState.InHouse || State == GhostState.LeaveHouse)
            {
                FlashAnimation.Stop();
                Frightened = true;
                FlipDirection();
            }
        }

        public void SetNotFrightened()
        {
            Frightened = false;
        }

        public void SetFrightenedFlash()
        {
            FlashAnimation.Reset();
        }

        public void SetEaten(PointsMultiplier points)
        {
            State = GhostState.Eaten;
            Frightened = false;
            ShowAsPoints = points;
        }

        public void SetEyes()
        {
            State = GhostState.Eyes;
        }

        public void SetAlive()
        {
            State = GhostState.Alive;
            NextDirection = Direction.Left;
            NextTarget = new Location(Location.X, Location.CellY).Move(Direction.Left);
            ChangeDirection();
        }

        public void SetTargetToGhostDoor(Location ghostDoor)
        {
            State = GhostState.GhostDoor;
            CurrentTarget = ghostDoor;
            NextDirection = Direction.Down;
        }

        public void SetToIntoHouse()
        {
            State = GhostState.IntoHouse;
            CurrentTarget = HomeLocation;
            NextDirection = Direction.Down;
        }

        public void SetInHouse()
        {
            State = GhostState.InHouse;
        }

        public void JiggleInHouse()
        {
            if (Location.Y > HomeLocation.Y)
            {
                CurrentTarget = HomeLocation.Add(0, -0.5m);
                NextDirection = Direction.Up;
            }
            else
            {
                CurrentTarget = HomeLocation.Add(0, 0.5m);
                NextDirection = Direction.Down;
            }
        }

        public bool IsSlowMo => Frightened
                                || State == GhostState.InHouse
                                || State == GhostState.LeaveHouse;

        public bool IsForcedMovement => State == GhostState.InHouse
                                        || State == GhostState.GhostDoor
                                        || State == GhostState.LeaveHouse
                                        || State == GhostState.IntoHouse;

        public bool IsEyesMode => State == GhostState.Eyes
                                  || State == GhostState.GhostDoor
                                  || State == GhostState.IntoHouse;
    }
}