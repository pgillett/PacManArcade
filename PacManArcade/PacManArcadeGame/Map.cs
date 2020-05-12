using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PacManArcadeGame
{
    public class Map
    {
        public int Height;
        public int Width;

        private readonly MapCellDetail[,] _map;

        public int Pills;
        public List<Location> PowerPills;

        private readonly MapCellDetail _empty;

        private Map()
        {
            _empty = new MapCellDetail(this, -1, -1, BasicMapPiece.DeadSpace, BoardPiece.Blank)
                {Piece = BoardPiece.Blank};
        }

        private Map(Map map) : this()
        {
            Height = map.Height;
            Width = map.Width;

            _map = new MapCellDetail[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var cell = map.Cell(x, y);
                    _map[x, y] = new MapCellDetail(this, cell.X, cell.Y, cell.BasicType, cell.Piece);
                }
            }

            Pills = map.Pills;
            PowerPills = map.PowerPills.ToList();
        }

        public Map Copy()
        {
            return new Map(this);
        }

        public Map(string board) : this()
        {
            var lines = board.Split(Environment.NewLine);
            Height = lines.Length;
            Width = lines.Max(l => l.Length);

            _map = new MapCellDetail[Width, Height];

            Pills = 0;
            PowerPills = new List<Location>();

            MakeBasicMap(lines);
            MakeDetailedMap();
        }

        private void MakeBasicMap(string[] lines)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var c = lines[y][x];

                    BasicMapPiece piece;
                    if (c == '.')
                    {
                        piece = BasicMapPiece.Pill;
                        Pills++;
                    }
                    else if (c == ',')
                    {
                        piece = BasicMapPiece.ThroughSpacePill;
                        Pills++;
                    }
                    else if (c == '*')
                    {
                        piece = BasicMapPiece.PlayArea;
                        PowerPills.Add(new Location(x, y));
                    }
                    else
                    {
                        piece = c switch
                        {
                            ' ' => BasicMapPiece.PlayArea,
                            'X' => BasicMapPiece.SingleWall,
                            '#' => BasicMapPiece.DoubleWall,
                            '+' => BasicMapPiece.DeadSpace,
                            'G' => BasicMapPiece.GhostWall,
                            '-' => BasicMapPiece.Door,
                            'T' => BasicMapPiece.Tunnel,
                            '=' => BasicMapPiece.ThroughSpace,
                            _ => throw new NotImplementedException()
                        };
                    }

                    _map[x, y] = new MapCellDetail(this, x, y, piece, BoardPiece.Blank);
                }
            }
        }

        private void MakeDetailedMap()
        {
            var cellPatternFinder = new CellPatternFinder();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var cell = _map[x, y];
                    switch (cell.BasicType)
                    {
                        case BasicMapPiece.PlayArea:
                        case BasicMapPiece.DeadSpace:
                        case BasicMapPiece.ThroughSpace:
                        case BasicMapPiece.Tunnel:
                            cell.Piece = BoardPiece.Blank;
                            break;
                        case BasicMapPiece.Door:
                            cell.Piece = BoardPiece.GhostDoor;
                            break;
                        case BasicMapPiece.Pill:
                        case BasicMapPiece.ThroughSpacePill:
                            cell.Piece = BoardPiece.Pill;
                            break;
                        default:
                            cell.Piece = cellPatternFinder.FindBoardPiece(cell);
                            break;
                    }
                }
            }
        }


        public MapCellDetail Cell(int x, int y) =>
            x >= 0 && x < Width && y >= 0 && y < Height ? _map[x, y] : _empty;

#if DEBUG

        // These are tools to help with debugging a map design
        // Can check for basic map and detailed map with all pieces to see all has parsed correctly

        /// <summary>
        /// To help with debugging basic map
        /// </summary>
        public string[] ToMapStringBasic => ConvertMapToStringArray(cell => (int) cell.BasicType);

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
