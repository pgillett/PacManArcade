using System;
using System.Collections.Generic;

namespace PacManArcadeGame.Map
{
    /// <summary>
    /// Checks for patterns in the given cell (and it's surrounding cells) and sets the detailed map piece
    /// </summary>
    public class CellPatternFinder
    {
        private readonly List<CellPattern> _patterns;

        public CellPatternFinder()
        {
            _patterns = new List<CellPattern>
            {
                new CellPattern("-0-GGD---", MapDisplayPiece.GhostEndLeft),
                new CellPattern("-0-DGG---", MapDisplayPiece.GhostEndRight),
                new CellPattern("-0-GGG---", MapDisplayPiece.DoubleBottom),
                new CellPattern("---GGG-0-", MapDisplayPiece.DoubleTop),
                new CellPattern("-G-0G--G-", MapDisplayPiece.DoubleRight),
                new CellPattern("-G--G0-G-", MapDisplayPiece.DoubleLeft),
                new CellPattern("-0-0GG-G-", MapDisplayPiece.GhostTopLeft),
                new CellPattern("-0-GG0-G-", MapDisplayPiece.GhostTopRight),
                new CellPattern("-G-0GG-0-", MapDisplayPiece.GhostBottomLeft),
                new CellPattern("-G-GG0-0-", MapDisplayPiece.GhostBottomRight),
                new CellPattern("-0-WXW---", MapDisplayPiece.Top),
                new CellPattern("---WXW-0-", MapDisplayPiece.Bottom),
                new CellPattern("-W-0X--W-", MapDisplayPiece.Left),
                new CellPattern("-W--X0-W-", MapDisplayPiece.Right),
                new CellPattern("-0-WX0---", MapDisplayPiece.TopRight),
                new CellPattern("-0-0XW---", MapDisplayPiece.TopLeft),
                new CellPattern("---WX0-0-", MapDisplayPiece.BottomRight),
                new CellPattern("---0XW-0-", MapDisplayPiece.BottomLeft),
                new CellPattern("-W0-XW---", MapDisplayPiece.InnerBottomLeft),
                new CellPattern("0W-WX----", MapDisplayPiece.InnerBottomRight),
                new CellPattern("----XW-W0", MapDisplayPiece.InnerTopLeft),
                new CellPattern("---WX-0W-", MapDisplayPiece.InnerTopRight),
                new CellPattern("-0-###---", MapDisplayPiece.DoubleBottom),
                new CellPattern("---###-0-", MapDisplayPiece.DoubleTop),
                new CellPattern("-#-0#--#-", MapDisplayPiece.DoubleRight),
                new CellPattern("-#--#0-#-", MapDisplayPiece.DoubleLeft),
                new CellPattern("-~-~##-0-", MapDisplayPiece.DoubleTop),
                new CellPattern("-~-##~-0-", MapDisplayPiece.DoubleTop),
                new CellPattern("-0-~##-~-", MapDisplayPiece.DoubleBottom),
                new CellPattern("-0-##~-~-", MapDisplayPiece.DoubleBottom),
                new CellPattern("-00##0-#0", MapDisplayPiece.TopRight),
                new CellPattern("00-0##0#-", MapDisplayPiece.TopLeft),
                new CellPattern("-#0##0-00", MapDisplayPiece.BottomRight),
                new CellPattern("0#-0##00-", MapDisplayPiece.BottomLeft),
                new CellPattern("-0-##0---", MapDisplayPiece.DoubleTopRight),
                new CellPattern("-0-0##---", MapDisplayPiece.DoubleTopLeft),
                new CellPattern("---##0-0-", MapDisplayPiece.DoubleBottomRight),
                new CellPattern("---0##-0-", MapDisplayPiece.DoubleBottomLeft),
                new CellPattern("-#0-##---", MapDisplayPiece.DoubleBottomLeft),
                new CellPattern("0#-##----", MapDisplayPiece.DoubleBottomRight),
                new CellPattern("----##-#0", MapDisplayPiece.DoubleTopLeft),
                new CellPattern("---##-0#-", MapDisplayPiece.DoubleTopRight),
                new CellPattern("----##-X0", MapDisplayPiece.JoinTopRight),
                new CellPattern("---##-0X-", MapDisplayPiece.JoinTopLeft),
                new CellPattern("-#0-#X---", MapDisplayPiece.JoinLeftHandTop),
                new CellPattern("0#-X#----", MapDisplayPiece.JoinRightHandTop),
                new CellPattern("----#X-#0", MapDisplayPiece.JoinLeftHandBottom),
                new CellPattern("---X#-0#-", MapDisplayPiece.JoinRightHandBottom)
            };

            // Not in the original map
            // new CellPattern("-X0-##---", MapDisplayPiece.JoinBottomRight));
            // new CellPattern("0X-##----", MapDisplayPiece.JoinBottomLeft));
        }

        public MapDisplayPiece FindBoardPiece(MapCellDetail board)
        {
            foreach (var pattern in _patterns)
            {
                if (pattern.DoCellsMatchPattern(board))
                    return pattern.MapDisplayPiece;
            }

#if DEBUG
            // Failed to match - loop through again to help debug layout

            foreach (var pattern in _patterns)
            {
                if (pattern.DoCellsMatchPattern(board))
                    return pattern.MapDisplayPiece;
            }
#endif

            throw new Exception($"No matching board piece pattern @ c{board.X},{board.Y}");
        }
    }
}
