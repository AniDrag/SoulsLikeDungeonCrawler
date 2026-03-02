using AniDrag.Core;
using UnityEngine;

namespace AniDrag.InventoryAndItems
{
    [CreateAssetMenu(fileName = "HealthPotion", menuName = "AniDrag/Items/Consumables/HealthPotion")]
    public class HealthPotion : Item
    {
        public override bool Use(GameObject owner)
        {
            var receiver = owner.GetComponent<IEffectReceiver>();
            if (receiver != null)
            {
                receiver.ApplyEffect(effect, effectValue, effectTime);
                return true;
            }
            return false;
        }
    }
}