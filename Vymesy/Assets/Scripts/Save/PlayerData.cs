using System;
using System.Collections.Generic;

namespace Vymesy.Save
{
    /// <summary>
    /// Persistent player profile. Serialized as JSON via <see cref="SaveLoadManager"/>.
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public int Version = 1;
        public int PlayerLevel = 1;
        public int PlayerExp = 0;

        public int Gold;
        public int MetaPoints;
        public int SoulShards;

        public int RunsPlayed;
        public int RunsWon;
        public int HighestWave;

        public List<string> UnlockedSkills = new List<string>();
        public List<string> UnlockedItems = new List<string>();
        public List<string> UnlockedSystems = new List<string>();
        public List<string> UnlockedTreeNodes = new List<string>();
        public List<string> UnlockedAchievements = new List<string>();

        // Persisted gem inventory (4 slots).
        public List<SerializedGem> Gems = new List<SerializedGem>();

        public Dictionary<string, int> AzrarLevels = new Dictionary<string, int>();
    }

    [Serializable]
    public class SerializedGem
    {
        public string GemId;
        public int Level;
    }
}
