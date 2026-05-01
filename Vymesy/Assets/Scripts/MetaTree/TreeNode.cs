using System.Collections.Generic;
using UnityEngine;
using Vymesy.Player;

namespace Vymesy.MetaTree
{
    [CreateAssetMenu(fileName = "TreeNode", menuName = "Vymesy/Meta/Tree Node")]
    public class TreeNode : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        [TextArea] public string Description;
        public int Cost = 1;
        public Sprite Icon;
        public List<TreeNode> Prerequisites = new List<TreeNode>();
        public PlayerStatsModifier Modifier = new PlayerStatsModifier();
    }
}
