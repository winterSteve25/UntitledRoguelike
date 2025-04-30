using Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Content
{
    public class MovementAbility : MonoBehaviour, IAbility
    {
        public string Name => "Move";
        public int Cost => 1;

        [SerializeField] private int movementRadius;
        [SerializeField] private SpotSelectionMode movementMode;

        public async UniTaskVoid Perform(CombatManager combatManager, Unit unit)
        {
            var area = await AreaSelector.Current.SelectArea(unit.GridPosition, unit.Type.Size, movementRadius,
                movementMode);

            if (!combatManager.CanPlaceUnitAt(unit.Type, area))
            {
                Debug.Log("CANT GO THERE!");
                return;
            }
            
            unit.MoveTo(area);
        }
    }
}