using System;
using System.Collections.Generic;

namespace Vymesy.Save
{
    /// <summary>
    /// Pure-list mirror of <see cref="PlayerData"/> so JsonUtility can round-trip the dictionary.
    /// </summary>
    [Serializable]
    public class SaveWrapper
    {
        public int Version;
        public int PlayerLevel;
        public int PlayerExp;
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
        public List<SerializedGem> Gems = new List<SerializedGem>();

        // Dictionary serialized as parallel arrays.
        public List<string> AzrarKeys = new List<string>();
        public List<int> AzrarValues = new List<int>();

        public static SaveWrapper From(PlayerData data)
        {
            var w = new SaveWrapper
            {
                Version = data.Version,
                PlayerLevel = data.PlayerLevel,
                PlayerExp = data.PlayerExp,
                Gold = data.Gold,
                MetaPoints = data.MetaPoints,
                SoulShards = data.SoulShards,
                RunsPlayed = data.RunsPlayed,
                RunsWon = data.RunsWon,
                HighestWave = data.HighestWave,
                UnlockedSkills = new List<string>(data.UnlockedSkills),
                UnlockedItems = new List<string>(data.UnlockedItems),
                UnlockedSystems = new List<string>(data.UnlockedSystems),
                UnlockedTreeNodes = new List<string>(data.UnlockedTreeNodes),
                UnlockedAchievements = new List<string>(data.UnlockedAchievements),
                Gems = new List<SerializedGem>(data.Gems),
            };
            foreach (var kv in data.AzrarLevels)
            {
                w.AzrarKeys.Add(kv.Key);
                w.AzrarValues.Add(kv.Value);
            }
            return w;
        }

        public void ApplyTo(PlayerData data)
        {
            data.Version = Version;
            data.PlayerLevel = PlayerLevel;
            data.PlayerExp = PlayerExp;
            data.Gold = Gold;
            data.MetaPoints = MetaPoints;
            data.SoulShards = SoulShards;
            data.RunsPlayed = RunsPlayed;
            data.RunsWon = RunsWon;
            data.HighestWave = HighestWave;
            data.UnlockedSkills = new List<string>(UnlockedSkills ?? new List<string>());
            data.UnlockedItems = new List<string>(UnlockedItems ?? new List<string>());
            data.UnlockedSystems = new List<string>(UnlockedSystems ?? new List<string>());
            data.UnlockedTreeNodes = new List<string>(UnlockedTreeNodes ?? new List<string>());
            data.UnlockedAchievements = new List<string>(UnlockedAchievements ?? new List<string>());
            data.Gems = new List<SerializedGem>(Gems ?? new List<SerializedGem>());
            data.AzrarLevels = new Dictionary<string, int>();
            int n = Math.Min(AzrarKeys?.Count ?? 0, AzrarValues?.Count ?? 0);
            for (int i = 0; i < n; i++) data.AzrarLevels[AzrarKeys[i]] = AzrarValues[i];
        }
    }
}
