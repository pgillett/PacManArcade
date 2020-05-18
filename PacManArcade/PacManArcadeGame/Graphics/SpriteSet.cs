using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PacManArcadeGame.GameItems;
using PacManArcadeGame.Helpers;
using PacManArcadeGame.Map;

namespace PacManArcadeGame.Graphics
{
    public class SpriteSet
    {
        private readonly Dictionary<MapDisplayPiece, SpriteSource> _blueMapPieces;
        private readonly Dictionary<MapDisplayPiece, SpriteSource> _whiteMapPieces;

        private readonly Dictionary<TextColour, Dictionary<char, SpriteSource>> _characters =
            new Dictionary<TextColour, Dictionary<char, SpriteSource>>();

        public readonly SpriteSource Pill;
        public readonly SpriteSource PowerPill;

        public readonly SpriteSource Blank;

        public readonly ReadOnlyCollection<SpriteSource> TestBox;
        public readonly IReadOnlyDictionary<Fruit, SpriteSource> Fruit;

        public readonly IReadOnlyDictionary<Fruit, ReadOnlyCollection<SpriteSource>> BonusScores;

        public SpriteSet()
        {
            var fruit = new Dictionary<Fruit, SpriteSource>();
            for (int i = 0; i < 8; i++)
                fruit[(Fruit)i]=new SpriteSource(i * 2, 10, 2);

            Fruit = fruit;

            Blank = new SpriteSource(0, 6, 1);
            Pill = new SpriteSource(16, 0, 1);
            PowerPill = new SpriteSource(20, 0, 1);

            _blueMapPieces = MapSet(0);
            _whiteMapPieces = MapSet(26);

            _characters[TextColour.White] = CharacterColour(1);
            _characters[TextColour.Red] = CharacterColour(24);
            _characters[TextColour.Pink] = CharacterColour(26);
            _characters[TextColour.Cyan] = CharacterColour(28);
            _characters[TextColour.Orange] = CharacterColour(30);
            _characters[TextColour.Peach] = CharacterColour(34);
            _characters[TextColour.Yellow] = CharacterColour(36);

            TestBox = new List<SpriteSource>
            {
                new SpriteSource(30, 1, 1),
                new SpriteSource(28, 1, 1),
                new SpriteSource(31, 1, 1),
                new SpriteSource(29, 1, 1),
            }.AsReadOnly();

            BonusScores = new Dictionary<Fruit, ReadOnlyCollection<SpriteSource>>
            {
                [GameItems.Fruit.Cherry100] = BonusSet(1, 5),
                [GameItems.Fruit.Strawberry300] = BonusSet(2, 5),
                [GameItems.Fruit.Orange500] = BonusSet(3, 5),
                [GameItems.Fruit.Bell700] = BonusSet(4, 5),
                [GameItems.Fruit.Apple1000] = BonusSet(6, 13, 14),
                [GameItems.Fruit.Grapes2000] = BonusSet(7, 8, 13, 14),
                [GameItems.Fruit.Arcadian3000] = BonusSet(9, 10, 13, 14),
                [GameItems.Fruit.Key5000] = BonusSet(11, 12, 13, 14)
            };
        }

        private Dictionary<MapDisplayPiece, SpriteSource> MapSet(int yOff)
        {
            var pieces = new Dictionary<MapDisplayPiece, SpriteSource>
            {
                {MapDisplayPiece.Blank, Blank},
                {MapDisplayPiece.Pill, new SpriteSource(16, 0, 1)},
                {MapDisplayPiece.PowerAnim1, new SpriteSource(18, 0, 1)},
                {MapDisplayPiece.PowerAnim2, new SpriteSource(20, 0, 1)},
                {MapDisplayPiece.DoubleTopRight, new SpriteSource(16, 6 + yOff, 1)},
                {MapDisplayPiece.DoubleTopLeft, new SpriteSource(17, 6 + yOff, 1)},
                {MapDisplayPiece.DoubleRight, new SpriteSource(18, 6 + yOff, 1)},
                {MapDisplayPiece.DoubleLeft, new SpriteSource(19, 6 + yOff, 1)},
                {MapDisplayPiece.DoubleBottomRight, new SpriteSource(20, 6 + yOff, 1)},
                {MapDisplayPiece.DoubleBottomLeft, new SpriteSource(21, 6 + yOff, 1)},
                {MapDisplayPiece.JoinRightHandTop, new SpriteSource(22, 6 + yOff, 1)},
                {MapDisplayPiece.JoinLeftHandTop, new SpriteSource(23, 6 + yOff, 1)},
                {MapDisplayPiece.JoinRightHandBottom, new SpriteSource(24, 6 + yOff, 1)},
                {MapDisplayPiece.JoinLeftHandBottom, new SpriteSource(25, 6 + yOff, 1)},
                {MapDisplayPiece.DoubleTop, new SpriteSource(26, 6 + yOff, 1)},
                {MapDisplayPiece.DoubleBottom, new SpriteSource(28, 6 + yOff, 1)},
                {MapDisplayPiece.Top, new SpriteSource(30, 6 + yOff, 1)},
                {MapDisplayPiece.Bottom, new SpriteSource(4, 7 + yOff, 1)},
                {MapDisplayPiece.TopRight, new SpriteSource(6, 7 + yOff, 1)},
                {MapDisplayPiece.TopLeft, new SpriteSource(7, 7 + yOff, 1)},
                {MapDisplayPiece.Right, new SpriteSource(8, 7 + yOff, 1)},
                {MapDisplayPiece.Left, new SpriteSource(9, 7 + yOff, 1)},
                {MapDisplayPiece.BottomRight, new SpriteSource(10, 7 + yOff, 1)},
                {MapDisplayPiece.BottomLeft, new SpriteSource(11, 7 + yOff, 1)},
                {MapDisplayPiece.GhostTopRight, new SpriteSource(12, 7 + yOff, 1)},
                {MapDisplayPiece.GhostTopLeft, new SpriteSource(13, 7 + yOff, 1)},
                {MapDisplayPiece.GhostBottomRight, new SpriteSource(14, 7 + yOff, 1)},
                {MapDisplayPiece.GhostBottomLeft, new SpriteSource(15, 7 + yOff, 1)},
                {MapDisplayPiece.GhostEndRight, new SpriteSource(16, 7 + yOff, 1)},
                {MapDisplayPiece.GhostEndLeft, new SpriteSource(17, 7 + yOff, 1)},
                {MapDisplayPiece.JoinTopRight, new SpriteSource(26, 7 + yOff, 1)},
                {MapDisplayPiece.JoinTopLeft, new SpriteSource(27, 7 + yOff, 1)},
                {MapDisplayPiece.GhostDoor, new SpriteSource(15, 6 + yOff, 1)},
                {MapDisplayPiece.InnerTopRight, new SpriteSource(19, 7 + yOff, 1)},
                {MapDisplayPiece.InnerTopLeft, new SpriteSource(18, 7 + yOff, 1)},
                {MapDisplayPiece.InnerBottomRight, new SpriteSource(21, 7 + yOff, 1)},
                {MapDisplayPiece.InnerBottomLeft, new SpriteSource(20, 7 + yOff, 1)},
                {MapDisplayPiece.InsideWalls, new SpriteSource(1, 6 + yOff, 1)}
            };
            return pieces;
        }

