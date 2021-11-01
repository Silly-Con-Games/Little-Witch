
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Analytics
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
            listeners[type].Add(listener);
        }

        public static void RemoveListener(Type type, Action<IGameEvent> listener)
        {
            listeners[type].Remove(listener);
        }

        public static void ProcessEvents()
        {
            while(queue.Count > 0)
            {
                IGameEvent e = queue.Dequeue();

                foreach (var listener in listeners[e.GetType()])
                    listener.Invoke(e);
            }
        }
    }
}