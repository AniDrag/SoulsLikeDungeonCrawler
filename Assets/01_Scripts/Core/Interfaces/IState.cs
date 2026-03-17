namespace AniDrag.Core
{
    public interface IState
    {
        string StateName();
        void TransitionSetup();
        void OnEnter();
        void OnUpdate();
        void OnFixedUpdate();
        void OnTickUpdate();
        void OnExit();
    }
}