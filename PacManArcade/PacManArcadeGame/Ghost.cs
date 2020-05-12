namespace PacManArcadeGame
{
    public class Ghost
    {
        public Location ScatterTarget;

        public GhostColour Colour;
        public Location Location;
        public Direction Direction;
        public Animation Animation;
        public GhostState State;

        public Ghost(GhostColour colour, Location location, Direction direction, Location scatterTarget)
        {
            Colour = colour;
            Location = location;
            Direction = direction;
            ScatterTarget = scatterTarget;
            Animation = new Animation(2, 10);
            State = GhostState.Alive;
        }

        public void Animate()
        {
            Animation.Tick();
        }
    }
}