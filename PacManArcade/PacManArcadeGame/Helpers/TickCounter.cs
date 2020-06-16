using System;

namespace PacManArcadeGame.Helpers
{
    public class TickCounter
    {
        private int _counter;
        
        public void Tick()
        {
            _counter--;
        }

        public void NextEventAfter(int ticks)
        {
            _counter = ticks;
        }

        public void PushEvent(int ticks)
        {
            _counter += ticks;
        }

        private bool IsAtEvent => _counter <= 0;

        public void AtEvent(Action action)
        {
            if (IsAtEvent) action();
        }

        public bool IsWithinNext(int ticks) => _counter <= ticks;

        public bool IsTickStepZero(int tickSteps) => _counter % tickSteps == 0;
    }
}
