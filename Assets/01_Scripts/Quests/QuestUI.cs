using UnityEngine;
namespace AniDrag.Core
{
    public class QuestUI : MonoBehaviour
    {
        [SerializeField] private GameObject questPanel;

        private void Update()
        {
            if (Services.Input != null && Services.Input.QuestPressed) // Or a dedicated quest toggle
            {
                TogglePanel();
            }
        }

        private void TogglePanel()
        {
            if (questPanel == null) return;
            questPanel.SetActive(!questPanel.activeSelf);
        }
    }
}