using System;
using System.Collections.Generic;

namespace AniDrag.Core
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public void Publish<T>(T ev) where T : BaseEvent
        {
            if (_handlers.TryGetValue(typeof(T), out var delegates))
            {
                foreach (var d in delegates)
                {
                    (d as Action<T>)?.Invoke(ev);
                    if (ev.Consumed) break;
                }
            }
        }

        public void Subscribe<T>(Action<T> handler) where T : BaseEvent
        {
            var type = typeof(T);
            if (!_handlers.ContainsKey(type))
                _handlers[type] = new List<Delegate>();
            _handlers[type].Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : BaseEvent
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var delegates))
            {
                delegates.Remove(handler);
                if (delegates.Count == 0)
                    _handlers.Remove(type);
            }
        }
    }
}