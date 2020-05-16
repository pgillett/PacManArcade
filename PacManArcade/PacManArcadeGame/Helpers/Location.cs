using System;

namespace PacManArcadeGame.Helpers
{
    public readonly struct Location
    {
        public readonly decimal X;
        public readonly decimal Y;

        public int CellX => (int) Math.Round(X, MidpointRounding.AwayFromZero);
        public int CellY => (int) Math.Round(Y, MidpointRounding.AwayFromZero);

        public Location(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }

        public Location Add(decimal x, decimal y) => new Location(X + x, Y + y);

        public Location Cell => new Location(CellX, CellY);

        public bool CloseToCell
        {
            get
            {
                var dx = Math.Abs(CellX - X);
                var dy = Math.Abs(CellY - Y);
                return dx <= 0.375m && dy <= 0.375m;
            }
        }

        public Location Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new Location(X, Y - 1);
                case Direction.Down:
                    return new Location(X, Y + 1);
                case Direction.Left:
                    return new Location(X - 1, Y);
                default:
                    return new Location(X + 1, Y);
            }
        }

        public int DistanceTo(Location target)
        {
            var dx = target.X - X;
            var dy = target.Y - Y;
            return (int) (dx * dx + dy * dy);
        }

        public bool IsNearTo(Location location, decimal distance = 0.5m) => Math.Abs(X - location.X) + Math.Abs(Y - location.Y) < distance;

        public bool IsSameCell(Location location) => CellX == location.CellX && CellY == location.CellY;

        public bool IsOutOfBounds(decimal x, decimal y, out decimal dx, out decimal dy)
        {
            dx = 0;
            dy = 0;
            if (X < -BoundLimit) dx = (x + BoundLimit * 2);
            if (X > x + BoundLimit) dx = -(x + BoundLimit * 2);
            if (Y < -BoundLimit) dy = (y + BoundLimit * 2);
            if (Y > y + BoundLimit) dy = -(y + BoundLimit * 2);

            return dx != 0 || dy != 0;
        }

        private const decimal BoundLimit = 1.5m;
    }
}
