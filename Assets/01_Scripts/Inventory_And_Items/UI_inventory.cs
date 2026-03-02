using UnityEngine;
namespace AniDrag.InventoryAndItems
{
    public class UI_inventory : MonoBehaviour
    {
        [Header("========================\n" +
               "        References       \n" +
               "========================")]
        [SerializeField] private InventoryController controller;
        [SerializeField] private GameObject itemUIPrefab;
        [SerializeField] private Transform contentParent; // e.g., scroll view content

        private void OnEnable()
        {
            if (controller != null)
            {
                controller.OnInventoryChanged.AddListener(RefreshUI);
                RefreshUI();
            }
        }

        private void OnDisable()
        {
            if (controller != null)
                controller.OnInventoryChanged.RemoveListener(RefreshUI);
        }

        private void RefreshUI()
        {
            // Clear existing UI items
            foreach (Transform child in contentParent)
                Destroy(child.gameObject);

            // Create new UI items for each stack
            foreach (var stack in controller.GetItems())
            {
                GameObject go = Instantiate(itemUIPrefab, contentParent);
                ItemUI itemUI = go.GetComponent<ItemUI>();
                itemUI.Setup(stack, controller);
            }
        }
    }
}