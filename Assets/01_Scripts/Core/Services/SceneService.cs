using UnityEngine;
using UnityEngine.SceneManagement;

namespace AniDrag.Core
{
    public class SceneService : MonoBehaviour, ISceneService
    {
        public void LoadScene(int buildIndex)
        {
            SceneManager.LoadSceneAsync(buildIndex);
        }

        public void ReloadCurrentScene()
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadSceneAsync(currentIndex);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}