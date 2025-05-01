using Combat;
using UnityEngine;

namespace Content.Slime
{
    public class SplitPassive : MonoBehaviour, IPassive
    {
        [SerializeField] private UnitType smallSlime;
        
        public void OnSpawned(Unit unit)
        {
            unit.OnDeath += UnitOnOnDeath;
        }
        
        private void UnitOnOnDeath(Unit unit)
        {
            var combatManager = CombatManager.Current;
            
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