namespace PacManArcadeGame.Graphics
{
    public class Animation
    {
        public bool Active { get; private set; }
        public int Current { get; private set; }

        private int _tickCounter;
        private readonly bool _loops;
        private readonly int[] _steps;
        private int _pause;

        public bool IsZero => Current == 0;

        public Animation(int steps, int tickPerStep, bool loops = true)
        {
            _steps = new int[steps];
            for (int i = 0; i < steps; i++)
            {
                _steps[i] = tickPerStep;
            }
            _loops = loops;
            Reset();
        }

        public Animation(int[] steps, bool loops = true)
        {
            _steps = steps;
            Reset();
        }

        public void Reset()
        {
            _tickCounter = 0;
            Current = 0;
            Active = true;
            _pause = 0;
        }

        public void ResetWithPause(int pause)
        {
            Reset();
            _pause = pause;
        }

        public void Stop()
        {
            Current = 0;
            Active = false;
        }

        public void Tick()
        {
            if (!Active) return;
            if (_pause > 0)
            {
                _pause--;
                return;
            }
            _tickCounter++;
            if (_tickCounter >= _steps[Current])
            {
                Current++;
                _tickCounter = 0;
                if (Current >= _steps.Length)
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