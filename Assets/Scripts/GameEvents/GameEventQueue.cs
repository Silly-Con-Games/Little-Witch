
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameEvents
{
    public static class GameEventQueue
    {

        static Queue<IGameEvent> queue = new Queue<IGameEvent>(100);
        static Dictionary<Type, HashSet<Action<IGameEvent> > > listeners = new Dictionary<Type, HashSet<Action<IGameEvent> > >();
        static HashSet<(Type type, Action<IGameEvent> action)> toBeRemoved = new HashSet<(Type type, Action<IGameEvent> action)>();
        
        public static void QueueEvent(IGameEvent e)
        {
            queue.Enqueue(e);
        }

        public static void AddListener(Type type, Action<IGameEvent> listener)
        {
            if (toBeRemoved.Contains((type, listener)))
            {
                toBeRemoved.Remove((type, listener));
                return;
            }

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
            if (listeners.ContainsKey(type) && listeners[type].Contains(listener))
                toBeRemoved.Add((type, listener));
        }

        public static void ProcessEvents()
        {
            if(toBeRemoved.Count > 0)
            {
                foreach (var item in toBeRemoved)
                    if (listeners.ContainsKey(item.type) && listeners[item.type].Contains(item.action))
                        listeners[item.type].Remove(item.action);
                toBeRemoved.Clear();
            }

            while (queue.Count > 0)
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