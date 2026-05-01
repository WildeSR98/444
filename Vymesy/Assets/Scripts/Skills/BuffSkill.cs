using UnityEngine;
using Vymesy.Player;

namespace Vymesy.Skills
{
    /// <summary>
    /// Temporary stat buff applied to the player when the skill triggers.
    /// </summary>
    [CreateAssetMenu(menuName = "Vymesy/Skills/Buff Skill", fileName = "BuffSkill")]
    public class BuffSkill : SkillBase
    {
        [Header("Buff")]
        public PlayerStatsModifier Modifier = new PlayerStatsModifier();
        public float Duration = 4f;

        public override void Trigger(SkillContext ctx)
        {
            PlayerManager pm = null;
            if (ctx.Source != null) pm = ctx.Source.GetComponentInParent<PlayerManager>();
#if UNITY_2023_1_OR_NEWER
            if (pm == null) pm = Object.FindFirstObjectByType<PlayerManager>();
#else
            if (pm == null) pm = Object.FindObjectOfType<PlayerManager>();
#endif
            if (pm == null) return;
            pm.AddModifier(Modifier);
            pm.StartCoroutine(RemoveAfter(pm, Modifier, Duration));
        }

        private static System.Collections.IEnumerator RemoveAfter(PlayerManager pm, PlayerStatsModifier mod, float duration)
        {
            yield return new WaitForSeconds(duration);
            if (pm != null) pm.RemoveModifier(mod);
        }
    }
}
