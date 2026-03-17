namespace AniDrag.Core
{
    public interface ISceneService
    {
        void LoadScene(int buildIndex);
        void ReloadCurrentScene();
        void QuitGame();
    }
}