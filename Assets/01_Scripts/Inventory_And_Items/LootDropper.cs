using System.Collections.Generic;
using UnityEngine;
namespace AniDrag.InventoryAndItems
{
    /// <summary>
    /// This is on the player, when the player dies, this script will spawn the loot prefab on the player position, and then destroy itself.
    /// It is also on all enemies and it will collect enemy loop drop data. It has a button to get items that the Entity is wearing and has in inventory to drop it.
    /// </summary>
    public class LootDropper : MonoBehaviour
    {
        [SerializeField] private GameObject lootPrefab;
        [SerializeField] private Transform dropZone;

        [field:SerializeField] public List<ItemStack> DropableItems { get; private set; } = new List<ItemStack>();
        public void DroopLoot()
        {
            Debug.Log("I DROPED LOOT");
                if (DropableItems.Count > 0)
                {
                    GameObject loot = Instantiate(lootPrefab, dropZone.position, Quaternion.identity);
                    PhysicalItemInstance lootInstance = loot.GetComponent<PhysicalItemInstance>();
                    lootInstance.SetInstanceData(DropableItems);
                }
            Destroy(this.gameObject);
        }
    }
}
