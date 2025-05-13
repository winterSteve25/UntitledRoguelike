using Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Content.General
{
    public class MovementAbility : MonoBehaviour, IAbility
    {
        public string Name => "Move";
        public int Cost => 1;
        public bool Blocking => true;
        public string Description => "Moves the unit to a selected location within range";

        [field: SerializeField] public int MovementRadius { get; private set; }
        [field: SerializeField] public SpotSelectionMode MovementMode { get; private set; }

        public async UniTask<bool> Perform(CombatManager combatManager, Unit unit, IAreaSelector areaSelector)
        {
            var area = await areaSelector.SelectArea(
                unit.GridPositionSync,
                unit.Type.Size,
                unit.Type.Size,
                MovementRadius,
                p => combatManager.CanMoveTo(unit, p),
                MovementMode
            );
            
            if (area == null) return false;
            unit.MoveToRpc(area.Value);
            return true;
        }
    }
}