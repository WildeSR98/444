using System.Collections.Generic;
using UnityEngine;
using Vymesy.Core;
using Vymesy.Player;
using Vymesy.Save;

namespace Vymesy.MetaTree
{
    public class TreeManager : MonoBehaviour
    {
        [SerializeField] private List<TreeNode> _allNodes = new List<TreeNode>();

        public void AddNode(TreeNode node)
        {
            if (node != null && !_allNodes.Contains(node)) _allNodes.Add(node);
        }

        public IReadOnlyList<TreeNode> AllNodes => _allNodes;

        public bool IsUnlocked(TreeNode node)
        {
            if (node == null) return false;
            var data = GameManager.HasInstance ? GameManager.Instance.PlayerData : null;
            return data != null && data.UnlockedTreeNodes.Contains(node.Id);
        }

        public bool CanUnlock(TreeNode node)
        {
            if (node == null) return false;
            var data = GameManager.HasInstance ? GameManager.Instance.PlayerData : null;
            if (data == null) return false;
            if (data.MetaPoints < node.Cost) return false;
            for (int i = 0; i < node.Prerequisites.Count; i++)
            {
                var pre = node.Prerequisites[i];
                if (pre != null && !data.UnlockedTreeNodes.Contains(pre.Id)) return false;
            }
            return !data.UnlockedTreeNodes.Contains(node.Id);
        }

        public bool UnlockNode(TreeNode node)
        {
            if (!CanUnlock(node)) return false;
            var data = GameManager.Instance.PlayerData;
            data.MetaPoints -= node.Cost;
            data.UnlockedTreeNodes.Add(node.Id);
            SaveLoadManager.Save(data);
            return true;
        }

        /// <summary>Apply all unlocked node modifiers to the live player. Call when a run starts.</summary>
        public void ApplyAllUnlocked()
        {
            var rm = GameManager.HasInstance ? GameManager.Instance.RunManager : null;
            if (rm == null || rm.Player == null) return;
            var data = GameManager.Instance.PlayerData;
            for (int i = 0; i < _allNodes.Count; i++)
            {
                var node = _allNodes[i];
                if (node == null) continue;
                if (!data.UnlockedTreeNodes.Contains(node.Id)) continue;
                rm.Player.AddModifier(node.Modifier);
            }
        }
    }
}
