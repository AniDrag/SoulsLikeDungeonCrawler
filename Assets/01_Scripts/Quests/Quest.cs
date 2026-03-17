using System;
using System.Collections.Generic;
using UnityEngine;

namespace AniDrag.Quest
{
    [CreateAssetMenu(menuName = "AniDrag/Quests/Quest", fileName = "Quest")]
    public class Quest : ScriptableObject
    {
        public string questID;
        public string questName;
        public string description;
        public List<QuestObjective> objectives;

        [Header("Rewards")]
        public List<QuestReward> rewards;

        [Header("Chain")]
        public Quest nextQuest; // optional follow‑up quest
        
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(questID))
                questID = questName + Guid.NewGuid();
        }
    }
}