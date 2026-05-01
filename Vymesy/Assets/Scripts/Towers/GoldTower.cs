using Vymesy.Player;

namespace Vymesy.Towers
{
    /// <summary>
    /// Generates gold for the player each tick.
    /// </summary>
    public class GoldTower : TowerBase
    {
        protected override void Tick()
        {
            if (Player == null) return;
            Player.AddCurrency(CurrencyType.Gold, Definition.GoldPerTick);
        }
    }
}
