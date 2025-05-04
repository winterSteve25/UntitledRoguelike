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
            if (!cm.TryGetUnit(GridPosition.x, GridPosition.y, out var unit)) return;
            unit.AddHp(-damage, DamageSource.Poison);
        }
    }
}