using AniDrag.Core;
using System.Collections.Generic;
using UnityEngine;
namespace AniDrag.Inventory
{
    public class PhysicalItemInstance : MonoBehaviour
    {
        [SerializeField] private List<ItemStack> _items = new List<ItemStack>();
        [SerializeField] private bool _canBeCollected = true;

        public IReadOnlyList<ItemStack> Items => _items;
        public bool CanBeCollected => _canBeCollected;

        public void SetItems(List<ItemStack> items)
        {
            _items = new List<ItemStack>(items);
        }

        public void SetItems(Item item, int amount)
        {
            _items = new List<ItemStack> { new ItemStack(item, amount) };
        }
    }
}