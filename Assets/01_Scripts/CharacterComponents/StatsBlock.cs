
using UnityEngine;
namespace AniDrag.CharacterComponents
{
    /// <summary>
    /// this class extends the Stats class to include growth factors for leveling up a character.
    /// This allows characters to increase their stats based on predefined growth rates when they level up.
    /// And is the main componet used in the Entity class for character stats management.
    /// </summary>
    [System.Serializable]
    public class StatsBlock : Stats
    {
        [Header("========================\n" +
     "   Stat Block Details      \n" +
     "========================")]
        public Stats GrowthFactors { get; private set; }
        public Stats EquipmentBonus { get; private set; }

        private int level;
        private int startVIT;
        private int startSTR;
        private int startDEX;
        private int startINT;

        public StatsBlock(Stats baseStats, Stats growthFactors) : base(baseStats)
        {
            startVIT = baseStats.VIT;
            startSTR = baseStats.STR;
            startDEX = baseStats.DEX;
            startINT = baseStats.INT;
            EquipmentBonus = new Stats(0, 0, 0, 0);
            GrowthFactors = new Stats(growthFactors);
        }
        /// <summary>
        /// Update stats based on level and optional equipment stats.
        /// </summary>
        /// <param name="pLevel"></param>
        /// <param name="equipmentStats"></param>
        public void UpdateStats(int pLevel, Stats equipmentStats = null)
        {
            if (equipmentStats != null) { EquipmentBonus = new Stats(equipmentStats); }

            level = pLevel - 1; // Level 1 is base stats, so we subtract 1 to avoid double counting
            
            VIT = GrowthFactors.VIT * level + startVIT + EquipmentBonus.VIT;
            STR = GrowthFactors.STR * level + startSTR + EquipmentBonus.STR;
            DEX = GrowthFactors.DEX * level + startDEX + EquipmentBonus.DEX;
            INT = GrowthFactors.INT * level + startINT + EquipmentBonus.INT;
        }
    }
}
