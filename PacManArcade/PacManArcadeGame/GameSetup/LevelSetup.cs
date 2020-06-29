using System.Collections.Generic;
using System.Collections.ObjectModel;
using PacManArcadeGame.GameItems;
using PacManArcadeGame.Helpers;
using PacManArcadeGame.Properties;

namespace PacManArcadeGame.GameSetup
{
    public class LevelSetup
    {
        public Ghost InitialBlinky => new Ghost(GhostColour.Red, new Location(13.5m, 11), Direction.Left,
            new Location(25, -2), new Location(13.5m,14), false);
        public Ghost InitialPinky => new Ghost(GhostColour.Pink, new Location(13.5m, 14), Direction.Down,
            new Location(2, -2), new Location(13.5m, 14), true);
        public Ghost InitialInky => new Ghost(GhostColour.Cyan, new Location(11.5m, 14), Direction.Up,
            new Location(27, 31), new Location(11.5m, 14), true);
        public Ghost InitialClyde => new Ghost(GhostColour.Orange, new Location(15.5m, 14), Direction.Up,
            new Location(0, 31), new Location(15.5m,14), true);

        public PacMan InitialPacMan => new PacMan(new Location(13.5m, 23), Direction.Left);

        public Location ExitGhostHouse => new Location(13.5m, 11);

        public Location FruitLocation => new Location(13.5m, 17);

        public readonly Map.Map Map;

        public LevelSetup()
        {
            var board = Resources.Board;
            Map = new Map.Map(board);
        }

        public int MapHeight => Map.Height;
        public int MapWidth => Map.Width;

        
    }
}
