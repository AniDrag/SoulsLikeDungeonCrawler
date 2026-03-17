using UnityEngine;
using AniDrag.Core;

namespace AniDrag.Inventory
{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "AniDrag/Items/Consumable")]
    public class ConsumableItem : Item
    {
        [Header("Consumable Effect")]
        public ConsumableEffectType effectType;
        public int effectValue; // positive = buff/heal, negative = debuff/damage
        public float effectDuration; // 0 for instant
        public bool isNegative = false; // optional

        public override bool Use(GameObject owner)
        {
            Debug.Log($"[ConsumableItem] {itemName} used by {owner.name}");
            var receiver = owner.GetComponent<IEffectReceiver>();
            if (receiver != null)
            {
                receiver.ApplyEffect(effectType, effectValue, effectDuration, owner);
                Debug.Log($"[ConsumableItem] Applied effect {effectType} to {owner.name}");
                return true;
            }
            else
            {
                Debug.LogError($"[ConsumableItem] {owner.name} does not implement IEffectReceiver!");
                return false;
            }
        }
        public override string GetTooltipText()
        {
            string text = base.GetTooltipText();
            text += $"\n\n<b>Consumable</b>\n";
            text += $"Name: {itemName}\n";
            text += $"Rarity: {rarity.ToString()}\n";
            text += $"<b>Effect type</b>";
            text += $"{effectType}";
            if (effectValue != 0) text += $": {(effectValue > 0 ? "+" : "")}{effectValue}";
            if (effectDuration > 0) text += $" over {effectDuration}s";
            if (isNegative) text += " (Harmful)";
            text += $"Description:\n {description}\n";
            text += $"Value: {baseValue} \n ";
            text += $"Weight: {weight} \n ";
            return text;
        }
    }
}