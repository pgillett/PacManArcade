using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.GameSetup
{
    public class GameSetup
    {
        public GhostSetup InitialBlinky => new GhostSetup(GhostColour.Red, new Location(13.5m, 11), Direction.Left,
            new Location(25, -2), new Location(13.5m,14), false);
        public GhostSetup InitialPinky => new GhostSetup(GhostColour.Pink, new Location(13.5m, 14), Direction.Down,
            new Location(2, -2), new Location(13.5m, 14), true);
        public GhostSetup InitialInky => new GhostSetup(GhostColour.Cyan, new Location(11.5m, 14), Direction.Up,
            new Location(27, 31), new Location(11.5m, 14), true);
        public GhostSetup InitialClyde => new GhostSetup(GhostColour.Orange, new Location(15.5m, 14), Direction.Up,
            new Location(0, 31), new Location(15.5m,14), true);

        public PacMacSetup InitialPacMan => new PacMacSetup(new Location(13.5m, 23), Direction.Left);

        public Location ExitGhostHouse => new Location(13.5m, 11);

        private readonly Map.Map _map;

        public GameSetup()
        {
            var board = System.IO.File.ReadAllText("board.txt");
            _map = new Map.Map(board);
        }

        public Map.Map Map => _map.Copy();

        public int MapHeight => _map.Height;
        public int MapWidth => _map.Width;
    }
}
