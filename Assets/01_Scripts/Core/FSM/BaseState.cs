
using UnityEngine;

namespace AniDrag.Core
{
    // So i dont need to cast or reference type of FSM we get the referenced type == BaseState<T> : IState where T:FSM is the same just more clearer for the reader
    public class BaseState<TController> : IState
       where TController : FSM
    {
        /// <summary>
        /// This is dependent on the script that inherites from IController It is used as a Blueprint.
        /// </summary>
        protected readonly TController controller;
        protected readonly Animator animator;

        #region Base State Functionality
        public virtual string StateName()
        {
            return "no name set";
        }
        protected BaseState(TController pController, Animator pAnimator)
        {
            controller = pController;
            animator = pAnimator;
        }
        public virtual void TransitionSetup(){}
        public virtual void OnEnter() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnExit() { }
        public virtual void OnTickUpdate() { }
        #endregion

    }
}
