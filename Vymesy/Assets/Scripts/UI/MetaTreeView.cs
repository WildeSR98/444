using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vymesy.Core;
using Vymesy.MetaTree;
using Vymesy.Save;

namespace Vymesy.UI
{
    /// <summary>
    /// Minimal meta-tree UI: a button per node, click to unlock.
    /// </summary>
    public class MetaTreeView : MonoBehaviour
    {
        [SerializeField] private TreeManager _tree;
        [SerializeField] private GameObject _nodeButtonPrefab;
        [SerializeField] private Transform _nodeRoot;
        [SerializeField] private TMP_Text _metaPointsLabel;

        private readonly List<NodeButton> _buttons = new List<NodeButton>();

        private struct NodeButton
        {
            public TreeNode Node;
            public Button Button;
            public TMP_Text Label;
            public Image Frame;
        }

        private void Start()
        {
            if (_tree == null || _nodeButtonPrefab == null || _nodeRoot == null) return;
            foreach (var node in _tree.AllNodes)
            {
                var go = Instantiate(_nodeButtonPrefab, _nodeRoot);
                var button = go.GetComponent<Button>();
                var label = go.GetComponentInChildren<TMP_Text>();
                var frame = go.GetComponent<Image>();
                var captured = node;
                if (button != null) button.onClick.AddListener(() => TryUnlock(captured));
                _buttons.Add(new NodeButton { Node = node, Button = button, Label = label, Frame = frame });
            }
            Refresh();
        }

        private void TryUnlock(TreeNode node)
        {
            if (_tree.UnlockNode(node)) Refresh();
        }

        public void Refresh()
        {
            if (GameManager.HasInstance && _metaPointsLabel != null)
                _metaPointsLabel.text = $"Очки: {GameManager.Instance.PlayerData.MetaPoints}";

            foreach (var b in _buttons)
            {
                if (b.Node == null) continue;
                bool unlocked = _tree.IsUnlocked(b.Node);
                bool canUnlock = !unlocked && _tree.CanUnlock(b.Node);
                if (b.Label != null) b.Label.text = $"{b.Node.DisplayName}\n{(unlocked ? "✓" : b.Node.Cost.ToString())}";
                if (b.Button != null) b.Button.interactable = canUnlock;
                if (b.Frame != null) b.Frame.color = unlocked ? new Color(0.4f, 1f, 0.4f) : (canUnlock ? Color.white : new Color(0.4f, 0.4f, 0.4f));
            }
        }
    }
}
