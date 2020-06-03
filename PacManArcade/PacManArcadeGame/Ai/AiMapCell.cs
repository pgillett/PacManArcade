using System.Collections.Generic;
using System.Linq;
using PacManArcadeGame.Ai;
using PacManArcadeGame.Helpers;
using PacManArcadeGame.Map;

namespace PacManArcadeGame.Ai
{
    public class AiMapCell
    {
        private readonly AiMap _map;

        public readonly int X;
        public readonly int Y;
        public bool Visited;
        public int Distance;

        public bool IsPlayArea => MapCellDetail.IsPlayArea;

        public readonly MapCellDetail MapCellDetail;

        public Dictionary<Direction, AiMapCell> LinkedCells;

        public bool IsJunction => LinkedCells.Count > 2;

        public AiMapCell(AiMap map, int x, int y, MapCellDetail mapCellDetail)
        {
            _map = map;
            X = x;
            Y = y;
            MapCellDetail = mapCellDetail;
            Reset();
        }

        public void UpdatePossibleDirections()
        {
            LinkedCells = new[] {Direction.Up, Direction.Down, Direction.Left, Direction.Right}
                .Where(d => CellInDirection(d).IsPlayArea)
                .ToDictionary(d => d, CellInDirection);

        }

        public void Reset()
        {
            Visited = false;
            Distance = int.MaxValue;
        }
        
        public AiMapCell CellInDirection(Direction direction) => _map.Cell(X + direction.Dx(), Y + direction.Dy());
    }
}
