using Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Content.Slime
{
    public class SplitAbility : MonoBehaviour, IAbility
    {
        [SerializeField] private UnitType smallSlime;
        
        public string Name => "Split";
        public int Cost => 1;
        public bool Blocking => false;

        public async UniTask<bool> Perform(CombatManager combatManager, Unit unit, IAreaSelector areaSelector)
        {
            unit.Interactable = false;
            await IAbility.UntilNextFriendlyTurn(combatManager);
            combatManager.DespawnUnit(unit);

            combatManager.SpawnUnit(
                smallSlime, new Vector2Int(unit.GridPositionSync.x, unit.GridPositionSync.y), unit.Friendly);
            combatManager.SpawnUnit(
                smallSlime, new Vector2Int(unit.GridPositionSync.x + 1, unit.GridPositionSync.y), unit.Friendly);
            combatManager.SpawnUnit(
                smallSlime, new Vector2Int(unit.GridPositionSync.x, unit.GridPositionSync.y + 1), unit.Friendly);
            combatManager.SpawnUnit(
                smallSlime, new Vector2Int(unit.GridPositionSync.x + 1, unit.GridPositionSync.y + 1), unit.Friendly);

            return true;
        }
    }
}