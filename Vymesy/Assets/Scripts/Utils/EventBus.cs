using System;
using System.Collections.Generic;

namespace Vymesy.Utils
{
    /// <summary>
    /// Lightweight static event bus keyed by message type. No allocations on publish.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>(64);

        public static void Subscribe<T>(Action<T> handler)
        {
            if (handler == null) return;
            var t = typeof(T);
            _handlers.TryGetValue(t, out var existing);
            _handlers[t] = (Action<T>)existing + handler;
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            if (handler == null) return;
            var t = typeof(T);
            if (!_handlers.TryGetValue(t, out var existing)) return;
            var updated = (Action<T>)existing - handler;
            if (updated == null) _handlers.Remove(t);
            else _handlers[t] = updated;
        }

        public static void Publish<T>(T evt)
        {
            if (_handlers.TryGetValue(typeof(T), out var existing))
            {
                ((Action<T>)existing)?.Invoke(evt);
            }
        }

        public static void Clear() => _handlers.Clear();
    }

    public readonly struct PlayerDamagedEvent { public readonly float Amount; public PlayerDamagedEvent(float a) { Amount = a; } }
    public readonly struct PlayerDiedEvent { }
    public readonly struct EnemyKilledEvent
    {
        public readonly Enemies.EnemyType Type;
        public readonly UnityEngine.Vector3 Position;
        public readonly int GoldReward;
        public EnemyKilledEvent(Enemies.EnemyType t, UnityEngine.Vector3 p, int g) { Type = t; Position = p; GoldReward = g; }
    }
    public readonly struct WaveStartedEvent { public readonly int Wave; public WaveStartedEvent(int w) { Wave = w; } }
    public readonly struct RunStartedEvent { }
    public readonly struct RunEndedEvent { public readonly bool Victory; public RunEndedEvent(bool v) { Victory = v; } }
    public readonly struct CurrencyChangedEvent
    {
        public readonly Player.CurrencyType Type;
        public readonly int NewAmount;
        public CurrencyChangedEvent(Player.CurrencyType t, int n) { Type = t; NewAmount = n; }
    }
    public readonly struct ItemPickedUpEvent
    {
        public readonly Inventory.ItemData Item;
        public ItemPickedUpEvent(Inventory.ItemData i) { Item = i; }
    }
}
