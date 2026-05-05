using UnityEngine;
using Vymesy.Core;
using Vymesy.Inventory;
using Vymesy.Player;
using Vymesy.Pooling;

namespace Vymesy.Items
{
    /// <summary>
    /// World-space item drop. Magnetizes to the player when within pickup radius.
    /// </summary>
    public class PickupItem : MonoBehaviour, IPooled
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private float _attractSpeed = 12f;
        [SerializeField] private float _pickupSqrRadius = 0.04f;

        private ItemData _item;
        private string _poolKey;
        private System.Action<string, GameObject> _returnToPool;

        public void Configure(ItemData item, string poolKey, System.Action<string, GameObject> returnToPool)
        {
            _item = item;
            _poolKey = poolKey;
            _returnToPool = returnToPool;
            if (_renderer != null && item != null)
            {
                _renderer.sprite = item.Icon;
                _renderer.color = ItemRarityColors.Color(item.Rarity);
            }
        }

        public void OnSpawnedFromPool() { }
        public void OnReturnedToPool() { _item = null; }

        private void Update()
        {
            if (!GameManager.HasInstance) return;
            var rm = GameManager.Instance.RunManager;
            if (rm == null || rm.Player == null) return;
            var player = rm.Player;
            Vector2 toPlayer = (Vector2)player.transform.position - (Vector2)transform.position;
            float pickup = player.Stats != null ? player.Stats.PickupRadius : 1.5f;
            if (toPlayer.sqrMagnitude <= pickup * pickup)
            {
                Vector2 step = toPlayer.normalized * _attractSpeed * Time.deltaTime;
                if (step.sqrMagnitude >= toPlayer.sqrMagnitude)
                {
                    rm.Inventory?.TryPickUp(_item);
                    _returnToPool?.Invoke(_poolKey, gameObject);
                    return;
                }
                transform.position += (Vector3)step;
            }
        }
    }
}
