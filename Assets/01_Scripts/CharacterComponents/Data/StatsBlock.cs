using System;
using AniDrag.Core;

namespace AniDrag.CharacterComponents
{
    /// <summary>
    /// Represents a character's complete stat block, including base stats,
    /// growth per level, and equipment bonuses.
    /// </summary>
    [Serializable]
    public class StatsBlock
    {
        private Stats _baseStats;
        private Stats _growthFactors;
        private Stats _equipmentBonuses;
        private int _level;

        // Current calculated stats
        public Stats CurrentStats { get; private set; }

        public Stats BaseStats => _baseStats;

        public StatsBlock(Stats baseStats, Stats growthFactors)
        {
            _baseStats = new Stats(baseStats);
            _growthFactors = new Stats(growthFactors);
            _equipmentBonuses = new Stats(0, 0, 0, 0);
            CurrentStats = new Stats(_baseStats);
        }

        /// <summary>
        /// Update stats based on current level and equipment bonuses.
        /// </summary>
        public void UpdateStats(int level, Stats equipmentBonuses = null)
        {
            _level = level;
            if (equipmentBonuses != null)
                _equipmentBonuses = new Stats(equipmentBonuses);

            int levelBonus = _level - 1; // level 1 gives no growth

            CurrentStats.VIT = _baseStats.VIT + _growthFactors.VIT * levelBonus + _equipmentBonuses.VIT;
            CurrentStats.STR = _baseStats.STR + _growthFactors.STR * levelBonus + _equipmentBonuses.STR;
            CurrentStats.DEX = _baseStats.DEX + _growthFactors.DEX * levelBonus + _equipmentBonuses.DEX;
            CurrentStats.INT = _baseStats.INT + _growthFactors.INT * levelBonus + _equipmentBonuses.INT;
        }

        /// <summary>
        /// Apply equipment bonuses (called when equipment changes).
        /// </summary>
        public void ApplyEquipmentBonuses(Stats bonuses)
        {
            _equipmentBonuses = new Stats(bonuses);
            UpdateStats(_level, _equipmentBonuses);
        }
    }
}