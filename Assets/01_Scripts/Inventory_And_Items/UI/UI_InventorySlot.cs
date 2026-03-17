using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using AniDrag.Core;

namespace AniDrag.Inventory
{
      public class UI_InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("========================\n" +
                "         UI Parts        \n" +
                "========================")]
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _stackText;
        [SerializeField] private Button _useButton;
        [SerializeField] private Button _removeButton;

        private IInventoryHolder _holder;
        private ItemStack _currentStack;
        private int _slotIndex;   

        public void Initialize(IInventoryHolder holder, int index, ItemStack stack)
        {
            _holder = holder;
            _slotIndex = index;
            _currentStack = stack; // ← missing

            _icon.sprite = stack.item.icon;
            _icon.enabled = true;
            _stackText.text = stack.amount > 1 ? stack.amount.ToString() : "";

            ConfigureButtons(); // ← call this to set up button text/visibility

            _useButton.onClick.AddListener(OnClickUse);
            _removeButton.onClick.AddListener(OnClickRemove);
        }

        public void Refresh()
        {
            if (_currentStack == null || _currentStack.amount <= 0)
            {
                Destroy(gameObject);
                return;
            }
            _stackText.text = _currentStack.amount > 1 ? _currentStack.amount.ToString() : "";
        }

        private void OnClickUse()
        {
            _holder?.UseItemAtIndex(_slotIndex);
        }
        private void ConfigureButtons()
        {
            TMP_Text _useButtonText = _useButton.transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text _removeButtonText = _removeButton.transform.GetChild(0).GetComponent<TMP_Text>();
            Item _currentItem = _currentStack.item; 

            // 1. Use button text/visibility
            if (_currentItem.itemType == ItemType.Equipment)
            {
                _useButton.gameObject.SetActive(true);
                if (_useButtonText != null) _useButtonText.text = "Equip";
            }
            else if (_currentItem.itemType == ItemType.Consumable)
            {
                _useButton.gameObject.SetActive(true);
                if (_useButtonText != null) _useButtonText.text = "Use";
            }
            else
            {
                _useButton.gameObject.SetActive(false);
            }

            // 2. Remove button visibility (hide for quest items)
            if (_currentItem.itemType == ItemType.Quest)
            {
                _removeButton.gameObject.SetActive(false);
            }
            else
            {
                _removeButton.gameObject.SetActive(true);
            }
        }
        private void OnClickRemove()
        {
            // Get current stack from holder to know which item to remove
            var items = _holder.Items;
            if (_slotIndex >= 0 && _slotIndex < items.Count)
            {
                var stack = items[_slotIndex];
                _holder.RemoveItem(stack.item, 1);
            }
        }

        private Coroutine _showTooltipCoroutine;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_currentStack == null) return;
            if (_showTooltipCoroutine != null) StopCoroutine(_showTooltipCoroutine);
            _showTooltipCoroutine = StartCoroutine(ShowTooltipAfterDelay(eventData.position));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_showTooltipCoroutine != null) StopCoroutine(_showTooltipCoroutine);
            UI_ItemTooltip.Hide();
        }

        private IEnumerator ShowTooltipAfterDelay(Vector2 position)
        {
            yield return new WaitForSeconds(0.3f);
            UI_ItemTooltip.Show(_currentStack.item, position);
            _showTooltipCoroutine = null;
        }

        private void OnDisable()
        {
            UI_ItemTooltip.Hide();
        }
    }
}