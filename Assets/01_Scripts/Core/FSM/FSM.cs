using System;
using System.Collections.Generic;
using UnityEngine;
namespace AniDrag.Core
{
    /// <summary>
    /// Basicaly an FSM setup. This will bea inherited by Controllers like a PlayerController:FSM. And can be then made to use different Controllers or Phases Ehh or i can add a Blackboard to stuff that needs it
    /// Smart stuff here
    /// /// <summary>
    ///     So how does this work?
    ///         By Creating a Inhereted script from FSM Like PlayerController we can then add any function or transition method on here. 
    ///         And then make transitions happen naturaly. Anay checks or anything like that should be segmented in to a
    ///         # Region transition Logic
    ///             #"State name " Transitions.
    ///         Thast how we will manage everything and having less cross refrences than needed.
    ///     
    ///     Why not use the schools provided FSM?
    ///         I wanted to create my own variant. and i think its good. We have State transitions, And FromAnyState Transitions witch the other one does not have.
    ///         I did a lot of research to get to this point for this. So i used it like this
    /// </summary>
    /// </summary>
    public class FSM : MonoBehaviour
    {
        #region Classes
         class StateNode
        {
            [Header("========================\n" +
     "    State Node Details      \n" +
     "========================")]
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

        [Header("========================\n" +
   "    FSM Refrences      \n" +
   "========================")]
        [SerializeField] protected Animator animator;

        // ------------------------------------------------------------------------------------------------------------------
        // Private vars ----------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------------
        private StateNode oldState;
        private StateNode currentState;
        private Dictionary<Type, StateNode> nodes = new Dictionary<Type, StateNode>();// Type ? well i dont need to give it a name or anything. Type makes its own judgement
        protected HashSet<ITransition> anyTransition = new HashSet<ITransition>();
        protected bool disableAnimationTriggers = false;

        protected virtual void Update()
        {
            ITransition transition = GetTransition();
            if (transition != null)
                ChangeState(transition.to);

            currentState.State?.OnUpdate();
        }
        protected virtual void FixedUpdate()
        {
            currentState.State.OnFixedUpdate();
        }

        #region FSM Public API
        public void SetState(IState state)
        {
            currentState = GetOrAddNode(state);
            currentState.State?.OnEnter();
#if UNITY_EDITOR 
            Debug.Log($"Initial State Set to: {currentState.State.StateName()}, by object: {this.gameObject.name}");
#endif
        }

        public void ChangeState(IState state)
        {
            if (state == currentState.State) return;


            oldState = currentState;
            currentState = GetOrAddNode(state);

            oldState.State.OnExit();
            currentState.State.OnEnter();

#if UNITY_EDITOR
            Debug.Log($"Switching State to: {currentState.State.StateName()}, by gameobject: {this.gameObject.name}");
#endif
        }
        // Like we have in class add a transition for the state we want to.in Base Cotroller well have a void StateSetup.
        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }

        // Addin a From any state transition
        public void AddAnyTransition(IState to, IPredicate condition)
        {
            anyTransition.Add(new Transition(to, condition));
        }
        public void LogCurrentState()
        {
            if (currentState != null && currentState.State != null)
                Debug.Log($"Object: {this.gameObject.name} | -> | Current State: {currentState.State.StateName()}");
            else
                Debug.Log("No current state set");
        }
        #endregion

        #region FSM Functions
        protected virtual void SetupFSM()
        {
            if (animator == null)
            {
                Debug.Log("No animator detected, Truning off animation triggers");
                disableAnimationTriggers = true;
            }
        }
        ITransition GetTransition()
        {
            // Check any transitions first
            foreach (var transition in anyTransition)
            {
                if (transition.condition != null && transition.condition.Evaluate())
                    return transition;
            }

            // Check current state transitions
            if (currentState != null)
            {
                foreach (var transition in currentState.Transitions)
                {
                    if (transition.condition != null && transition.condition.Evaluate())
                        return transition;
                }
            }
            return null;

        }
        StateNode GetOrAddNode(IState state)
        {
            var node = nodes.GetValueOrDefault(state.GetType());

            if (node == null)
            {
                node = new StateNode(state);
                nodes.Add(state.GetType(), node);
            }
#if UNITY_EDITOR
            //Debug.Log($"Got node State Set to: {node.State.StateName()}");
#endif

            return node;
        }
        
        #endregion







    } 
}
