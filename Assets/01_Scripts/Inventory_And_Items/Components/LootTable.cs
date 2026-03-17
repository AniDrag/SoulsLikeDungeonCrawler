using System;
using System.Collections.Generic;
using AniDrag.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AniDrag.Inventory
{
    [Serializable]
    public class LootTable
    {
        [SerializeField] private List<ItemStack> _fixedLoot = new List<ItemStack>();
        [SerializeField] private List<DroppableItem> _randomLoot = new List<DroppableItem>();

        /// <summary>
        /// Generates a list of item stacks based on fixed + random rolls.
        /// </summary>
        public List<ItemStack> GenerateLoot()
        {
            List<ItemStack> drops = new List<ItemStack>(_fixedLoot);

            foreach (var entry in _randomLoot)
            {
                if (Random.value <= entry.chance)
                {
                    int amount = Random.Range(entry.minAmount, entry.maxAmount + 1);
                    drops.Add(new ItemStack(entry.item, amount));
                }
            }
            return drops;
        }
    }
}