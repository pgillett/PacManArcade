using System.Collections.Generic;
using System.Collections.ObjectModel;
using PacManArcadeGame.GameItems;
using PacManArcadeGame.Helpers;

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

        private readonly Map.Map _map;

        public LevelSetup()
        {
            var board = System.IO.File.ReadAllText("board.txt");
            _map = new Map.Map(board);
        }

        public Map.Map Map => _map.Copy();

        public int MapHeight => _map.Height;
        public int MapWidth => _map.Width;

        public readonly ReadOnlyCollection<DemoMove> DemoMoves =

           new List<DemoMove>
           {
                new DemoMove(9, 23, Direction.Left),
                new DemoMove(9, 26, Direction.Down),
                new DemoMove(12, 26, Direction.Right),
                new DemoMove(12, 29, Direction.Down),
                new DemoMove(26, 29, Direction.Right),
                new DemoMove(26, 26, Direction.Up),
                new DemoMove(24, 26, Direction.Left),
                new DemoMove(24, 23, Direction.Up),
                new DemoMove(26, 23, Direction.Right),
                new DemoMove(26, 20, Direction.Up),
                new DemoMove(18, 20, Direction.Left),
                new DemoMove(18, 11, Direction.Up),
                new DemoMove(9, 11, Direction.Left),
                new DemoMove(9, 14, Direction.Down),
                new DemoMove(6, 14, Direction.Left),
                new DemoMove(6, 1, Direction.Up),
                new DemoMove(1, 1, Direction.Left),
                new DemoMove(1, 5, Direction.Down),
                new DemoMove(12, 5, Direction.Right),
                new DemoMove(12, 1, Direction.Up),
                new DemoMove(6, 1, Direction.Left),
                new DemoMove(6, 5, Direction.Down),
                new DemoMove(9,5, Direction.Right),
                new DemoMove(9,8, Direction.Down),
                new DemoMove(12, 8, Direction.Right),
                new DemoMove(12, 11, Direction.Down),
                new DemoMove(9,11, Direction.Left),
                new DemoMove(9, 14, Direction.Down),
                new DemoMove(1, 14, Direction.Left),
                new DemoMove(18, 14, Direction.Left),
                new DemoMove(18,20, Direction.Down),
                new DemoMove(21,20,Direction.Right),
                new DemoMove(21, 23, Direction.Down),
                new DemoMove(18,23, Direction.Left),
                new DemoMove(18,26, Direction.Down),
                new DemoMove(15,26, Direction.Left),
                new DemoMove(15,29, Direction.Down),
                new DemoMove(1, 29, Direction.Left),
                new DemoMove(1, 26, Direction.Up),
                new DemoMove(3, 26, Direction.Right),
                new DemoMove(3, 23, Direction.Up),
                new DemoMove(1, 23, Direction.Left),
                new DemoMove(1, 20, Direction.Up),
                new DemoMove(6,20, Direction.Right),
                new DemoMove(6,23,Direction.Down),
                new DemoMove(15, 23, Direction.Right),
                new DemoMove(15, 20, Direction.Up),
                new DemoMove(21, 20, Direction.Right),
                new DemoMove(21, 26, Direction.Down),
                new DemoMove(26,26, Direction.Right),
                new DemoMove(26, 29, Direction.Down),
                new DemoMove(12, 29, Direction.Left),
                new DemoMove(12, 26, Direction.Up),
                new DemoMove(9,26, Direction.Left),
                new DemoMove(9,23,Direction.Up),
                new DemoMove(6,23, Direction.Left),
                new DemoMove(6,26,Direction.Down),
                new DemoMove(1,26, Direction.Left),
                new DemoMove(1,29, Direction.Down),
                new DemoMove(3, 26, Direction.Right),
                new DemoMove(3, 23, Direction.Up),
                new DemoMove(1, 23, Direction.Left),
                new DemoMove(1, 20, Direction.Up),
                new DemoMove(6, 20, Direction.Right),
           }.AsReadOnly();
    }
}
