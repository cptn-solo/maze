using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class GameRunner: MonoBehaviour
    {
        private void Awake() =>
            DontDestroyOnLoad(this);

        private void Start() =>
            StartCoroutine(LoadLobbyScene());

        private IEnumerator LoadLobbyScene()
        {
            var op = SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);            
            op.allowSceneActivation = false;

            while (op.progress < .9f)
                yield return null;

            yield return new WaitForSecondsRealtime(3.0f);

            op.allowSceneActivation = true;

            while (!op.isDone)
                yield return null;

            LobbyOp_completed(op);
        }

        private void LobbyOp_completed(AsyncOperation op)
        {
            var lobbyScene = SceneManager.GetActiveScene();
            var rootObjects = lobbyScene.GetRootGameObjects();
            var gameMenu = rootObjects.Select(x => x.GetComponent<GameMenu>()).FirstOrDefault();

            if (gameMenu != default)
                gameMenu.OnLevelSelected += GameMenu_OnLevelSelected;
        }

        private void GameMenu_OnLevelSelected(int idx)
        {
            var levelSceneName = idx switch
            {                
                1 => "Level2",
                _ => "Level1"
            };

            SceneManager.LoadSceneAsync(levelSceneName, LoadSceneMode.Single)
                .completed += (op) => Debug.Log($"Level loaded: {levelSceneName}");
        }
    }
}