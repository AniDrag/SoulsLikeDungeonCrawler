using AniDrag.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AniDrag.InventoryAndItems
{
    public class ItemUI : MonoBehaviour
    {
        [Header("========================\n" +
               "       UI Elements       \n" +
               "========================")]
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private TMP_Text stackText;
        [SerializeField] private Image icon;
        [SerializeField] private Button useButton;

        private ItemStack itemStack;
        private InventoryController controller;

        private void Awake()
        {
            if (useButton != null)
                useButton.onClick.AddListener(OnUseClicked);
        }

        public void Setup(ItemStack stack, InventoryController invController)
        {
            itemStack = stack;
            controller = invController;

            if (itemNameText != null)
                itemNameText.text = stack.item.itemName;

            if (stackText != null)
                stackText.text = stack.amount.ToString();

            if (icon != null)
                icon.sprite = stack.item.icon;

            // Configure use button text based on item type
            if (useButton != null)
            {
                string buttonText = stack.item.itemType == ItemType.Equipment ? "Equip" : "Use";
                TMP_Text btnText = useButton.GetComponentInChildren<TMP_Text>();
                if (btnText != null) btnText.text = buttonText;
            }
        }

        private void OnUseClicked()
        {
            if (controller == null || itemStack == null) return;

            // Find the index of this stack in the inventory
            var items = controller.GetItems();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == itemStack)
                {
                    controller.UseItemAtIndex(i);
                    break;
                }
            }
        }
    }
}