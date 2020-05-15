namespace PacManArcadeGame.Map
{
    /// <summary>
    /// A pattern of 9 cells around a target cell that correspond to a particular map piece
    /// </summary>
    public class CellPattern
    {
        public readonly MapDisplayPiece MapDisplayPiece;
        private readonly string[] _pattern;

        public CellPattern(string pattern, MapDisplayPiece mapDisplayPiece)
        {
            MapDisplayPiece = mapDisplayPiece;
            _pattern = new string[3];
            for (int y = 0; y < 3; y++)
            {
                _pattern[y] = pattern.Substring(y * 3, 3);
            }
        }

        private char PatternCell(int x, int y) => _pattern[y + 1][x + 1];

        /// <summary>
        /// Check the nine squares around the given cell for matching with pattern
        /// </summary>
        /// <param name="middleOfNine"></param>
        /// <returns></returns>
        public bool DoCellsMatchPattern(MapCellDetail middleOfNine)
        {
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    var onBoard = middleOfNine.Cell(x, y);
                    var compare = PatternCell(x, y);

                    if (compare == 'G' && onBoard.CellType != CellType.GhostWall) return false;

                    if (compare == 'D' && onBoard.CellType != CellType.Door) return false;

                    if (compare == 'X' && onBoard.CellType != CellType.SingleWall) return false;

                    if (compare == 'W' && onBoard.CellType != CellType.SingleWall
                                       && onBoard.CellType != CellType.DoubleWall) return false;

                    if (compare == '0' && !onBoard.IsPlayArea) return false;

                    if (compare == '#' && onBoard.CellType != CellType.DoubleWall) return false;

                    if (compare == '~' && onBoard.CellType != CellType.DeadSpace) return false;
                }
            }

            return true;
        }
    }
}
