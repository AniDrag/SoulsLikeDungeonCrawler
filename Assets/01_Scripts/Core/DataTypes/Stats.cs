using UnityEngine;

namespace AniDrag.Core
{
    [System.Serializable]
    public class Stats
    {
        public int VIT;
        public int STR;
        public int DEX;
        public int INT;

        public Stats(int vit = 1, int str = 1, int dex = 1, int intel = 1)
        {
            VIT = vit; STR = str; DEX = dex; INT = intel;
        }

        public Stats(Stats other)
        {
            VIT = other.VIT; STR = other.STR; DEX = other.DEX; INT = other.INT;
        }

        public void Add(Stats other)
        {
            VIT += other.VIT; STR += other.STR; DEX += other.DEX; INT += other.INT;
        }

        public void Reset() => VIT = STR = DEX = INT = 0;
    }
}