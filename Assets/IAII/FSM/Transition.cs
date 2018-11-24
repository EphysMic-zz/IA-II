using System;

namespace IAII
{
    public class Transition<T>
    {
        T input;//valor del input
        State<T> targetState;//el target del state
        public event Action<T> OnTransition = delegate { };
        public T Input { get { return input; } }
        public State<T> TargetState { get { return targetState; } }
        //Constructor.
        public Transition(T input, State<T> targetState)
        {
            this.input = input;
            this.targetState = targetState;
        }
        public void OnTransitionExecute(T input)
        {
            OnTransition(input);
        }
    }
}

