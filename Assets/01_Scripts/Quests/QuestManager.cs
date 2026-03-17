using System.Collections.Generic;
using AniDrag.Core;
using UnityEngine;

namespace AniDrag.Quest
{
    public class QuestManager : MonoBehaviour
    {
        private List<QuestProgress> _activeQuests = new();
        public event System.Action<QuestProgress> OnQuestAdded;
        private void OnEnable()
        {
            Services.EventBus.Subscribe<DeathEvent>(OnDeathEvent);
            Services.EventBus.Subscribe<ItemPickedEvent>(OnItemPickedEvent);
        }

        private void OnDisable()
        {
            // Only unsubscribe if the service still exists
            if (Services.EventBus != null)
            {
                Services.EventBus.Unsubscribe<DeathEvent>(OnDeathEvent);
                Services.EventBus.Unsubscribe<ItemPickedEvent>(OnItemPickedEvent);
            }
        }

        public void AddQuest(Quest quest)
        {
            var progress = new QuestProgress(quest);
            progress.OnUpdated += HandleQuestUpdated;
            _activeQuests.Add(progress);
            OnQuestAdded?.Invoke(progress);
            Debug.Log($"Quest started: {quest.questName}");
        }

        private void OnDeathEvent(DeathEvent ev)
        {
            // Only process if this player is involved (as killer)
            if (ev.Target != gameObject) return; // assuming killer is Target

            foreach (var quest in _activeQuests)
                quest.ProcessEvent(ev);
        }

        private void OnItemPickedEvent(ItemPickedEvent ev)
        {
            // Only process if this player picked up the item
            if (ev.Source != gameObject) return;

            foreach (var quest in _activeQuests)
                quest.ProcessEvent(ev);
        }

        private void HandleQuestUpdated(QuestProgress progress)
        {
            if (progress.IsCompleted)
                CompleteQuest(progress);
        }

        private void CompleteQuest(QuestProgress progress)
        {
            progress.OnUpdated -= HandleQuestUpdated;
            _activeQuests.Remove(progress);

            // Grant rewards
            foreach (var reward in progress.Quest.rewards)
            {
                GrantReward(reward);
            }

            // Start next quest if any
            if (progress.Quest.nextQuest != null)
            {
                AddQuest(progress.Quest.nextQuest);
            }
        }

        private void GrantReward(QuestReward reward)
        {
            switch (reward.type)
            {
                case RewardType.Experience:
                    var xp = GetComponent<IXp>();
                    xp?.GainXp(reward.amount);
                    break;
                case RewardType.Gold:
                    // Assuming a gold inventory on player
                    break;
                case RewardType.Item:
                    var inv = GetComponent<IInventoryHolder>();
                    inv?.AddItem(reward.item, reward.amount);
                    break;
                // ... other types
            }
        }
    }
}