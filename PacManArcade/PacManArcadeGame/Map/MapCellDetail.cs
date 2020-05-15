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

        private readonly Map _map;
        
        public MapCellDetail(Map map, int x, int y, CellType cellType, MapDisplayPiece mapDisplayPiece)
        {
            _map = map;
            Y = y;
            X = x;
            CellType = cellType;
            Piece = mapDisplayPiece;
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
                                  || CellType == CellType.ThroughSpace
                                  || CellType == CellType.ThroughSpacePill
                                  || CellType == CellType.Tunnel
                                  || CellType == CellType.PlayArea;

        public bool IsGhostEyeArea => IsPlayArea
                                      || CellType == CellType.Door
                                      || CellType == CellType.DeadSpace;

        public bool IsThrough => CellType == CellType.ThroughSpace
                                 || CellType == CellType.ThroughSpacePill;

        public void RemovePill()
        {
            if (CellType == CellType.Pill || CellType == CellType.PowerPill)
                CellType = CellType.PlayArea;
            if (CellType == CellType.ThroughSpacePill)
                CellType = CellType.ThroughSpace;
            Piece = MapDisplayPiece.Blank;
        }

        public Location Location => new Location(X, Y);
    }
}
