namespace IAII
{
    public class EventFSM<T>
    {
        State<T> current;
        public State<T> Current { get { return current; } }

        //Constructor.
        public EventFSM(State<T> init)
        {
            current = init;
            current.Enter(default(T));
        }

        public void Feed(T input)
        {
            State<T> newState;

            if (current.CheckInput(input, out newState))
            {
                current.Exit(input);
                current = newState;
                current.Enter(input);
            }
        }

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