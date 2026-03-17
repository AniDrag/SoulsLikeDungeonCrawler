using AniDrag.Core;
using UnityEngine;
using UnityEngine.UI;

namespace AniDrag.Inventory
{
    public class UI_Inventory : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _owner; 
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private Transform _slotContainer; 
        [SerializeField] private GameObject _panel; 

        private IInventoryHolder _inventoryHolder;
        private bool _isOpen = false;

        private void Awake()
        {
            if (_owner != null)
                _inventoryHolder = _owner.GetComponent<IInventoryHolder>();
            else
                _inventoryHolder = GetComponentInParent<IInventoryHolder>();

            if (_inventoryHolder == null)
            {
                Debug.LogError("UI_Inventory: No IInventoryHolder found.", this);
                return;
            }
            _panel.SetActive(false);
        }
        private void Start()
        {
            if (_inventoryHolder != null)
            {
                _inventoryHolder.OnInventoryChanged += RefreshUI;
                RefreshUI();
            }
        }
        private void OnEnable()
        {
            if (Services.Input != null)
                Services.Input.OnInventoryToggle += ToggleInventory;
            else
            {
                Debug.LogWarning("UI_Inventory: Input service not ready, will retry.");
                Invoke(nameof(RetrySubscribe), 0.1f);
            }
        }
        private void OnDisable()
        {
            if (Services.Input != null)
                Services.Input.OnInventoryToggle -= ToggleInventory;
            CancelInvoke();
        }

        private void RetrySubscribe()
        {
            if (Services.Input != null)
                Services.Input.OnInventoryToggle += ToggleInventory;
            else
                Invoke(nameof(RetrySubscribe), 0.1f);
        }
        private void OnDestroy()
        {
            if (_inventoryHolder != null)
                _inventoryHolder.OnInventoryChanged -= RefreshUI;
        }

        private void ToggleInventory()
        {
            Debug.Log("ToggleInventory called");

            _isOpen = !_isOpen;
            _panel.SetActive(_isOpen);

            if (_isOpen)
            {
#if  UNITY_EDITOR
                
                Debug.Log("Opening inventory - refreshing UI");
#endif
                RefreshUI();
                Services.GameState.FreezeTime();
                Services.GameState.UnlockCursor();
            }
            else
            {
                Services.GameState.UnfreezeTime();
                Services.GameState.LockCursor();
            }
        }

        private void RefreshUI()
        {
            foreach (Transform child in _slotContainer)
                Destroy(child.gameObject);

            int index = 0;
            foreach (var stack in _inventoryHolder.Items)
            {
                if (stack.item == null || stack.amount <= 0) continue;

                GameObject slotGO = Instantiate(_slotPrefab, _slotContainer);
                var slot = slotGO.GetComponent<UI_InventorySlot>();
                if (slot != null)
                    slot.Initialize(_inventoryHolder, index, stack);
                else
                    Debug.LogError("UI_InventorySlot component missing on prefab.");
                index++;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_slotContainer as RectTransform);
        }
    }
}