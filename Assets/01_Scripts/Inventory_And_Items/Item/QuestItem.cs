using AniDrag.Core;
using UnityEngine;

namespace AniDrag.Inventory
{
    [CreateAssetMenu(fileName = "New Quest Item", menuName = "AniDrag/Items/Quest Item")]
    public class QuestItem : Item
    {
        // No need to override IsQuestItem – we use itemType.
        // Ensure in the Inspector you set itemType = Quest.

        public override bool Use(GameObject owner)
        {
            // Quest items cannot be used; return false so they aren't consumed.
            Debug.Log($"[QuestItem] {itemName} cannot be used.");
            return false;
        }

        public override string GetTooltipText()
        {
            string text = base.GetTooltipText();
            text += $"\n\n<b>Quest Item – cannot be removed</b>\n";
            text += $"Name: {itemName}\n";
            text += $"Rarity: {rarity.ToString()}\n";
            text += $"Description:\n {description}\n";
            return text;
           
        }
    }
}