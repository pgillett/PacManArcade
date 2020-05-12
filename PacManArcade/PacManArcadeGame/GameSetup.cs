using System;
using System.Collections.Generic;
using System.Text;

namespace PacManArcadeGame
{
    public class GameSetup
    {
        public GhostSetup InitalBlinky => new GhostSetup(GhostColour.Red, new Location(13.5m, 11), Direction.Left,
            new Location(27, -2));
        public GhostSetup InitalPinky => new GhostSetup(GhostColour.Pink, new Location(13.5m, 14), Direction.Down,
            new Location(1, -2));
        public GhostSetup InitalInky => new GhostSetup(GhostColour.Cyan, new Location(11.5m, 14), Direction.Up,
            new Location(30, 31));
        public GhostSetup IntialClyde => new GhostSetup(GhostColour.Orange, new Location(15.5m, 14), Direction.Up,
            new Location(0, 31));

        public PacMacSetup InitialPacMan => new PacMacSetup(new Location(13.5m, 23), Direction.Left);

        public Location GhostHouse => new Location(13.5m, 14);

        private readonly Map _map;

        public GameSetup()
        {
            var board = System.IO.File.ReadAllText("board1.txt");
            _map = new Map(board);
        }

        public Map Map => _map.Copy();

        public int MapHeight => _map.Height;
        public int MapWidth => _map.Width;
    }

    public class GhostSetup
    {
        public GhostColour Colour;
        public Location InitialLocation;
        public Direction InitialDirection;
        public Location ScatterTarget;

        public GhostSetup(GhostColour colour, Location initialLocation, Direction initialDirection,
            Location scatterTarget)
        {
            Colour = colour;
            InitialLocation = initialLocation;
            InitialDirection = initialDirection;
            ScatterTarget = scatterTarget;
        }

        public Ghost Ghost => new Ghost(Colour, InitialLocation, InitialDirection, ScatterTarget);
    }

    public class PacMacSetup
    {
        public Location InitialLocation;
        public Direction InitialDirection;

        public PacMacSetup(Location initialLocation, Direction initialDirection)
        {
            InitialLocation = initialLocation;
            InitialDirection = initialDirection;
        }

        public PacMan PacMan => new PacMan(InitialLocation, InitialDirection);
    }
}
