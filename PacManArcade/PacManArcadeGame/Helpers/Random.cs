namespace PacManArcadeGame.Helpers
{
    public class Random
    {
        private int _randomSeed = 1;

        public int Get(int range)
        {
            _randomSeed = (_randomSeed * 13) % 127;
            return _randomSeed % range;
        }
    }
}
