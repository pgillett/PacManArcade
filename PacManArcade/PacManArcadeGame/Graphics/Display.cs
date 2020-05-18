using System.Collections.Generic;
using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.Graphics
{
    public class Display
    {
        public readonly int Height;
        public readonly int Width;

        private readonly SpriteSource[,] _screenMap;

        private readonly SpriteSet _spriteSet;

        private List<SpriteDisplay> _sprites;

        public IEnumerable<SpriteDisplay> Sprites => _sprites;

        public Display(int height, int width, SpriteSet spriteSet)
        {
            Height = height;
            Width = width;
            _screenMap = new SpriteSource[width, height];
            _spriteSet = spriteSet;
        }

        public void ClearSprites()
        {
            _sprites = new List<SpriteDisplay>();
        }

        public void AddSprite(SpriteSource sprite, Location location)
        {
            _sprites.Add(new SpriteDisplay(location, sprite));
        }

        public void Blank()
        {
            Fill(_spriteSet.Blank);
        }

        public void Fill(SpriteSource source)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _screenMap[x, y] = source;
                }
            }
        }

        public void Update(SpriteSource sprite, int x, int y)
        {
            _screenMap[x, y] = sprite;
        }

        public void WriteLine(string text, TextColour colour, int x, int y)
        {
            foreach (var c in text)
            {
                Update(_spriteSet.Character(colour, c), x, y);
                x++;
            }
        }

        public SpriteSource Get(int x, int y) => (x >= 0 && x < Width && y >= 0 && y < Height) ? _screenMap[x, y] : null;
    }
}
