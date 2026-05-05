using System.Collections.Generic;
using UnityEngine;
using Vymesy.Core;

namespace Vymesy.Towers
{
    public class TowersManager : MonoBehaviour
    {
        [System.Serializable]
        public class TowerEntry
        {
            public TowerDefinition Definition;
            [Tooltip("Probability weight for random spawns or drops.")]
            public int Weight = 10;
        }

        [SerializeField] private List<TowerEntry> _entries = new List<TowerEntry>();
        [SerializeField] private int _maxTowers = 8;

        public void AddCatalogEntry(TowerDefinition def, int weight = 10)
        {
            if (def == null) return;
            _entries.Add(new TowerEntry { Definition = def, Weight = weight });
        }
        public void SetMaxTowers(int max) => _maxTowers = Mathf.Max(1, max);

        private readonly List<TowerBase> _alive = new List<TowerBase>();

        public IReadOnlyList<TowerBase> AliveTowers => _alive;
        public IReadOnlyList<TowerEntry> Catalog => _entries;
        public int MaxTowers => _maxTowers;

        public void BeginRun() { /* runs cleared in EndRun */ }

        public void EndRun()
        {
            for (int i = 0; i < _alive.Count; i++)
            {
                if (_alive[i] != null) Destroy(_alive[i].gameObject);
            }
            _alive.Clear();
        }

        public TowerBase SpawnTower(TowerDefinition def, Vector3 position)
        {
            if (def == null || def.Prefab == null) return null;
            if (_alive.Count >= _maxTowers) return null;
            var rm = Core.GameManager.HasInstance ? GameManager.Instance.RunManager : null;
            if (rm == null) return null;

            var go = Instantiate(def.Prefab, position, Quaternion.identity, transform);
            go.SetActive(true);
            var tower = go.GetComponent<TowerBase>();
            if (tower == null)
            {
                Debug.LogWarning($"[TowersManager] Tower prefab for {def.name} has no TowerBase component.");
                Destroy(go);
                return null;
            }
            tower.Configure(def, rm.Enemies, rm.Player);
            _alive.Add(tower);
            return tower;
        }
    }
}
