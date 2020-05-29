using PacManArcadeGame.Graphics;
using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.GameItems
{
    public class PacMan
    {
        public Location Location { get; private set; }
        public Direction Direction { get; private set; }
        public Animation Animation { get; private set; }
        public bool Dying { get;private set; }

        public PacMan(Location location, Direction direction)
        {
            Location = location;
            Direction = direction;
            Animation = new Animation(4, 2);
            Dying = false;
        }
        
        public void Die()
        {
            Animation = new Animation(new [] { 30, 7, 7, 7, 7, 7, 7, 7, 7, 7, 15}, false);
            Dying = true;
        }

        public void ChangeDirection(Direction direction)
        {
            Direction = direction;
        }

        public void Move(decimal x, decimal y)
        {
            Location = Location.Add(x, y);
        }

        public void MoveTowards(Location location, bool withAnimate)
        {
            var dx = (Location.X < location.X ? 0.125m : 0) +
                     (Location.X > location.X ? -0.125m : 0);
            var dy = (Location.Y < location.Y ? 0.125m : 0) +
                     (Location.Y > location.Y ? -0.125m : 0);
            if (dx != 0 || dy != 0)
            {
                Move(dx, dy);
                if(withAnimate)
                    Animation.Tick();
            }
        }

        public void KeepInBounds(decimal x, decimal y)
        {
            if (Location.IsOutOfBounds(x, y, out var dx, out var dy))
            {
                Location = Location.Add(dx, dy);
            }
        }
    }
}