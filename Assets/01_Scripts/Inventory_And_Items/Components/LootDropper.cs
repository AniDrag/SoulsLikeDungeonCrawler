using System.Collections.Generic;
using UnityEngine;
namespace AniDrag.Inventory
{
    /// <summary>
    /// Spawns a physical item instance with the given list of item stacks.
    /// Call DropLoot() from anywhere (e.g., health component, death handler).
    /// Mainly Called In unity Events.
    /// </summary>
    public class LootDropper : MonoBehaviour
    {
        [SerializeField] private LootTable _lootTable;
        [SerializeField] private GameObject _lootPrefab; // PhysicalItemInstance prefab

        public void DropLoot()
        {
            if (_lootPrefab == null || _lootTable == null) return;
            GameObject lootGO = Instantiate(_lootPrefab, transform.position, Quaternion.identity);
            var physical = lootGO.GetComponent<PhysicalItemInstance>();
            if (physical != null)
                physical.SetItems(_lootTable.GenerateLoot());
        }
    }
}
