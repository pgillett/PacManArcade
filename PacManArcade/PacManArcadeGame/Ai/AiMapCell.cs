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

        public bool PlayArea => MapCellDetail.IsPlayArea;

        public readonly MapCellDetail MapCellDetail;

        public AiMapCell(AiMap map, int x, int y, MapCellDetail mapCellDetail)
        {
            _map = map;
            X = x;
            Y = y;
            MapCellDetail = mapCellDetail;
            Reset();
        }

        public void Reset()
        {
            Visited = false;
            Distance = int.MaxValue;
        }

        public int Score => PlayArea ? Distance : int.MaxValue;

        public AiMapCell Above => _map.Cell(X, Y - 1);
        public AiMapCell Below => _map.Cell(X, Y + 1);
        public AiMapCell Left => _map.Cell(X - 1, Y);
        public AiMapCell Right => _map.Cell(X + 1, Y);
    }
}