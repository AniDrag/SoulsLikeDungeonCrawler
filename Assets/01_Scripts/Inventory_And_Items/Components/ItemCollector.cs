using AniDrag.Inventory;
using UnityEngine;

namespace AniDrag.Core
{
    public class ItemCollector : MonoBehaviour
    {
        [Header("========================\n" +
                "      Collector Setup    \n" +
                "========================")]
        [SerializeField]
        private float _collectRange = 2f;

        [SerializeField] private LayerMask _itemLayer;
        [SerializeField] private InventoryHolder _inventoryHolder; // or IInventoryHolder

        private Collider[] _results = new Collider[10];

        private void Update()
        {
            int hits = Physics.OverlapSphereNonAlloc(transform.position, _collectRange, _results, _itemLayer);
            for (int i = 0; i < hits; i++)
            {
                var physicalItem = _results[i].GetComponent<PhysicalItemInstance>();
                if (physicalItem != null && physicalItem.CanBeCollected)
                    Collect(physicalItem);
            }
        }

        private void Collect(PhysicalItemInstance physicalItem)
        {
            bool allAdded = true;
            foreach (var stack in physicalItem.Items)
            {
                if (!_inventoryHolder.AddItem(stack.item, stack.amount))
                {
                    allAdded = false;
                    break;
                }
            }

            if (allAdded)
                Destroy(physicalItem.gameObject);
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _collectRange);
        }
#endif
    }
}