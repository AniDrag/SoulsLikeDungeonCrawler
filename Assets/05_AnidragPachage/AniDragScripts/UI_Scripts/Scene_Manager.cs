using UnityEngine;
using UnityEngine.SceneManagement;
namespace AniDrag.Core
{
    public class Scene_Manager : MonoBehaviour
    {
        public static Scene_Manager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        [Tooltip("Prepared for a loading screen")]
        public GameObject LoadSceneObj;
        public void SCENE_LoadScene(int sceneIndex)
        {
            //Add a safety Check
            SceneManager.LoadSceneAsync(sceneIndex);
            GameManager.Instance.SpawnPlayer();
        }
        public void SCENE_ReloadScene()
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadSceneAsync(index);
            GameManager.Instance.SpawnPlayer();
        }
        public void SCENE_QuitGame()
        {
            Application.Quit();
        }
    }
}