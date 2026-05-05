namespace Vymesy.Enemies
{
    public enum EnemyType
    {
        /// <summary>Common вымес: standard fodder enemy.</summary>
        Common,
        /// <summary>Faster, frail вымес — applies pressure.</summary>
        Stalker,
        /// <summary>Tankier вымес that hits hard at close range.</summary>
        Brute,
        /// <summary>Ranged вымес that spits poison projectiles.</summary>
        Wretch,
        /// <summary>Elite вымес — buffed stats, more loot, drops Soul Shard.</summary>
        Elite,
        /// <summary>Rare shiny вымес — drops a guaranteed item.</summary>
        Shiny,
        /// <summary>Boss вымес — wave climax.</summary>
        Boss,
    }
}
