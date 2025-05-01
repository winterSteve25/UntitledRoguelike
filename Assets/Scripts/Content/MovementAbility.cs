using Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Content
{
    public class MovementAbility : MonoBehaviour, IAbility
    {
        public string Name => "Move";
        public int Cost => 1;

        [field: SerializeField] public int MovementRadius { get; private set; }
        [field: SerializeField] public SpotSelectionMode MovementMode { get; private set; }

        public async UniTaskVoid Perform(CombatManager combatManager, Unit unit, IAreaSelector areaSelector)
        {
            var area = await areaSelector
                .SelectArea(unit.GridPosition, unit.Type.Size, MovementRadius,
                    p => combatManager.CanMoveTo(unit, p), MovementMode);

            unit.MoveTo(area);
        }
    }
}