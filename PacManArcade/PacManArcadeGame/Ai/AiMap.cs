using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PacManArcadeGame.Helpers;
using PacManArcadeGame.Map;

namespace PacManArcadeGame.Ai
{
    public class AiMap
    {
        public readonly int Height;
        public readonly int Width;

        private readonly AiMapCell[,] _cells;

        public AiMap(Map.Map map)
        {
            Height = map.Height;
            Width = map.Width;

            _cells = new AiMapCell[map.Width, map.Height];

            // Copy the map into the AI map

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    _cells[x, y] = new AiMapCell(this, x, y, map.Cell(x, y));
                }
            }

            // Add the possible moves from every cell

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    _cells[x, y].UpdatePossibleDirections();
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
                        if (cell.IsPlayArea)
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
        /// Calculate the distance to every play square
        /// </summary>
        public void CalcDistance(int x, int y)
        {
            foreach (var c in _cells)
            {
                c.Reset();
            }

            var queue = new Queue<DistanceQueueItem>();

            queue.Enqueue(new DistanceQueueItem(Cell(x,y), 0));

            while (queue.Count > 0)
            {
                var next = queue.Dequeue();
                if (!next.Cell.Visited || next.Distance < next.Cell.Distance)
                {
                    next.Cell.Distance = next.Distance;
                    next.Cell.Visited = true;
                    foreach (var possible in next.Cell.LinkedCells.Values)
                    {
                        queue.Enqueue(new DistanceQueueItem(possible, next.Distance + 1));
                    }
                }
            }
        }

        public AiMapCell ClosestPill()
        {
            var distance = int.MaxValue;
            AiMapCell closest = null;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var cell = _cells[x, y];
                    var ct = cell.MapCellDetail.CellType;
                    if ((ct == CellType.Pill || ct == CellType.ThroughSpacePill) && cell.Distance <= distance)
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
            while (true)
            {
                var lowest = Cell(fx, fy)
                    .LinkedCells.OrderBy(kp => kp.Value.Distance)
                    .First();
                var cell = lowest.Value;

                // Are we there yet?
                // Directions are opposite as we're working backwards

                if (cell.X == tx && cell.Y == ty) return lowest.Key.Opposite();

                // No, carry on

                fx = cell.X;
                fy = cell.Y;
            }
        }

        public AiMapCell Cell(int x, int y) => _cells[x < 0 ? x + Width : x % Width, y < 0 ? y + Height : y % Height];
    }
}