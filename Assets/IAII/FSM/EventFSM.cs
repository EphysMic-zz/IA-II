using System;

namespace IAII
{
    public class EventFSM<T>
    {
        private State<T> current;

        public EventFSM(State<T> init)
        {
            current = init;
            current.Enter(default(T));
        }

        public void SendInput(T input)
        {
            State<T> newState;

            if (current.CheckInput(input, out newState))
            {
                current.Exit(input);
                current = newState;
                current.Enter(input);
            }
        }

        public State<T> Current { get { return current; } }

        public void Update()
        {
            current.Update();
        }

        public void LateUpdate()
        {
            current.LateUpdate();
        }

        public void FixedUpdate()
        {
            current.FixedUpdate();
        }
    }
}