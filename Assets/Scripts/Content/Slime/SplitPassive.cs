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
                smallSlime, new Vector2Int(unit.GridPositionSynchronized.x, unit.GridPositionSynchronized.y), unit.Friendly);
            combatManager.SpawnUnit(
                smallSlime, new Vector2Int(unit.GridPositionSynchronized.x + 1, unit.GridPositionSynchronized.y), unit.Friendly);
            combatManager.SpawnUnit(
                smallSlime, new Vector2Int(unit.GridPositionSynchronized.x, unit.GridPositionSynchronized.y + 1), unit.Friendly);
            combatManager.SpawnUnit(
                smallSlime, new Vector2Int(unit.GridPositionSynchronized.x + 1, unit.GridPositionSynchronized.y + 1), unit.Friendly);
        }
    }
}