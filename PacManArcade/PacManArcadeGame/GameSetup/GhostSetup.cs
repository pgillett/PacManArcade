using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.GameSetup
{
    public class GhostSetup
    {
        private readonly GhostColour _colour;
        private readonly Location _initialLocation;
        private readonly Direction _initialDirection;
        private readonly Location _scatterTarget;
        private readonly Location _homeLocation;

        public GhostSetup(GhostColour colour, Location initialLocation, Direction initialDirection,
            Location scatterTarget, Location homeLocation)
        {
            _colour = colour;
            _initialLocation = initialLocation;
            _initialDirection = initialDirection;
            _scatterTarget = scatterTarget;
            _homeLocation = homeLocation;
        }

        public Ghost Ghost => new Ghost(_colour, _initialLocation, _initialDirection, _scatterTarget, _homeLocation);
    }
}