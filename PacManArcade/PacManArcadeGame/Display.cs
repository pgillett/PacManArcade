using System.Collections.Generic;

namespace PacManArcadeGame
{
    public class Display
    {
        public int Height;
        public int Width;

        private readonly SpriteSource[,] _screenMap;

        public Sprites SpriteMap;

        public List<SpriteDisplay> Sprites;

        public Display(int height, int width, Sprites spriteMap)
        {
            Height = height;
            Width = width;
            _screenMap = new SpriteSource[width, height];
            SpriteMap = spriteMap;
        }

        public void ClearSprites()
        {
            Sprites = new List<SpriteDisplay>();
        }

        public void AddSprite(SpriteSource sprite, Location location)
        {
            Sprites.Add(new SpriteDisplay(location, sprite));
        }

        public void Blank()
        {
            Fill(SpriteMap.Blank);
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
                Update(SpriteMap.Character(colour, c), x, y);
                x++;
            }
        }

        public SpriteSource Get(int x, int y) => (x >= 0 && x < Width && y >= 0 && y < Height) ? _screenMap[x, y] : null;
    }

    public class SpriteDisplay
    {
        public readonly Location Location;
        public readonly SpriteSource Sprite;

        public SpriteDisplay(Location location, SpriteSource sprite)
        {
            Location = location;
            Sprite = sprite;
        }
    }
}
