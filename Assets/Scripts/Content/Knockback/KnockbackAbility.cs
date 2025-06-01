using Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Content.Knockback
{
    public class KnockbackAbility : MonoBehaviour, IAbility
    {
        public string Name => "Shove";
        public int Cost => 2;
        public bool Blocking => true;
        public string Description => "Push someone";
        public Sprite Icon { get; }

        [SerializeField] private int radius;

        public async UniTask<bool> Perform(CombatManager combatManager, Unit unit, IAreaSelector areaSelector)
        {
            var pos = await areaSelector.SelectArea(
                unit.GridPositionSync,
                unit.Type.Size,
                Vector2Int.one,
                radius,
                p => combatManager.TryGetUnit(p.x, p.y, out var u) && u != unit,
                SpotSelectionMode.Straight
            );
            
            if (pos == null) return false;
            
            var u = combatManager.GetUnit(pos.Value.x, pos.Value.y);
            
            // todo
            
            return true;
        }
    }
}