using PacManArcadeGame.Helpers;
using PacManArcadeGame.Map;

namespace PacManArcadeGame.Ai
{
    public class AiMap
    {
        public readonly int Height;
        public readonly int Width;

        private readonly AiMapCell[,] Cells;

        public AiMap(Map.Map map)
        {
            Height = map.Height;
            Width = map.Width;

            Cells = new AiMapCell[map.Width, map.Height];

            // Copy the map into the AI map

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    Cells[x, y] = new AiMapCell(this, x, y, map.Cell(x, y));
                }
            }
        }

        /// <summary>
        /// Text version for debugging
        /// </summary>
        public string[] DebugMap
        {
            get
            {
                var str = new string[Height];
                for (int y = 0; y < Height; y++)
                {
                    var row = "";
                    for (int x = 0; x < Width; x++)
                    {
                        var cell = Cell(x, y);
                        if (cell.PlayArea)
                        {
                            row = $"{row}{(cell.Distance < 99 ? cell.Distance.ToString("D2") : "99")} ";
                        }
                        else
                        {
                            row = $"{row}###";
                        }
                    }

                    str[y] = row;
                }

                return str;
            }
        }

        /// <summary>
        /// Reset the map ready for next distance check
        /// </summary>
        public void CalcDistance(int x, int y)
        {
            foreach (var c in Cells)
            {
                c.Reset();
            }

            CalcNextDistance(Cell(x, y), 0);
        }

        public AiMapCell ClosestPill()
        {
            var distance = int.MaxValue;
            AiMapCell closest = null;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var cell = Cells[x, y];
                    var ct = cell.MapCellDetail.CellType;
                    if ((ct==CellType.Pill || ct==CellType.ThroughSpacePill) && cell.Distance <= distance)
                    {
                        closest = cell;
                        distance = cell.Distance;
                    }
                }
            }

            return closest;
        }

        public Direction WorkBackTo(int fx, int fy, int tx, int ty)
        {
            // Directions are opposite as we're working backwards

            var direction = Direction.Down;
            var cell = Cell(fx, fy - 1);

            {
                var cell2 = Cell(fx, fy + 1);
                if (cell2.Score < cell.Score)
                {
                    cell = cell2;
                    direction = Direction.Up;
                }
            }
            {
                var cell2 = Cell(fx-1, fy);
                if (cell2.Score < cell.Score)
                {
                    cell = cell2;
                    direction = Direction.Right;
                }
            }
            {
                var cell2 = Cell(fx+1, fy);
                if (cell2.Score < cell.Score)
                {
                    cell = cell2;
                    direction = Direction.Left;
                }
            }

            if (cell.X==tx && cell.Y==ty) return direction;

            // If not there yet, carry on

            return WorkBackTo(cell.X, cell.Y, tx, ty);
        }

        private void CalcNextDistance(AiMapCell cell, int distance)
        {
            if (cell.Visited)
            {
                if (distance > cell.Distance)
                {
                    return;
                }
            }

            cell.Distance = distance;
            cell.Visited = true;

            if (cell.Above.PlayArea) CalcNextDistance(cell.Above, distance + 1);
            if (cell.Below.PlayArea) CalcNextDistance(cell.Below, distance + 1);
            if (cell.Left.PlayArea) CalcNextDistance(cell.Left, distance + 1);
            if (cell.Right.PlayArea) CalcNextDistance(cell.Right, distance + 1);
        }

        public AiMapCell Cell(int x, int y) => Cells[x < 0 ? x + Width : x % Width, y < 0 ? y + Height : y % Height];
    }
}