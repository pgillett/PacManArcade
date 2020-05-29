using PacManArcadeGame.GameItems;

namespace PacManArcadeGame.Ai
{
    public class GhostDistance
    {
        public readonly Ghost Ghost;
        public readonly int Distance;

        public GhostDistance(Ghost ghost, int distance)
        {
            Ghost = ghost;
            Distance = distance;
        }
    }
}