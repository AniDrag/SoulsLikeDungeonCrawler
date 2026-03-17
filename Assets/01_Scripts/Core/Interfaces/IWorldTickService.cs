namespace AniDrag.Core
{
    public interface IWorldTickService
    {
        float TickInterval { get; } // Seconds between ticks
        void Register(ITickable tickable); // Subscribe to ticks
        void Unregister(ITickable tickable); // Unsubscribe
    }
}