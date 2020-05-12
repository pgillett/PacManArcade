using System;
using System.Collections.Generic;

namespace PacManArcadeGame
{
    public class Sprites
    {
        public Sprites()
        {
            Bonus = new SpriteSource[8];
            for (int i = 0; i < 8; i++)
                Bonus[i] = new SpriteSource(i * 2, 10, 2);

            Blank = new SpriteSource(0, 6, 1);

            _mapPieces.Add(BoardPiece.Blank, Blank);
            _mapPieces.Add(BoardPiece.Pill, new SpriteSource(16, 0, 1));
            _mapPieces.Add(BoardPiece.PowerAnim1, new SpriteSource(18, 0, 1));
            _mapPieces.Add(BoardPiece.PowerAnim2, new SpriteSource(20, 0, 1));
            _mapPieces.Add(BoardPiece.DoubleTopRight, new SpriteSource(16, 6, 1));
            _mapPieces.Add(BoardPiece.DoubleTopLeft, new SpriteSource(17, 6, 1));
            _mapPieces.Add(BoardPiece.DoubleRight, new SpriteSource(18, 6, 1));
            _mapPieces.Add(BoardPiece.DoubleLeft, new SpriteSource(19, 6, 1));
            _mapPieces.Add(BoardPiece.DoubleBottomRight, new SpriteSource(20, 6, 1));
            _mapPieces.Add(BoardPiece.DoubleBottomLeft, new SpriteSource(21, 6, 1));
            _mapPieces.Add(BoardPiece.JoinRightHandTop, new SpriteSource(22, 6, 1));
            _mapPieces.Add(BoardPiece.JoinLeftHandTop, new SpriteSource(23, 6, 1));
            _mapPieces.Add(BoardPiece.JoinRightHandBottom, new SpriteSource(24, 6, 1));
            _mapPieces.Add(BoardPiece.JoinLeftHandBottom, new SpriteSource(25, 6, 1));
            _mapPieces.Add(BoardPiece.DoubleTop, new SpriteSource(26, 6, 1));
            _mapPieces.Add(BoardPiece.DoubleBottom, new SpriteSource(28, 6, 1));
            _mapPieces.Add(BoardPiece.Top, new SpriteSource(30, 6, 1));
            _mapPieces.Add(BoardPiece.Bottom, new SpriteSource(4, 7, 1));
            _mapPieces.Add(BoardPiece.TopRight, new SpriteSource(6, 7, 1));
            _mapPieces.Add(BoardPiece.TopLeft, new SpriteSource(7, 7, 1));
            _mapPieces.Add(BoardPiece.Right, new SpriteSource(8, 7, 1));
            _mapPieces.Add(BoardPiece.Left, new SpriteSource(9, 7, 1));
            _mapPieces.Add(BoardPiece.BottomRight, new SpriteSource(10, 7, 1));
            _mapPieces.Add(BoardPiece.BottomLeft, new SpriteSource(11, 7, 1));
            _mapPieces.Add(BoardPiece.GhostTopRight, new SpriteSource(12, 7, 1));
            _mapPieces.Add(BoardPiece.GhostTopLeft, new SpriteSource(13, 7, 1));
            _mapPieces.Add(BoardPiece.GhostBottomRight, new SpriteSource(14, 7, 1));
            _mapPieces.Add(BoardPiece.GhostBottomLeft, new SpriteSource(15, 7, 1));
            _mapPieces.Add(BoardPiece.GhostEndRight, new SpriteSource(16, 7, 1));
            _mapPieces.Add(BoardPiece.GhostEndLeft, new SpriteSource(17, 7, 1));
            _mapPieces.Add(BoardPiece.JoinTopRight, new SpriteSource(26, 7, 1));
            _mapPieces.Add(BoardPiece.JoinTopLeft, new SpriteSource(27, 7, 1));
            _mapPieces.Add(BoardPiece.GhostDoor, new SpriteSource(15, 6, 1));
            _mapPieces.Add(BoardPiece.InnerTopRight, new SpriteSource(19, 7, 1));
            _mapPieces.Add(BoardPiece.InnerTopLeft, new SpriteSource(18, 7, 1));
            _mapPieces.Add(BoardPiece.InnerBottomRight, new SpriteSource(21, 7, 1));
            _mapPieces.Add(BoardPiece.InnerBottomLeft, new SpriteSource(20, 7, 1));
            _mapPieces.Add(BoardPiece.InsideWalls, new SpriteSource(1, 6, 1));

            _characters[TextColour.White] = CharacterColour(1);
            _characters[TextColour.Red] = CharacterColour(24);
            _characters[TextColour.Pink] = CharacterColour(26);
            _characters[TextColour.Cyan] = CharacterColour(28);
            _characters[TextColour.Orange] = CharacterColour(30);

            Pill = new SpriteSource(16, 0, 1);

            TestBox=new SpriteSource[]
            {
                new SpriteSource(30, 1, 1), 
                new SpriteSource(28, 1, 1),
                new SpriteSource(31,1,1), 
                new SpriteSource(29,1,1), 
            };
        }

