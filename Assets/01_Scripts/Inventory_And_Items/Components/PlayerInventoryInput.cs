using AniDrag.Core;
using UnityEngine;

namespace AniDrag.Inventory
{
    public class PlayerInventoryInput: MonoBehaviour{
    [SerializeField] private InventoryHolder _inventoryHolder;
    [SerializeField] private GameObject _inventoryPanel;

    private void OnEnable()
    {
        Services.Input.OnInventoryToggle += ToggleInventory;
    }

    private void OnDisable()
    {
        Services.Input.OnInventoryToggle -= ToggleInventory;
    }

    private void ToggleInventory()
    {
        if (_inventoryPanel == null) return;

        bool isActive = !_inventoryPanel.activeSelf;
        _inventoryPanel.SetActive(isActive);

        if (isActive)
        {
            Services.GameState.FreezeTime();
            Services.GameState.UnlockCursor();
        }
        else
        {
            Services.GameState.UnfreezeTime();
            Services.GameState.LockCursor();
        }
    }
}
}