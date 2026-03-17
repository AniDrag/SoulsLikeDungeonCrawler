using System;

namespace AniDrag.Core
{
    public interface IEventBus
    {
        void Publish<T>(T ev) where T : BaseEvent;
        void Subscribe<T>(Action<T> handler) where T : BaseEvent;
        void Unsubscribe<T>(Action<T> handler) where T : BaseEvent;
    }
}