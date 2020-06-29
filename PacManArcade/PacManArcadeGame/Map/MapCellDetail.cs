using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.Map
{
    /// <summary>
    /// Holds the detail of one cell of the map
    /// </summary>
    public class MapCellDetail
    {
        public readonly int Y;
        public readonly int X;
        public CellType CellType { get; private set; }
        public MapDisplayPiece Piece;
        public bool PillEaten { get; private set; }
        public bool IsThroughSpace { get; private set; }

        private readonly Map _map;
        
        public MapCellDetail(Map map, int x, int y, CellType cellType, bool isThroughSpace, MapDisplayPiece mapDisplayPiece)
        {
            _map = map;
            Y = y;
            X = x;
            CellType = cellType;
            Piece = mapDisplayPiece;
            IsThroughSpace = isThroughSpace;
        }

        public MapCellDetail CellAbove => Cell(0, -1);
        public MapCellDetail CellBelow => Cell(0, 1);
        public MapCellDetail CellLeft => Cell(-1, 0);
        public MapCellDetail CellRight => Cell(1, 0);
        public MapCellDetail CellTopLeft => Cell(-1, -1);
        public MapCellDetail CellTopRight => Cell(1, -1);
        public MapCellDetail CellBottomLeft => Cell(-1, 1);
        public MapCellDetail CellBottomRight => Cell(1, 1);

        public MapCellDetail InDirection(Direction direction) => _map.Cell(new Location(X, Y).Move(direction));

        public MapCellDetail Cell(int directionColumn, int directionRow) =>
            _map.Cell(X + directionColumn, Y + directionRow);

        public bool IsPlayArea => CellType == CellType.Pill
                                  || CellType == CellType.PowerPill
                                  || CellType == CellType.Tunnel
                                  || CellType == CellType.PlayArea;

        public bool IsGhostEyeArea => IsPlayArea
                                      || CellType == CellType.Door
                                      || CellType == CellType.DeadSpace;

        public void RemovePill()
        {
            PillEaten = true;
            Piece = MapDisplayPiece.Blank;
        }

        public void ResetEaten()
        {
            if (CellType == CellType.Pill)
            {
                Piece = MapDisplayPiece.Pill;
                PillEaten = false;
            }
            if (CellType == CellType.PowerPill)
            {
                PillEaten = false;
            }
        }

        public Location Location => new Location(X, Y);
    }
}
