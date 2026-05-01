using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vymesy.Core;
using Vymesy.Utils;

namespace Vymesy.UI
{
    public class RunEndScreen : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _summaryText;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private string _menuSceneName = "MainMenu";

        private void Awake()
        {
            if (_root != null) _root.SetActive(false);
            if (_retryButton != null) _retryButton.onClick.AddListener(Retry);
            if (_menuButton != null) _menuButton.onClick.AddListener(BackToMenu);
        }

        private void OnEnable() => EventBus.Subscribe<RunEndedEvent>(Handle);
        private void OnDisable() => EventBus.Unsubscribe<RunEndedEvent>(Handle);

        private void Handle(RunEndedEvent evt)
        {
            if (_root != null) _root.SetActive(true);
            if (_titleText != null) _titleText.text = evt.Victory ? "ПОБЕДА" : "ВЫМЕСЫ ПОБЕДИЛИ";
            if (_summaryText != null && GameManager.HasInstance)
            {
                var rm = GameManager.Instance.RunManager;
                _summaryText.text = $"Волна: {rm.Wave}\nВремя: {Mathf.FloorToInt(rm.RunTime)}с\nОчки забега: {rm.RunPointsEarned}";
            }
        }

        private void Retry()
        {
            if (_root != null) _root.SetActive(false);
            if (GameManager.HasInstance) GameManager.Instance.RunManager.StartRun();
        }

        private void BackToMenu()
        {
            if (!string.IsNullOrEmpty(_menuSceneName)) SceneManager.LoadScene(_menuSceneName);
        }
    }
}
