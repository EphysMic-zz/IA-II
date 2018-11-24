using System.Collections.Generic;

namespace IAII
{
    public class StateConfigurer<T>
    {
        State<T> instance;//estado que despues le meto transiciones
        Dictionary<T, Transition<T>> transitions = new Dictionary<T, Transition<T>>();

        //Constructor.
        public StateConfigurer(State<T> _state)
        {
            instance = _state;
        }


        public StateConfigurer<T> SetTransition(T input, State<T> target)
        {
            transitions.Add(input, new Transition<T>(input, target));
            return this;
        }

        public void Done()
        {
            instance.Configure(transitions);
        }
    }

    //crea y devuelve el state configurer
    public static class StateConfigurer
    {
        public static StateConfigurer<T> Create<T>(State<T> state)
        {
            return new StateConfigurer<T>(state);
        }
    }
}

