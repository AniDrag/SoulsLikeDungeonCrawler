using System;
using UnityEngine;
using UnityEngine.Events;
using AniDrag.Utility;

namespace AniDrag.CharacterComponents
{
    /// <summary>
    /// Anything that requires experience point management can use this component.
    /// This is mainly used by the PlayerEntity class to handle leveling up and XP gain.
    /// Can be attached to an enemy or non-player character to increase their level dynamically. Mainly for special use cases.
    /// </summary>
    public class XpComponent : MonoBehaviour
    {
        [Header("========================\n" +
     "    Xp Component Details      \n" +
     "========================")]
        [field: SerializeField] public int maxXP { get; private set; } = 100;
        public int currentXP { get; private set; }
        [SerializeField] int level = 1;              // Current level
        [SerializeField] int maxLevel = 100;         // Optional max level cap
        [SerializeField] int baseXP = 100;           // Base XP for level 1 -> 2
        [SerializeField] float xpMultiplier = 1.5f;  // How fast XP grows each level

        [Header("========================\n" +
      "    Event Connectors      \n" +
      "========================")]
        public UnityEvent<int> onLevelUp;

        // ------------------------------------------------------------------------------------------------------------------
        // Actions ----------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------------
        public Action<XpComponent> updateXp;

        public void InitializeXpComponent(int pLevel)
        {
            level = pLevel;
            currentXP = 0;
            NewMaxXP();
            updateXp?.Invoke(this);
        }

        public void GainXp(int amount)
        {
            currentXP += amount;

            // Handle multiple level-ups if XP overflows
            while (currentXP >= maxXP && level < maxLevel)
            {
                currentXP -= maxXP;
                LevelUp();
                // Only invoke onLevelUp when we finished the wuhile loop to avoid multiple calls and save performance
                if (currentXP< maxXP) 
                    onLevelUp?.Invoke(level);
            }

            updateXp?.Invoke(this);
            
        }

        private void LevelUp()
        {
            level++;
            NewMaxXP();
            Debug.Log($"Leveled up! New Level: {level}, Next XP: {maxXP}");
        }

        void NewMaxXP()
        {
            // Exponential XP scaling formula
            maxXP = Mathf.RoundToInt(baseXP * Mathf.Pow(level, xpMultiplier));
        }

        // Optional: Get current level for external systems
        public int GetLevel() => level;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] int gainXP = 25;
        
        [Button]
        public void AddXP()
        {
            GainXp(gainXP);
            //Debug.Log($"Stamina now: {currentXP}, And Level: {level}");
        }
#endif
    }
}
