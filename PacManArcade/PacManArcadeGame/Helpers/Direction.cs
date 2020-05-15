using System.Collections.Generic;

namespace PacManArcadeGame.Helpers
{
    public enum Direction
    {
        Up,
        Left,
        Down,
        Right,
    }

    public static class DirectionExtensions
    {
        private static Dictionary<Direction, Direction> _opposite = new Dictionary<Direction, Direction>
        {
            {Direction.Up, Direction.Down},
            {Direction.Down, Direction.Up},
            {Direction.Left, Direction.Right},
            {Direction.Right, Direction.Left}
        };

        public static Direction Opposite(this Direction direction) => _opposite[direction];
    }
}
