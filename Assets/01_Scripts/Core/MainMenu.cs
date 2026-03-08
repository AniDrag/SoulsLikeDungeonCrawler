using UnityEngine;
using UnityEngine.UI;
namespace AniDrag.Core
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] Button Play;
        [SerializeField] Button AI;
        [SerializeField] Button Item;
        [SerializeField] Button Quit;

        void Start()
        {
            if (Play != null)
                Play.onClick.AddListener(OnPlayClicked);

            if (AI != null)
                AI.onClick.AddListener(OnAIClicked);

            if (Item != null)
                Item.onClick.AddListener(OnItemClicked);

            if (Quit != null)
                Quit.onClick.AddListener(OnQuitClicked);
        }

        void OnPlayClicked()
        {
            Scene_Manager.Instance?.SCENE_LoadScene(1);
        }

        void OnAIClicked()
        {
            Scene_Manager.Instance?.SCENE_LoadScene(2);
        }

        void OnItemClicked()
        {
            Scene_Manager.Instance?.SCENE_LoadScene(3);
        }

        void OnQuitClicked()
        {
            Scene_Manager.Instance?.SCENE_QuitGame();
        }
    }
}