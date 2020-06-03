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

        public static int Dx(this Direction direction) => 
            direction switch
            {
                Direction.Left => -1,
                Direction.Right => 1,
                _ => 0
            };

        public static int Dy(this Direction direction) =>
            direction switch
            {
                Direction.Up => -1,
                Direction.Down => 1,
                _ => 0
            };
    }
}
