using System;

namespace PacManArcadeGame
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

    }
}