        private ReadOnlyCollection<SpriteSource> BonusSet(params int[] xs)
        {
            var set = xs.Select(x => new SpriteSource(x, 4, 1)).ToList();

            return set.AsReadOnly();
        }

        private Dictionary<char, SpriteSource> CharacterColour(int yAdd)
        {
            var chars = new Dictionary<char, SpriteSource>();
            for (var c = 'A'; c <= 'Z'; c++)
                chars[c] = new SpriteSource(c - 64, 1 + yAdd, 1);
            for (var c = '0'; c <= '9'; c++)
                chars[c] = new SpriteSource(c - 32, yAdd, 1);
            chars['!'] = new SpriteSource(27, 1 + yAdd, 1);
            chars['c'] = new SpriteSource(28, 1 + yAdd, 1);
            chars['p'] = new SpriteSource(29, 1 + yAdd, 1);
            chars['t'] = new SpriteSource(30, 1 + yAdd, 1);
            chars['s'] = new SpriteSource(31, 1 + yAdd, 1);
            chars['/'] = new SpriteSource(26, yAdd, 1);
            chars['-'] = new SpriteSource(27, yAdd, 1);
            chars['"'] = new SpriteSource(6, yAdd, 1);
            chars['.'] = new SpriteSource(5, yAdd, 1);
            return chars;
        }

        public SpriteSource Ghost(Ghost ghost)
        {
            switch (ghost.State)
            {
                case GhostState.Hidden:
                    return Blank;
                case GhostState.Eaten:
                    return GhostPoints(ghost.ShowAsPoints);
                case GhostState.Eyes:
                case GhostState.GhostDoor:
                case GhostState.IntoHouse:
                    return Ghost(GhostColour.Eyes, ghost.NextDirection, ghost.Animation.IsZero);
                case GhostState.Alive:
                case GhostState.LeaveHouse:
                case GhostState.InHouse:
                    if (ghost.Frightened)
                    {
                        return Ghost(ghost.FlashAnimation.IsZero ? GhostColour.BlueFlash : GhostColour.WhiteFlash,
                            Direction.Right, ghost.Animation.IsZero);
                    }

                    return Ghost(ghost.Colour, ghost.NextDirection, ghost.Animation.IsZero);
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

            xpos += direction switch
            {
                Direction.Up => 12,
                Direction.Down => 4,
                Direction.Left => 8,
                Direction.Right => 0,
                _ => throw new Exception("Direction?")
            };

            if (animated)
                xpos += 2;

            return new SpriteSource(xpos, ypos, 2);
        }

        /// <summary>
        /// Points for eating ghosts 200, 400, 800, 1600
        /// </summary>
        /// <param name="multiplier">0-3</param>
        /// <returns></returns>
        public SpriteSource GhostPoints(PointsMultiplier multiplier)
        {
            return new SpriteSource(16 + (int)multiplier * 2, 14, 2);
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
            return _blueMapPieces[MapDisplayPiece.Blank];
        }

        public SpriteSource PacMan(PacMan pacMan)
        {
            if (pacMan.Animation.Active)
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
                return new SpriteSource(8 + animation * 2, 16, 2);
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

        public SpriteSource Map(MapDisplayPiece piece, bool white)
        {
            if (white)
            {
                if (_whiteMapPieces.ContainsKey(piece)) return _whiteMapPieces[piece];
                return Blank;
            }
            if (_blueMapPieces.ContainsKey(piece)) return _blueMapPieces[piece];
            return Blank;
        }
    }
}
