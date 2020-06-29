using System;
using System.Collections.Generic;
using System.Linq;
using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.Map
{
    public class Map
    {
        public readonly int Height;
        public readonly int Width;

        private readonly MapCellDetail[,] _map;

        private int _initialPills;
        private IReadOnlyList<Location> _initialPowerPills;

        public int Pills;
        public List<Location> PowerPills { get; private set; }

        public void Reset()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Cell(x, y).ResetEaten();
                }
            }

            Pills = _initialPills;
            PowerPills = _initialPowerPills.ToList();
        }

        public Map(string board)
        {
            if (!board.Contains(c13))
            {
                board = board.Replace(c10, c13);
            }

            board = board.Replace(c10.ToString(), "");

            var lines = board.Split(c13);
            Height = lines.Length;
            Width = lines.Max(l => l.Length);

            _map = new MapCellDetail[Width, Height];

            _initialPills = 0;
            _initialPowerPills = new List<Location>();

            MakeBasicMap(lines);
            MakeDetailedMap();
        }

        private const char c13 = (char) 13;
        private const char c10 = (char) 10;


        private void MakeBasicMap(string[] lines)
        {
            var powerPills = new List<Location>();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var c = lines[y][x];

                    CellType piece;
                    Boolean throughSpace = false;
                    switch (c)
                    {
                        case '.':
                            piece = CellType.Pill;
                            _initialPills++;
                            break;
                        case ',':
                            throughSpace = true;
                            piece = CellType.Pill;
                            _initialPills++;
                            break;
                        case '*':
                            piece = CellType.PowerPill;
                            powerPills.Add(new Location(x, y));
                            break;
                        case '=':
                            throughSpace = true;
                            piece = CellType.PlayArea;
                            break;
                        default:
                            piece = c switch
                            {
                                ' ' => CellType.PlayArea,
                                'X' => CellType.SingleWall,
                                '#' => CellType.DoubleWall,
                                '+' => CellType.DeadSpace,
                                'G' => CellType.GhostWall,
                                '-' => CellType.Door,
                                'T' => CellType.Tunnel,
                                _ => throw new NotImplementedException()
                            };
                            break;
                    }

                    _map[x, y] = new MapCellDetail(this, x, y, piece, throughSpace, MapDisplayPiece.Blank);
                }
            }

            _initialPowerPills = powerPills;
        }

        private void MakeDetailedMap()
        {
            var cellPatternFinder = new CellPatternFinder();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var cell = _map[x, y];
                    switch (cell.CellType)
                    {
                        case CellType.PlayArea:
                        case CellType.DeadSpace:
                        case CellType.Tunnel:
                            cell.Piece = MapDisplayPiece.Blank;
                            break;
                        case CellType.Door:
                            cell.Piece = MapDisplayPiece.GhostDoor;
                            break;
                        case CellType.Pill:
                            cell.Piece = MapDisplayPiece.Pill;
                            break;
                        case CellType.PowerPill:
                            cell.Piece = MapDisplayPiece.Blank;
                            break;
                        case CellType.GhostWall:
                        case CellType.SingleWall:
                        case CellType.DoubleWall:
                            cell.Piece = cellPatternFinder.FindBoardPiece(cell);
                            break;
                        default:
                            cell.Piece = MapDisplayPiece.Blank;
                            break;
                    }
                }
            }
        }

        public MapCellDetail Cell(int x, int y)
        {
            var xok = x >= 0 && x < Width;
            var yok = y >= 0 && y < Height;
            if (xok && yok) return _map[x, y];
            var src = _map[xok ? x : 0, yok ? y : 0];
            return new MapCellDetail(this, x, y, src.CellType, false, src.Piece);
        }

        public MapCellDetail Cell(Location location) => Cell(location.CellX, location.CellY);

        public void RemovePowerPill(Location location)
        {
            PowerPills.Remove(location);
        }

#if DEBUG

        // These are tools to help with debugging a map design
        // Can check for basic map and detailed map with all pieces to see all has parsed correctly

        /// <summary>
        /// To help with debugging basic map
        /// </summary>
        public string[] ToMapStringBasic => ConvertMapToStringArray(cell => (int) cell.CellType);

        /// <summary>
        /// To help with debugging detailed map
        /// </summary>
        public string[] ToMapStringPieces => ConvertMapToStringArray(cell => (int) cell.Piece);

        /// <summary>
        /// Produces a copy of the processed map as a string array
        /// To help with debugging map parsing
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private string[] ConvertMapToStringArray(Func<MapCellDetail, int> func)
        {
            var map = new string[Height];
            for (int y = 0; y < Height; y++)
            {
                var st = "";
                for (int x = 0; x < Width; x++)
                {
                    st = st + (char) ('A' + func(_map[x, y]));
                }

                map[y] = st;
            }

            return map;
        }
#endif
    }
}
