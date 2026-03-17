using System;
using System.Collections.Generic;
using AniDrag.CharacterComponents;
using AniDrag.Core;

namespace AniDrag.Quest
{
    /// <summary>
    /// an active quest, that has progress stored inside
    /// </summary>
    public class QuestProgress
    {
        public Quest Quest { get; private set; }
        public List<QuestObjective> Objectives { get; private set; }
        public bool IsCompleted => Objectives.TrueForAll(o => o.IsCompleted);

        public event Action<QuestProgress> OnUpdated;

        public QuestProgress(Quest quest)
        {
            Quest = quest;
            Objectives = new List<QuestObjective>();
            foreach (var obj in quest.objectives)
            {
                Objectives.Add(new QuestObjective
                {
                    objectiveID = obj.objectiveID,
                    description = obj.description,
                    type = obj.type,
                    requiredAmount = obj.requiredAmount,
                    currentAmount = 0,
                    targetName = obj.targetName
                });
            }
        }

        public void ProcessEvent(BaseEvent ev)
        {
            if (IsCompleted) return;

            bool changed = false;

            switch (ev)
            {
                case DeathEvent de:
                    changed |= ProcessDeath(de);
                    break;
                case ItemPickedEvent ipe:
                    changed |= ProcessItemPickup(ipe);
                    break;
            }

            if (changed)
            {
                OnUpdated?.Invoke(this);
            }
        }

        private bool ProcessDeath(DeathEvent de)
        {
            bool anyChanged = false;
            foreach (var obj in Objectives)
            {
                if (obj.IsCompleted) continue;
                if (obj.type == ObjectiveType.DefeatEnemy)
                {
                    // Check if the killed entity matches the target name
                    Entity entity = de.Source?.GetComponent<Entity>();
                    if (entity != null && entity.EntityName == obj.targetName)
                    {
                        obj.currentAmount++;
                        anyChanged = true;
                    }
                }
            }
            return anyChanged;
        }

        private bool ProcessItemPickup(ItemPickedEvent ipe)
        {
            bool anyChanged = false;
            foreach (var obj in Objectives)
            {
                if (obj.IsCompleted) continue;
                if (obj.type == ObjectiveType.CollectItem && ipe.Item.itemName == obj.targetName)
                {
                    obj.currentAmount += ipe.Quantity;
                    anyChanged = true;
                }
            }
            return anyChanged;
        }
    }
}
