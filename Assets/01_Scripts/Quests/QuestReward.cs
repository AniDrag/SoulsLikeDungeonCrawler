using AniDrag.Core;

namespace AniDrag.Quest
{
    [System.Serializable]
    public class QuestReward
    {
        public RewardType type;
        public int amount;          // for XP, gold, etc.
        public Item item;            // for item rewards
        // Could add more fields (e.g., spell unlock, reputation)
    }
}