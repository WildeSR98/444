namespace Vymesy.Player
{
    public enum CurrencyType
    {
        Gold,        // earned and spent within a single run
        RunPoints,   // earned in a run, converts to MetaPoints on run end
        MetaPoints,  // persistent meta currency for the skill tree
        SoulShards,  // rare currency dropped by elites/bosses
    }
}