        private Dictionary<char, SpriteSource> CharacterColour(int yAdd)
        {
            var chars = new Dictionary<char, SpriteSource>();
        for (var c = 'A'; c <= 'Z'; c++)
                chars[c] = new SpriteSource(c - 64, 1+yAdd, 1);
            for (var c = '0'; c <= '9'; c++)
                chars[c] = new SpriteSource(c - 32, yAdd, 1);
            chars['!'] = new SpriteSource(27, 1 + yAdd, 1);
            chars['c'] = new SpriteSource(28, 1 + yAdd, 1);
            chars['p'] = new SpriteSource(29, 1 + yAdd, 1);
            chars['t'] = new SpriteSource(30, 1 + yAdd, 1);
            chars['/'] = new SpriteSource(26, yAdd, 1);
            chars['-'] = new SpriteSource(27, yAdd, 1);
            chars['"'] = new SpriteSource(6, yAdd, 1);
            chars['.'] = new SpriteSource(5, yAdd, 1);
            return chars;
        }

        private Dictionary<TextColour, Dictionary<char, SpriteSource>> _characters = new Dictionary<TextColour, Dictionary<char, SpriteSource>>();

        public SpriteSource[] TestBox;

        public SpriteSource PowerPill(bool animated)
        {
            return new SpriteSource(animated ? 22 : 20, 0, 1);
        }

        public SpriteSource Pill;

        public SpriteSource[] Bonus;

        public SpriteSource Blank;

        public SpriteSource Ghost(Ghost ghost)
        {
            switch (ghost.State)
            {
                case GhostState.Dead:
                    return Blank;
                case GhostState.Eyes:
                    return Ghost(GhostColour.Eyes, ghost.Direction, ghost.Animation.Current == 0);
                case GhostState.Frightened:
                    return Ghost(GhostColour.BlueFlash, ghost.Direction, ghost.Animation.Current == 0);
                case GhostState.Alive:
                    return Ghost(ghost.Colour, ghost.Direction, ghost.Animation.Current == 0);
                default:
                    return Blank;
            }
        }

        /// <summary>
        /// Get source of ghost sprite
        /// </summary>
        /// <param name="ghostColour"></param>
        /// <param name="direction"></param>
        /// <param name="animated"></param>
        /// <returns></returns>
        public SpriteSource Ghost(GhostColour ghostColour, Direction direction, bool animated)
        {
            int xpos;
            int ypos;
            switch (ghostColour)
            {
                case GhostColour.Red:
                    xpos = 0;
                    ypos = 14;
                    break;
                case GhostColour.Cyan:
                    xpos = 16;
                    ypos = 18;
                    break;
                case GhostColour.Pink:
                    xpos = 0;
                    ypos = 20;
                    break;
                case GhostColour.Orange:
                    xpos = 16;
                    ypos = 20;
                    break;
                case GhostColour.Eyes:
                    xpos = 0;
                    ypos = 22;
                    break;
                case GhostColour.BlueFlash:
                    xpos = 24;
                    ypos = 12;
                    break;
                case GhostColour.WhiteFlash:
                    xpos = 28;
                    ypos = 12;
                    break;
                default:
                    throw new Exception("GhostColour?");
            }

            switch (direction)
            {
                case Direction.Up:
                    xpos += 12;
                    break;
                case Direction.Down:
                    xpos += 4;
                    break;
                case Direction.Left:
                    xpos += 8;
                    break;
                case Direction.Right:
                    break;
                default:
                    throw new Exception("Direction?");
            }

            if (animated)
                xpos += 2;

            return new SpriteSource(xpos, ypos, 2);

        }

        /// <summary>
        /// Points for eating ghosts 200, 400, 800, 1600
        /// </summary>
        /// <param name="multiplier">0-3</param>
        /// <returns></returns>
        public SpriteSource GhostPoints(int multiplier)
        {
            return new SpriteSource(16 + multiplier * 2, 14, 2);
        }

        /// <summary>
        /// Text character upper case letters numbers etc.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public SpriteSource Character(TextColour colour, char character)
        {
            if (_characters.ContainsKey(colour))
                if (_characters[colour].ContainsKey(character))
                    return _characters[colour][character];
            return _mapPieces[BoardPiece.Blank];
        }

        public SpriteSource PacMan(PacMan pacMan)
        {
            if(pacMan.Animation.Active)
                return PacMan(pacMan.Direction, pacMan.Animation.Current, pacMan.Dying);
            return Blank;
        }

        /// <summary>
            /// Get source of Pacmac sprite
            /// </summary>
            /// <param name="direction"></param>
            /// <param name="animation">0-3 moving, 0-11 dying</param>
            /// <param name="dying"></param>
            /// <returns></returns>
            public SpriteSource PacMan(Direction direction, int animation, bool dying)
        {
            if (dying)
            {
                return new SpriteSource(8 + animation - 4 * 2, 16, 2);
            }

            if (animation == 0)
            {
                return new SpriteSource(0, 16, 2);
            }

            int xpos;
            int ypos = 18;
            switch (direction)
            {
                case Direction.Right:
                    xpos = 0;
                    break;
                case Direction.Down:
                    xpos = 2;
                    break;
                case Direction.Left:
                    xpos = 6;
                    break;
                case Direction.Up:
                    xpos = 4;
                    break;
                default:
                    throw new Exception("Direction?");
            }

            if (animation == 1 || animation == 3)
                xpos += 8;

            return new SpriteSource(xpos, ypos, 2);
        }

        private Dictionary<BoardPiece, SpriteSource> _mapPieces = new Dictionary<BoardPiece, SpriteSource>();

        public SpriteSource Map(BoardPiece piece)
        {
            if (_mapPieces.ContainsKey(piece)) return _mapPieces[piece];
            return Blank;
        }
        
    }
}
