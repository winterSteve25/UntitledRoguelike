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
        public string Description => "Splits the big slime into 4 smaller slimes";
        public Sprite Icon => Resources.Load<Sprite>("Sprites/AbilityIcons/Split");

        public async UniTask<bool> Perform(CombatManager combatManager, Unit unit, IAreaSelector areaSelector)
        {
            unit.Interactable = false;
            await IAbility.UntilNextFriendlyTurn(combatManager);
            combatManager.DespawnUnit(unit, false);
            SpawnSmallSlimes(combatManager, unit, smallSlime);
            return true;
        }

        public static void SpawnSmallSlimes(CombatManager combatManager, Unit unit, UnitType unitType)
        {
            // UniTask.Void(async () =>
            // {
            //     await UniTask.NextFrame();
            combatManager.SpawnUnit(
                unitType, new Vector2Int(unit.GridPositionSync.x, unit.GridPositionSync.y), unit.Friendly);
            combatManager.SpawnUnit(
                unitType, new Vector2Int(unit.GridPositionSync.x + 1, unit.GridPositionSync.y), unit.Friendly);
            combatManager.SpawnUnit(
                unitType, new Vector2Int(unit.GridPositionSync.x, unit.GridPositionSync.y + 1), unit.Friendly);
            combatManager.SpawnUnit(
                unitType, new Vector2Int(unit.GridPositionSync.x + 1, unit.GridPositionSync.y + 1), unit.Friendly);
            // });
        }
    }
}