using AniDrag.Core;
using UnityEngine;

namespace AniDrag.Inventory
{
    [System.Serializable]
    public struct DroppableItem
    {
        public Item item;
        [Range(0, 1)] public float chance;
        public int minAmount;
        public int maxAmount;
    }
}