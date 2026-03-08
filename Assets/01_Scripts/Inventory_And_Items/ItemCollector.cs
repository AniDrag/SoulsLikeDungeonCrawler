using AniDrag.Core;
using AniDrag.Quest;
using UnityEngine;
namespace AniDrag.InventoryAndItems
{
    public class ItemCollector : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Transform collectPoint;
        [SerializeField] private float collectRange = 1f;
        [SerializeField] private float collectInterval = 0.1f;
        [SerializeField] private LayerMask itemLayer;
        [SerializeField] private InventoryController inventory;

        // physics . sphere cast every 0.1s to check for items in range, if there is an item, collect it and add it to the inventory.

        float timer = 0f;

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= collectInterval)
            {
                timer = 0f;
                CollectItems();
            }
        }
        void CollectItems()
        {
            Collider[] itemsInRange = Physics.OverlapSphere(transform.position, collectRange, itemLayer);
            foreach (Collider itemCollider in itemsInRange)
            {
                PhysicalItemInstance item = itemCollider.GetComponent<PhysicalItemInstance>();
                if (item.items.Count > 0)
                {
                    foreach (ItemStack i in item.items)
                    {
                        QuestBus.Instance.Enqueue(new ItemPickedEvent(i.item, i.amount));
                        inventory.AddItem(i.item, i.amount);
                    }
                }

                Destroy(item.gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            if(collectPoint == null) return;
            Gizmos.color = new Color(1,1,0,.1f)
            ;
            Gizmos.DrawWireSphere(collectPoint.position, collectRange);
        }
    }
}