using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacManArcadeGame.GameItems;
using PacManArcadeGame.Helpers;
using PacManArcadeGame.Map;

namespace PacManArcadeGame.Ai
{
    public class AiBot
    {
        public AiMap AiMap;
        public int Counter;
        public Direction Last;
        public int LastX;
        public int LastY;

        public AiBot(Map.Map map)
        {
            AiMap = new AiMap(map);
        }

        public Direction BestMove(Location location, Direction currentDirection, IEnumerable<Ghost> ghosts)
        {
            if (LastX == location.CellX && LastY == location.CellY)
                return Last;

            if (Counter > 0)
            {
                Counter--;
                return Last;
            }

            Counter = 3;

            LastX = location.CellX;
            LastY = location.CellY;

            AiMap.Clear();
            var x = Math.Min(Math.Max(0, location.CellX), AiMap.Width - 1);
            var y = Math.Min(Math.Max(0, location.CellY), AiMap.Height - 1);
            AiMap.CalcDistances(AiMap.Cell(x, y), 0);

            var cell = AiMap.ClosestPill();
            var frightenedDistances = ghosts.Where(g=>g.Frightened)
                .Select(g => new GhostDistance(g, AiMap.Cell(g.Location.CellX, g.Location.CellY).Distance))
                .OrderBy(gd => gd.Distance).ToList();
            if (frightenedDistances.Count > 0 && frightenedDistances[0].Distance < 10)
                cell = AiMap.Cell(frightenedDistances[0].Ghost.Location.CellX,
                    frightenedDistances[0].Ghost.Location.CellY);

            var moves = new List<Direction>();

            var pacMan = AiMap.Cell(location.CellX, location.CellY);
            if (pacMan.Above.PlayArea)
                moves.Add(Direction.Up);
            if (pacMan.Below.PlayArea)
                moves.Add(Direction.Down);
            if (pacMan.Left.PlayArea )
                moves.Add(Direction.Left);
            if (pacMan.Right.PlayArea)
                moves.Add(Direction.Right);

            //if (moves.Count > 1)
            //    moves.Remove(currentDirection.Opposite());

            var pillDirection = AiMap.WorkBackTo(cell.X, cell.Y, x, y);

            var ghostDistances = ghosts.Where(AvoidGhost)
                .Select(g => new GhostDistance(g, AiMap.Cell(g.Location.CellX, g.Location.CellY).Distance))
                .OrderBy(gd => gd.Distance);
            foreach (var ghostDistance in ghostDistances.Where(g=>g.Distance<7))
            {
                var ghostDirection = AiMap.WorkBackTo(ghostDistance.Ghost.Location.CellX, ghostDistance.Ghost.Location.CellY, x, y);
                if (moves.Contains(ghostDirection) && moves.Count > 1)
                {
                    moves.Remove(ghostDirection);
                }
            }

            Counter = 5;

            if (moves.Contains(pillDirection))
            {
                Last = pillDirection;
                return pillDirection;
            }

            

            Last = moves[0];
            return moves[0];
        }

        public bool AvoidGhost(Ghost ghost)
        {
            return !ghost.Frightened &&
                   (ghost.State == GhostState.Alive
                    || ghost.State == GhostState.GhostDoor
                    || ghost.State == GhostState.LeaveHouse);
        }
    }

    public class GhostDistance
    {
        public Ghost Ghost;
        public int Distance;

        public GhostDistance(Ghost ghost, int distance)
        {
            Ghost = ghost;
            Distance = distance;
        }
    }

    public class AiMap
    {
        public int Height;
        public int Width;

        public AiMapCell[,] Cells;

        public AiMap(Map.Map map)
        {
            Height = map.Height;
            Width = map.Width;

            Cells=new AiMapCell[map.Width,map.Height];
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    Cells[x, y] = new AiMapCell(this, x, y, map.Cell(x, y));
                }
            }
        }

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

        public void Clear()
        {
            foreach (var cell in Cells)
            {
                cell.Reset();
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
            return WorkBackTo(cell.X, cell.Y, tx, ty);
        }

        public void CalcDistances(AiMapCell cell, int distance)
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

            if (cell.Above.PlayArea) CalcDistances(cell.Above, distance + 1);
            if (cell.Below.PlayArea) CalcDistances(cell.Below, distance + 1);
            if (cell.Left.PlayArea) CalcDistances(cell.Left, distance + 1);
            if (cell.Right.PlayArea) CalcDistances(cell.Right, distance + 1);
        }

        public AiMapCell Cell(int x, int y) => Cells[x < 0 ? x + Width : x % Width, y < 0 ? y + Height : y % Height];
    }

    public class AiMapCell
    {
        public AiMap Map;
        public bool Visited;
        public int Distance;
        public int X;
        public int Y;

        public bool PlayArea => MapCellDetail.IsPlayArea;

        public MapCellDetail MapCellDetail;

        public AiMapCell(AiMap map, int x, int y, MapCellDetail mapCellDetail)
        {
            Map = map;
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

        public AiMapCell Above => Map.Cell(X, Y - 1);
        public AiMapCell Below => Map.Cell(X, Y + 1);
        public AiMapCell Left => Map.Cell(X - 1, Y);
        public AiMapCell Right => Map.Cell(X + 1, Y);
    }
}
