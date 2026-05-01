using UnityEngine;
using Vymesy.Save;
using Vymesy.Utils;

namespace Vymesy.Core
{
    /// <summary>
    /// Top-level entry point. Boots persistent services and exposes references to other managers.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private RunManager _runManagerPrefab;
        [SerializeField] private bool _autoStartRun = false;

        public RunManager RunManager { get; private set; }
        public PlayerData PlayerData { get; private set; }

        protected override void OnAwake()
        {
            PlayerData = new PlayerData();
            SaveLoadManager.Load(PlayerData);

            if (_runManagerPrefab != null)
            {
                RunManager = Instantiate(_runManagerPrefab, transform);
            }
            else
            {
                var go = new GameObject(nameof(RunManager));
                go.transform.SetParent(transform, false);
                RunManager = go.AddComponent<RunManager>();
            }

            if (_autoStartRun) RunManager.StartRun();
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            if (PlayerData != null) SaveLoadManager.Save(PlayerData);
        }
    }
}
