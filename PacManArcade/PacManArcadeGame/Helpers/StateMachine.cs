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
        private bool _triggered;
        private bool _changed;
        private bool _finalise;

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
            _triggered = _changed;
            _changed = false;
        }

        public void ChangeState(T state)
        {
            _nextState = state;
            _leaving = !_nextState.Equals(Current);
            _changed = true;
        }

        private StateMachine<T> Execute(Func<bool> condition, params Action[] actions)
        {
            if (condition())
            {
                foreach (var action in actions)
                    action();
            }

            return this;
        }

        public StateMachine<T> OnEntry(T state, params Action[] actions)
        {
            return Execute(()=>Current.Equals(state) && _entering, actions);
        }

        public StateMachine<T> OnTrigger(T state, params Action[] actions)
        {
            return Execute(() => Current.Equals(state) && _triggered, actions);
        }

        public StateMachine<T> During(T state, params Action[] actions)
        {
            return Execute(() => Current.Equals(state), actions);
        }

        public StateMachine<T> During(IEnumerable<T> states, params Action[] actions)
        {
            return Execute(() =>states.Contains(Current), actions);
        }

        public StateMachine<T> NotDuring(T states, params Action[] actions)
        {
            return Execute(() => !states.Equals(Current), actions);
        }

        public StateMachine<T> NotDuring(T state1, T state2, params Action[] actions)
        {
            return Execute(() => !Current.Equals(state1) && !Current.Equals(state2), actions);
        }

        public StateMachine<T> OnExit(T state, params Action[] actions)
        {
            return Execute(() => Current.Equals(state) && _leaving, actions);
        }

        public bool IsCurrent(params T[] states) => states.Contains(Current);

        public void End()
        {
            _finalise = true;
        }

        public bool HasEnded => _finalise;
    }
}