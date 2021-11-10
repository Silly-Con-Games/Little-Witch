
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameEvents
{
    public static class GameEventQueue
    {

        static Queue<IGameEvent> queue = new Queue<IGameEvent>(100);
        static Dictionary<Type, HashSet<Action<IGameEvent> > > listeners = new Dictionary<Type, HashSet<Action<IGameEvent> > >();
        
        public static void QueueEvent(IGameEvent e)
        {
            queue.Enqueue(e);
        }

        public static void AddListener(Type type, Action<IGameEvent> listener)
        {
            if (listeners.ContainsKey(type))
                listeners[type].Add(listener);
            else
            {
                HashSet<Action<IGameEvent>> set = new HashSet<Action<IGameEvent>>();
                set.Add(listener);
                listeners.Add(type, set);
            }
        }

        public static void RemoveListener(Type type, Action<IGameEvent> listener)
        {
            if(listeners.ContainsKey(type))
                listeners[type].Remove(listener);
        }

        public static void ProcessEvents()
        {
            while(queue.Count > 0)
            {
                IGameEvent e = queue.Dequeue();
                Type type = e.GetType();
                if(listeners.ContainsKey(type))
                    foreach (var listener in listeners[type])
                        listener.Invoke(e);
            }
        }
    }
}