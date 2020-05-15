using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.GameSetup
{
    public class PacMacSetup
    {
        private readonly Location _initialLocation;
        private readonly Direction _initialDirection;

        public PacMacSetup(Location initialLocation, Direction initialDirection)
        {
            _initialLocation = initialLocation;
            _initialDirection = initialDirection;
        }

        public PacMan PacMan => new PacMan(_initialLocation, _initialDirection);
    }
}