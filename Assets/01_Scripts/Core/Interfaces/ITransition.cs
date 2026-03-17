namespace AniDrag.Core
{
    public interface ITransition
    {
        public IState to { get; }
        public IPredicate condition { get; }

    }
}