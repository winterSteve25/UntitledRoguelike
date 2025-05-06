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

        [SerializeField] private float damage;
        [SerializeField] private int attackRadius;
        [SerializeField] private SpotSelectionMode mode;
        
        public async UniTask<bool> Perform(CombatManager combatManager, Unit unit, IAreaSelector areaSelector)
        {
            var pos = await areaSelector.SelectArea(unit.GridPositionSynchronized, unit.Type.Size, attackRadius,
                 p => combatManager.TryGetUnit(p.x, p.y, out _), mode);
            if (pos == null) return false;
            
            var u = combatManager.GetUnit(pos.Value.x, pos.Value.y);
            u.AddHp(-damage, DamageSource.Bludgeoning);
            return true;
        }
    }
}