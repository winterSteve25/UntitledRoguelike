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
                smallSlime, new Vector2Int(unit.GridPositionSync.x, unit.GridPositionSync.y), unit.Friendly);
            combatManager.SpawnUnit(
                smallSlime, new Vector2Int(unit.GridPositionSync.x + 1, unit.GridPositionSync.y), unit.Friendly);
            combatManager.SpawnUnit(
                smallSlime, new Vector2Int(unit.GridPositionSync.x, unit.GridPositionSync.y + 1), unit.Friendly);
            combatManager.SpawnUnit(
                smallSlime, new Vector2Int(unit.GridPositionSync.x + 1, unit.GridPositionSync.y + 1), unit.Friendly);
        }
    }
}