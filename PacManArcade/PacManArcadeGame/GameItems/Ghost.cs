using System;
using System.Security.Cryptography.X509Certificates;
using PacManArcadeGame.Graphics;
using PacManArcadeGame.Helpers;

namespace PacManArcadeGame
{
    public class Ghost
    {
        public Location ScatterTarget;

        public GhostColour Colour { get; private set; }
        public Location Location { get; private set; }
        public Direction Direction { get; private set; }
        public Location Target { get; private set; }
        public Animation Animation { get; private set; }
        public GhostState State { get; private set; }
        public Location CurrentTarget { get; private set; }
        public Direction CurrentDirection { get; private set; }
        public int SkipTickEvery { get; private set; }
        public int AsPoints { get; private set; }
        public Location HomeLocation { get; private set; }
        public Animation FlashAnimation { get; private set; }
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
            Direction = direction;
            ScatterTarget = scatterTarget;
            Animation = new Animation(2, 10);
            FlashAnimation = new Animation(2, 7, true);
            FlashAnimation.Stop();
            State = startInHouse ? GhostState.InHouse : GhostState.Alive;
            HomeLocation = homeLocation;
            Target = Location.Cell;
            ChangeDirection();
            Target = Location.Cell.Move(CurrentDirection);
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

        public void Bounds(decimal x, decimal y)
        {
            if (Location.IsOutOfBounds(x, y, out var dx, out var dy))
            {
                Location = Location.Add(dx, dy);
                Target = Target.Add(dx, dy);
                CurrentTarget = CurrentTarget.Add(dx, dy);
            }
        }

        public void ChangeState(GhostState state)
        {
            State = state;
        }

        public void ChangeDirection()
        {
            CurrentDirection = Direction;
            CurrentTarget = Target;
        }

        public void ChangeNextDirection(Direction direction, Location target)
        {
            Direction = direction;
            Target = target;
        }

        public void FlipDirection()
        {
            Direction = Direction.Opposite();
            Target = Target.Move(Direction);
        }

        public bool IsAtTarget => Location.X == CurrentTarget.X && Location.Y == CurrentTarget.Y;

        public bool IsOnNextTargetCell => Location.CellX == Target.X
                                      && Location.CellY == Target.Y;

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

        public void ForceTo(Location target)
        {
            Target = new Location(target.X, Location.Y);
            Direction = Direction.Up;
            ChangeDirection();
        }

        public void SetToLeave(Location exitGhostHouse)
        {
            State = GhostState.LeaveHouse;
            CurrentTarget = exitGhostHouse;
            Direction = Direction.Up;
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

        public void SetEaten(int points)
        {
            State = GhostState.Eaten;
            Frightened = false;
            AsPoints = points;
        }

        public void SetEyes()
        {
            State = GhostState.Eyes;
        }

        public void SetAlive()
        {
            State = GhostState.Alive;
            Direction = Direction.Left;
            Target = new Location(Location.X, Location.CellY).Move(Direction.Left);
            ChangeDirection();
        }

        public void SetToGhostDoor(Location ghostDoor)
        {
            State = GhostState.GhostDoor;
            CurrentTarget = ghostDoor;
            Direction = Direction.Down;
        }

        public void SetToIntoHouse()
        {
            State = GhostState.IntoHouse;
            CurrentTarget = HomeLocation;
            Direction = Direction.Down;
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
                Direction = Direction.Up;
            }
            else
            {
                CurrentTarget = HomeLocation.Add(0, 0.5m);
                Direction = Direction.Down;
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