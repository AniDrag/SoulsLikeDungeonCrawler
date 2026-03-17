using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AniDrag.Quest
{
    public class QuestUIExample : MonoBehaviour
    {
        [SerializeField] private Transform questListContent;
        [SerializeField] private GameObject questEntryPrefab;
        [SerializeField] private GameObject objectiveTextPrefab;

        private QuestManager _playerQuestManager;
        private Dictionary<QuestProgress, GameObject> _entryByProgress = new();

        private void Start()
        {
            
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                _playerQuestManager = player.GetComponent<QuestManager>();

            if (_playerQuestManager == null)
            {
                Debug.LogError("QuestUIExample: No QuestManager found on player.");
                return;
            }
            if (_playerQuestManager != null)
                _playerQuestManager.OnQuestAdded += AddQuestToUI;
        }

        public void AddQuestToUI(QuestProgress progress)
        {
            if (_entryByProgress.ContainsKey(progress)) return;

            GameObject entry = Instantiate(questEntryPrefab, questListContent);
            _entryByProgress[progress] = entry;

            TMP_Text questNameText = entry.transform.Find("QuestNameText").GetComponent<TMP_Text>();
            questNameText.text = progress.Quest.questName;

            Transform objectiveList = entry.transform.Find("ObjectiveList");
            foreach (var objective in progress.Objectives)
            {
                GameObject go = Instantiate(objectiveTextPrefab, objectiveList);
                go.name = objective.objectiveID;
            }

            UpdateAllObjectives(entry, progress);
            progress.OnUpdated += HandleQuestUpdated;
        }

        private void UpdateAllObjectives(GameObject entry, QuestProgress progress)
        {
            Transform objectiveList = entry.transform.Find("ObjectiveList");
            foreach (var objective in progress.Objectives)
            {
                Transform child = objectiveList.Find(objective.objectiveID);
                if (child == null) continue;

                TMP_Text objText = child.GetComponent<TMP_Text>();
                if (!objective.IsCompleted)
                {
                    objText.text = $"{objective.description} ({objective.currentAmount} / {objective.requiredAmount})";
                }
                else
                {
                    objText.text = $"{objective.description} DONE";
                    objText.color = Color.green;
                }
            }
        }

        private void HandleQuestUpdated(QuestProgress progress)
        {
            if (_entryByProgress.TryGetValue(progress, out var entry))
            {
                UpdateAllObjectives(entry, progress);
                if (progress.IsCompleted)
                {
                    
                    TMP_Text questNameText = entry.transform.Find("QuestNameText").GetComponent<TMP_Text>();
                    questNameText.text = progress.Quest.questName + " DONE";
                    questNameText.color = Color.green;
                }
            }
        }

        private void OnDestroy()
        {
            if (_playerQuestManager != null)
                _playerQuestManager.OnQuestAdded -= AddQuestToUI;
            
            foreach (var kvp in _entryByProgress)
            {
                kvp.Key.OnUpdated -= HandleQuestUpdated;
            }
            _entryByProgress.Clear();
        }
    }
}