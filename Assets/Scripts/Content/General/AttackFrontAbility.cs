using Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Content.General
{
    public class AttackFrontAbility : MonoBehaviour, IAbility
    {
        public string Name => "Attack";
        public int Cost => 1;
        public bool Blocking => true;
        public string Description => $"Attacks the enemy in front for {damage} damage";
        public Sprite Icon => Resources.Load<Sprite>("Sprites/AbilityIcons/Attack");

        [SerializeField] private float damage;
        [SerializeField] private int attackRadius;
        [SerializeField] private SpotSelectionMode mode;

        public async UniTask<bool> Perform(CombatManager combatManager, Unit unit, IAreaSelector areaSelector)
        {
            var pos = await areaSelector.SelectArea(
                unit.GridPositionSync,
                unit.Type.Size,
                Vector2Int.one,
                attackRadius,
                p => combatManager.TryGetUnit(p.x, p.y, out var u) && u != unit,
                mode
            );
            
            if (pos == null) return false;
            
            var u = combatManager.GetUnit(pos.Value.x, pos.Value.y);
            u.AddHpRpc(-damage, DamageSource.Bludgeoning);
            return true;
        }
    }
}