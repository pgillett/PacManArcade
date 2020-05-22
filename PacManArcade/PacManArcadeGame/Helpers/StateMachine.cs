using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using PacManArcadeGame.GameItems;

namespace PacManArcadeGame.Helpers
{
    public class StateMachine<T>
    {
        public T Current { get; private set; }
        private T _last;
        private T _nextState;
        private bool _entering;
        private bool _leaving;

        public StateMachine(T state)
        {
            ChangeState(state);
        }

        public bool Changed
        {
            get
            {
                var c = !_last.Equals(Current);
                _last = Current;
                return c;
            }
        }

        public void Start()
        {
            _entering = !_nextState.Equals(Current);
            _leaving = false;
            Current = _nextState;
        }

        public void ChangeState(T state)
        {
            _nextState = state;
            _leaving = !_nextState.Equals(Current);
        }

        public bool OnEntry(T state)
        {
            return Current.Equals(state) && _entering;
        }

        public bool During(T state)
        {
            return Current.Equals(state);
        }

        public bool During(params T[] states)
        {
            return states.Contains(Current);
        }

        public bool NotDuring(T states)
        {
            return !states.Equals(Current);
        }

        public bool NotDuring(params T[] states)
        {
            return !states.Contains(Current);
        }

        public bool OnExit(T state)
        {
            return Current.Equals(state) && _leaving;
        }
    }
}