using System;
using System.Collections.Generic;
using UnityEngine;
namespace AniDrag.Core
{
    /// <summary>
    /// In core becasue everything can and might want to use animations
    /// </summary>
    public class FSM : MonoBehaviour// derive from IState
    {
        //  stored old state jsut incase
        //  current state 
        //  next state -> transition time
        //  public blackboard and internal blackboard.
        //  Animator refrence
        //  Dictionary of animations 
        //  A HahsSet of Transitions
        #region Classes
        public class StateNode
        {
            public IState State { get; private set; }
            public HashSet<ITransition> Transitions { get; private set; }

            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }

            public void AddTransition(IState to, IPredicate condition)
            {
                Transitions.Add(new Transition(to, condition));
            }
        }
        #endregion

        // -----------  Basic info  ------------------
        [field: SerializeField] public string NameFSM { get; private set; } = "FSM -> Not a phase";
        [field: SerializeField] public Animator animator { get; protected set; }

        // -----------  States info  ------------------
        private bool disableAnimationTriggers = false;
        private StateNode oldState;
        public StateNode currentState {get; private set;}
        private Dictionary<Type, StateNode> nodes = new Dictionary<Type, StateNode>();// Type ? well i dont need to give it a name or anything. Type makes its own judgement
        private HashSet<ITransition> anyTransition = new HashSet<ITransition>();

        // -----------  Transitioning info  ------------------
        protected Coroutine transitionWait;
        public IState nextState = null;
        protected bool isEntering = false;
        protected bool isExiting = false;
        protected bool isTransitioning = false;
        
        void Update()
        {
            // check transitions
            // curent state.on update func
        }

        #region Animation Events
        /// <summary>
        /// Used in animation clips as event trigers at start and end of a transitioning event. This locks Main Logic from moving when needed or depends on what u wish to use it with.
        /// This one is for Enter animations!
        /// </summary>
        public void AnimEv_EnteringState()
        {
            isEntering = !isEntering;
            Debug.Log($"FSM: event -> EnteringState: {currentState.State.StateName()} = {isEntering}");
            if (!isEntering)
            {
                currentState.State.OnEnter();
                Debug.Log($"FSM: event -> EnteringState: {currentState.State.StateName()} Executed On Enter state");
            }
        }
       
        /// <summary>
        /// Used in animation clips as event trigers at start and end of a transitioning event. This locks Main Logic from moving when needed or depends on what u wish to use it with.
        /// This one is for Exit animations!
        /// </summary>
        public void AnimEv_ExitingState()
        {
            isExiting = !isExiting;
            Debug.Log($"FSM: event -> Exiting State: {currentState.State.StateName()} = {isExiting}");
            if (!isExiting)
            {
                currentState.State.OnExit();
                Debug.Log($"FSM: event -> Exiting State: {currentState.State.StateName()} Executed On Exit state");
            }
        }
       
        /// <summary>
        /// Used in animation clips as event trigers at start and end of a transitioning event. This locks Main Logic from moving when needed or depends on what u wish to use it with.
        /// This one is for any inbetween state transition that is inside the state it self animations!
        /// </summary>
        public void AnimEv_Transitioning() { isTransitioning = !isTransitioning; }
       
        /// <summary>
        /// If you wish a return or check if we are currently transitioning.
        /// </summary>
        /// <returns></returns>
        public bool InTransitionState() { return isTransitioning || isEntering || isExiting; }

        #endregion

        #region Internal API
        // changin states,
        // check valid states or add states to the nodes lsit.
        // mybe use the LRU methodology idk

        #endregion

        #region Public API
        // Set initial state
        // Add state Nodes
        // Add transitions
        // TickUpdate(){ currentState.OnTick();}
        #endregion
        #region DEBUG Methods
        // Hsow transitions and stuff.
        public void Debug_NextState_log(){}
        public void Debug_Ente_Exit_log(bool isEntering = true)
        {
            if (currentState != null && currentState.State != null)
            {
                string OldStateName = oldState.State != null ? oldState.State.StateName() : " none";
                Debug.Log($"FSM -> Object: {this.gameObject.name} | -> | Current State: {currentState.State.StateName()} Old State: {OldStateName}");
            }
            else
                Debug.Log("FSM ->  No current state set");
        }
        #endregion
    }
}