using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.Graphics
{
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