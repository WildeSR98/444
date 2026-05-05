using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vymesy.Core;

namespace Vymesy.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private string _gameSceneName = "Game";

        private void Awake()
        {
            if (_playButton != null) _playButton.onClick.AddListener(OnPlay);
            if (_quitButton != null) _quitButton.onClick.AddListener(OnQuit);
        }

        private void OnPlay()
        {
            if (!string.IsNullOrEmpty(_gameSceneName)) SceneManager.LoadScene(_gameSceneName);
            else if (GameManager.HasInstance) GameManager.Instance.RunManager.StartRun();
        }

        private void OnQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
