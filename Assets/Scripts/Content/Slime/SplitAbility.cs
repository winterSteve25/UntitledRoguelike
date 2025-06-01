using Combat;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Content.Slime
{
    public class SplitAbility : NetworkBehaviour, IAbility
    {
        [SerializeField] private UnitType smallSlime;
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private SpriteRenderer mask;
        [SerializeField] private Sprite aboutTo;
        [SerializeField] private Sprite aboutToMask;

        public string Name => "Split";
        public int Cost => 1;
        public bool Blocking => false;
        public string Description => "Splits the big slime into 4 smaller slimes";
        public Sprite Icon => Resources.Load<Sprite>("Sprites/AbilityIcons/Split");

        public async UniTask<bool> Perform(CombatManager combatManager, Unit unit, IAreaSelector areaSelector)
        {
            unit.Interactable = false;
            ChangeTextureRpc();
            await IAbility.UntilNextFriendlyTurn(combatManager);
            combatManager.DespawnUnit(unit, false);
            SpawnSmallSlimes(combatManager, unit, smallSlime);
            return true;
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ChangeTextureRpc()
        {
            visual.sprite = aboutTo;
            mask.sprite = aboutToMask;
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