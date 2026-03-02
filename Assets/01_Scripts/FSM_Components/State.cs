using UnityEngine;
namespace AniDrag.Core
{
    public class State<TController> : IState
           where TController : FSM
    {

        protected readonly TController controller;
        protected readonly Animator animator;
        public virtual string StateName()
        {
            return "This is a default state";
        }
        protected State(TController pController, Animator pAnimator)
        {
            controller = pController;
            animator = pAnimator;
        }
        public virtual void TransitionSetup() { }
        public virtual void OnTickUpdate() { }
        public virtual void OnEnter() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnExit() { }

    }
}