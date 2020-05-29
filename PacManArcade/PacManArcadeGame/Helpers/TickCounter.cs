using System;

namespace PacManArcadeGame.Helpers
{
    public class TickCounter
    {
        private int _counter;
        private int _next;

        public void Reset()
        {
            _counter = 0;
        }

        public void Tick()
        {
            _counter++;
        }

        public void NextEventAfter(int ticks)
        {
            _next = _counter + ticks;
        }

        public bool IsAtEvent => _counter > _next;

        public void AtEvent(Action action)
        {
            if (IsAtEvent) action();
        }

        public bool IsWithinNext(int ticks) => _counter + ticks > _next;

        public bool IsTickStepZero(int tickSteps) => _counter % tickSteps == 0;
    }
}
