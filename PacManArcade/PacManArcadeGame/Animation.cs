namespace PacManArcadeGame
{
    public class Animation
    {
        public bool Active;
        public int Current;

        private int _tickCounter;
        private bool _loops;
        private int _steps;
        private int _tickPerStep;

        public Animation(int steps, int tickPerStep, bool loops = true)
        {
            _steps = steps;
            _tickPerStep = tickPerStep;
            _loops = loops;
            Reset();
        }

        public void Reset()
        {
            _tickCounter = 0;
            Current = 0;
            Active = true;
        }

        public void Tick()
        {
            _tickCounter++;
            if (_tickCounter >= _tickPerStep)
            {
                Current++;
                _tickCounter = 0;
                if (Current >= _steps)
                {
                    if (_loops)
                    {
                        Current = 0;
                    }
                    else
                    {
                        Active = false;
                    }
                }
            }
        }
    }
}