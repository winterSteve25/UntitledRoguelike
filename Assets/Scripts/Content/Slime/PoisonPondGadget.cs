using Combat;
using UnityEngine;

namespace Content.Slime
{
    public class PoisonPondGadget : Gadget
    {
        [SerializeField] private float damage;

        public override Vector2Int GridSize => Vector2Int.one;

        protected override void OnTurn(bool friendlyTurn)
        {
            var cm = CombatManager.Current;
            if (!cm.TryGetUnit(GridPositionSync.x, GridPositionSync.y, out var unit)) return;
            unit.AddHpRpc(-damage, DamageSource.Poison);
        }
    }
}