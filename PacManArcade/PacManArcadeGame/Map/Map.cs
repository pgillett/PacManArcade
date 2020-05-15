﻿using System;
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

        public int Pills;
        public List<Location> PowerPills;

        private readonly MapCellDetail _empty;

        private Map()
        {
            _empty = new MapCellDetail(this, -1, -1, CellType.DeadSpace, MapDisplayPiece.Blank)
                {Piece = MapDisplayPiece.Blank};
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
                    _map[x, y] = new MapCellDetail(this, cell.X, cell.Y, cell.CellType, cell.Piece);
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

                    CellType piece;
                    if (c == '.')
                    {
                        piece = CellType.Pill;
                        Pills++;
                    }
                    else if (c == ',')
                    {
                        piece = CellType.ThroughSpacePill;
                        Pills++;
                    }
                    else if (c == '*')
                    {
                        piece = CellType.PowerPill;
                        PowerPills.Add(new Location(x, y));
                    }
                    else
                    {
                        piece = c switch
                        {
                            ' ' => CellType.PlayArea,
                            'X' => CellType.SingleWall,
                            '#' => CellType.DoubleWall,
                            '+' => CellType.DeadSpace,
                            'G' => CellType.GhostWall,
                            '-' => CellType.Door,
                            'T' => CellType.Tunnel,
                            '=' => CellType.ThroughSpace,
                            _ => throw new NotImplementedException()
                        };
                    }

                    _map[x, y] = new MapCellDetail(this, x, y, piece, MapDisplayPiece.Blank);
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
                    switch (cell.CellType)
                    {
                        case CellType.PlayArea:
                        case CellType.DeadSpace:
                        case CellType.ThroughSpace:
                        case CellType.Tunnel:
                            cell.Piece = MapDisplayPiece.Blank;
                            break;
                        case CellType.Door:
                            cell.Piece = MapDisplayPiece.GhostDoor;
                            break;
                        case CellType.Pill:
                        case CellType.ThroughSpacePill:
                            cell.Piece = MapDisplayPiece.Pill;
                            break;
                        case CellType.PowerPill:
                            cell.Piece = MapDisplayPiece.Blank;
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