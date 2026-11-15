using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FishSlop
{
    public class PlayerHUDLoader : NetworkBehaviour
    {
        [SerializeField] private string uiSceneName = "GameplayUI";
        [SerializeField] private string pauseSceneName = "PauseUI";

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public override void OnStartLocalPlayer()
        {
            LoadUI();
        }

        private void LoadUI()
        {
            if (!SceneManager.GetSceneByName(uiSceneName).isLoaded)
            {
                SceneManager.LoadSceneAsync(uiSceneName, LoadSceneMode.Additive);
                SceneManager.LoadSceneAsync(pauseSceneName, LoadSceneMode.Additive);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == uiSceneName) Debug.Log("Gameplay UI loaded!");
            if (scene.name == pauseSceneName) Debug.Log("Pause UI loaded!");
        }
    }
}