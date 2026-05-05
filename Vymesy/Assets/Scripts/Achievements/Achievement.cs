using UnityEngine;

namespace Vymesy.Achievements
{
    public enum AchievementType
    {
        EnemiesKilled,
        RunsWon,
        ReachWave,
        CollectGold,
    }

    [CreateAssetMenu(fileName = "Achievement", menuName = "Vymesy/Achievements/Achievement")]
    public class Achievement : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        [TextArea] public string Description;
        public AchievementType Type = AchievementType.EnemiesKilled;
        public int Threshold = 100;
        public int MetaPointsReward = 1;
    }
}
