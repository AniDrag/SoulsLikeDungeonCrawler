using UnityEngine;

namespace AniDrag.Core
{
    // 2 stat values growth and normal stats, level. we return the calculation.
    public class StatsCalculator
    {
        // Health
        public static int MaxHealth(Stats baseStats, int level) 
            => 100 + baseStats.VIT * (25 + level);

        // Stamina
        public static int MaxStamina(Stats baseStats)
            => baseStats.STR * 2 + baseStats.DEX * 2 + 10;

        public static int StaminaRegenRate(Stats baseStats, int maxStamina)
            => Mathf.Max(1, (maxStamina / 100) + (baseStats.DEX / 10));

        // Mana
        public static int MaxMana(Stats baseStats)
            => baseStats.INT * 5 + 50;

        public static int ManaRegenRate(Stats baseStats, int maxMana)
            => Mathf.Max(1, (maxMana / 100) + (baseStats.INT / 10));

        // Shield
        public static int MaxShield(Stats baseStats)
            => 50 + baseStats.VIT * 2;

        // Defense
        public static int BaseDefense(Stats baseStats)
            => baseStats.VIT * 2;

        // You can add more as needed
    }
}