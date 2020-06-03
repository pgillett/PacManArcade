namespace PacManArcadeGame.Ai
{
    public readonly struct DistanceQueueItem
    {
        public readonly AiMapCell Cell;
        public readonly int Distance;

        public DistanceQueueItem(AiMapCell cell, int distance)
        {
            Cell = cell;
            Distance = distance;
        }
    }
}