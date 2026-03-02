namespace AniDrag.Core
{
    public class Transition : ITransition
    {
        public IState to { get; }
        public IPredicate condition { get; }

        public Transition(IState pToState, IPredicate pCondition)
        {
            to = pToState;
            condition = pCondition;
        }
    }
}