namespace PacManArcadeGame.Helpers
{
    public class Random
    {
        private int _randomSeed ;

        public int Get(int range)
        {
            _randomSeed = (_randomSeed * 13) % 123;
            return _randomSeed % range;
        }
    }
}
