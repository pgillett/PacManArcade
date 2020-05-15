namespace PacManArcadeGame.Graphics
{
    public class Animation
    {
        public bool Active { get; private set; }
        public int Current { get; private set; }

        private int _tickCounter;
        private readonly bool _loops;
        private readonly int _steps;
        private readonly int _tickPerStep;

        public bool IsZero => Current == 0;

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