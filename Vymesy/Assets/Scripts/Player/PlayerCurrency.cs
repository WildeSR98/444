using System.Collections.Generic;
using Vymesy.Utils;

namespace Vymesy.Player
{
    public class PlayerCurrency
    {
        private readonly Dictionary<CurrencyType, int> _amounts = new Dictionary<CurrencyType, int>();

        public int Get(CurrencyType type) => _amounts.TryGetValue(type, out var v) ? v : 0;

        public void Add(CurrencyType type, int amount)
        {
            if (amount == 0) return;
            int current = Get(type);
            int next = current + amount;
            if (next < 0) next = 0;
            _amounts[type] = next;
            EventBus.Publish(new CurrencyChangedEvent(type, next));
        }

        public bool TrySpend(CurrencyType type, int amount)
        {
            if (amount <= 0) return true;
            int current = Get(type);
            if (current < amount) return false;
            _amounts[type] = current - amount;
            EventBus.Publish(new CurrencyChangedEvent(type, _amounts[type]));
            return true;
        }

        public void Reset()
        {
            _amounts.Clear();
        }
    }
}
