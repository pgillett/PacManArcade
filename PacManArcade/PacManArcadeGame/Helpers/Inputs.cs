using System;

namespace PacManArcadeGame.Helpers
{
    public class Inputs
    {
        public InputDirection Direction;
        public readonly InputTrigger Coin = new InputTrigger();
        public readonly InputTrigger Player1Start = new InputTrigger();
        public readonly InputTrigger Reset = new InputTrigger();
        public readonly InputTrigger Pause = new InputTrigger();
        public readonly InputTrigger Tick = new InputTrigger();
        public readonly InputTrigger Invincible = new InputTrigger();
        public readonly InputTrigger FastForward = new InputTrigger();
        public readonly InputTrigger LevelSkip = new InputTrigger();
    }

    public class InputTrigger
    {
        private bool _set;

        public void Press()
        {
            _set = true;
        }

        public void On(Action action)
        {
            if (WasPressed) action();
        }

        public bool WasPressed
        {
            get
            {
                if (!_set) return false;
                _set = false;
                return true;
            }
        }
    }
}
