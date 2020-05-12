namespace PacManArcadeGame
{
    public class PacMan
    {
        public Location Location;
        public Direction Direction;
        public Animation Animation;
        public bool Dying;

        public PacMan(Location location, Direction direction)
        {
            Location = location;
            Direction = direction;
            Animation = new Animation(4, 2);
            Dying = false;
        }

        public void Animate()
        {
            Animation.Tick();
        }

        public void Die()
        {
            Animation = new Animation(12, 3, false);
            Dying = true;
        }
    }
}