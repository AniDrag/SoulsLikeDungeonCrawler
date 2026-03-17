using System;
using AniDrag.Quest;

namespace AniDrag.Quest
{
    [System.Serializable]
    public class QuestObjective
    {
        public string objectiveID;
        public string description;
        public ObjectiveType type;
        public int requiredAmount;
        public int currentAmount;
        public string targetName; // e.g., enemy name, item name

        public bool IsCompleted => currentAmount >= requiredAmount;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(objectiveID))
                objectiveID = description + Guid.NewGuid();
        }
    }
}