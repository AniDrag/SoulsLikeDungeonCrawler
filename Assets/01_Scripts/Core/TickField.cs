using System;

namespace AniDrag.Core
{
    [System.Serializable]
    public class TickField
    {
        public int tickRate;
        public int tickCounter;// { get; private set; }
        private Action onTickTriggered;

        #region Constructors
        public TickField(int rate)
        {
            tickRate = rate;
            tickCounter = 0;
        }

        public TickField(int rate, Action triggerFunction)
        {
            tickRate = rate;
            tickCounter = 0;
            onTickTriggered = triggerFunction;
        }

        // Static factory methods
        public static TickField Create(int rate, Action triggerFunction = null)
        {
            return new TickField(rate, triggerFunction);
        }

        #endregion

        // Instance method for assignment
        public void AssignTriggerFunction(Action functionToTrigger)
        {
            onTickTriggered = functionToTrigger;
        }

        /// <summary>
        /// Call it with your tick machine / Game on tick call.
        /// </summary>
        /// <returns></returns>
        public bool Tick()
        {
            tickCounter++;
            if (tickCounter >= tickRate)
            {
                Reset();
                onTickTriggered?.Invoke();
                return true;
            }
            return false;
        }
        
        public void Reset()
        {
            tickCounter = 0;
        }
    }
}