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

        void OnPlayClicked() => Services.Scene.LoadScene(1);
        void OnAIClicked() => Services.Scene.LoadScene(2);
        void OnItemClicked() => Services.Scene.LoadScene(3);
        void OnQuitClicked() => Services.Scene.QuitGame();
    }
}