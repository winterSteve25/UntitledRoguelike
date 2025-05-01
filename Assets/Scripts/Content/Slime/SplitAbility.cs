using Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Content.Slime
{
    public class SplitAbility : MonoBehaviour, IAbility
    {
        public string Name => "Split";
        public int Cost => 1;

        public async UniTaskVoid Perform(CombatManager combatManager, Unit unit, IAreaSelector areaSelector)
        {
            unit.Interactable = false;
            await IAbility.UntilNextFriendlyTurn(combatManager);
            combatManager.RemoveUnit(unit);

            var smallSlime = Resources.Load<UnitType>("UnitTypes/Slime Small");
            
            combatManager.SpawnUnit(
                smallSlime, new Vector2Int(unit.GridPosition.x, unit.GridPosition.y), unit.Friendly);
            combatManager.SpawnUnit(
                smallSlime, new Vector2Int(unit.GridPosition.x + 1, unit.GridPosition.y), unit.Friendly);
            combatManager.SpawnUnit(
                smallSlime, new Vector2Int(unit.GridPosition.x, unit.GridPosition.y + 1), unit.Friendly);
            combatManager.SpawnUnit(
                smallSlime, new Vector2Int(unit.GridPosition.x + 1, unit.GridPosition.y + 1), unit.Friendly);
        }
    }
}