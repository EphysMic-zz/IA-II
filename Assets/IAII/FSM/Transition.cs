using System;

namespace IAII
{
    public class Transition<T>
    {
        //valor del input
        T input;
        //el target del state
        State<T> targetState;
        public event Action<T> OnTransition = delegate { };
        public T Input { get { return input; } }
        public State<T> TargetState { get { return targetState; } }

        public void OnTransitionExecute(T input)
        {
            OnTransition(input);
        }
        public Transition(T _input, State<T> _targetState)
        {
            this.input = _input;
            this.targetState = _targetState;
        }
    }
}

