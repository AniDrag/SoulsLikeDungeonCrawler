using UnityEngine;

namespace AniDrag.CharacterComponents
{
    /// <summary>
    /// This class represents the core stats of a character, including Vitality (VIT), Strength (STR), Dexterity (DEX), and Intelligence (INT).
    /// Other stats and attributes can be derived from these core stats.
    /// And easily epanded in the future.
    /// </summary>
    [System.Serializable]
    public class Stats 
    {
        [Header("========================\n" +
"    Stats Details     \n" +
"========================")]
        public int VIT;
        public int STR;
        public int DEX;
        public int INT;

        #region Initializations
        public Stats()
        {
            // Default values if Initialize() is never called
            VIT = 1;
            STR = 1;
            DEX = 1;
            INT = 1;
        }

        /// <summary>
        /// Initialize stats and automatically assign start values.
        /// </summary>
        public Stats(int vit, int str, int dex, int intel)
        {
            VIT = vit;
            STR = str;
            DEX = dex;
            INT = intel;
        }
       
        /// <summary>
        /// Copy constructor.
        /// </summary>
        public Stats(Stats newStats)
        {
            VIT = newStats.VIT;
            STR = newStats.STR;
            DEX = newStats.DEX;
            INT = newStats.INT;
        }
        #endregion

        public void IncreaseByStats(Stats newStats)
        {            
            VIT = newStats.VIT;
            STR = newStats.STR;
            DEX = newStats.DEX;
            INT = newStats.INT;
        }     
        public void Reset()
        {
            VIT = 0;
            STR = 0;
            DEX = 0;
            INT = 0;
        }

    }
}