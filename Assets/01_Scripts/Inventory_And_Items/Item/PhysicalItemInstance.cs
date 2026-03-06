using AniDrag.Core;
using System.Collections.Generic;
using UnityEngine;
namespace AniDrag.InventoryAndItems
{
    public class PhysicalItemInstance : MonoBehaviour
    {
        public List<ItemStack> items { get; private set; } = new List<ItemStack>();

        public void SetInstanceData(List<ItemStack> pItems)
        {
            items = new List<ItemStack>(pItems);
        }
    }
}