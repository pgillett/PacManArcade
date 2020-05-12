namespace PacManArcadeGame
{
    /// <summary>
    /// Holds the detail of one cell of the map
    /// </summary>
    public class MapCellDetail
    {
        public readonly int Y;
        public readonly int X;
        public readonly BasicMapPiece BasicType;
        public BoardPiece Piece;

        private readonly Map _map;


        public MapCellDetail(Map map, int x, int y, BasicMapPiece basicType, BoardPiece boardPiece)
        {
            _map = map;
            Y = y;
            X = x;
            BasicType = basicType;
            Piece = boardPiece;
        }

        public MapCellDetail CellAbove => Cell(0, -1);
        public MapCellDetail CellBelow => Cell(0, 1);
        public MapCellDetail CellLeft => Cell(-1, 0);
        public MapCellDetail CellRight => Cell(1, 0);
        public MapCellDetail CellTopLeft => Cell(-1, -1);
        public MapCellDetail CellTopRight => Cell(1, -1);
        public MapCellDetail CellBottomLeft => Cell(-1, 1);
        public MapCellDetail CellBottomRight => Cell(1, 1);

        public MapCellDetail Cell(int directionColumn, int directionRow) =>
            _map.Cell(X + directionColumn, Y + directionRow);
    }
}
