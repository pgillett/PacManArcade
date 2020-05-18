using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.GameSetup
{
    public class DemoMove
    {
        public readonly Location Location;
        public readonly Direction Direction;

        public DemoMove(int x, int y, Direction direction)
        {
            Location = new Location(x, y);
            Direction = direction;
        }
    }
}