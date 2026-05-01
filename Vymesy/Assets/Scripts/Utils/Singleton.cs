using UnityEngine;

namespace Vymesy.Utils
{
    /// <summary>
    /// Generic MonoBehaviour singleton. Use only for true game-wide managers.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _shuttingDown;

        public static T Instance
        {
            get
            {
                if (_shuttingDown) return null;
                lock (_lock)
                {
                    if (_instance != null) return _instance;
                    _instance = FindExisting();
                    if (_instance != null) return _instance;
                    var go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go);
                    return _instance;
                }
            }
        }

        public static bool HasInstance => _instance != null && !_shuttingDown;

        private static T FindExisting()
        {
#if UNITY_2023_1_OR_NEWER
            return FindFirstObjectByType<T>(FindObjectsInactive.Include);
#else
            return FindObjectOfType<T>();
#endif
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = (T)this;
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
            OnAwake();
        }

        protected virtual void OnAwake() { }

        protected virtual void OnApplicationQuit() => _shuttingDown = true;
        protected virtual void OnDestroy()
        {
            if (_instance == this) _shuttingDown = true;
        }
    }
}
