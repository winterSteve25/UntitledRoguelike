using Combat;
using Cysharp.Threading.Tasks;
using Levels;
using UnityEngine;

namespace Content.WhirlPool
{
    public class SpawnWhirlPoolAbility : MonoBehaviour, IAbility
    {
        public string Name => "Whirl Pool";
        public int Cost => 3;
        public bool Blocking => true;
        public string Description =>
            "Spawns a whirl pool that sucks nearby units into the center, then disperses all units randomly onto the board";

        [SerializeField] private WhirlPoolGadget prefab;
        [SerializeField] private int spawnRadius;

        public async UniTask<bool> Perform(CombatManager combatManager, Unit unit, IAreaSelector areaSelector)
        {
            var area = await areaSelector.SelectArea(unit.GridPositionSync, unit.Type.Size, spawnRadius,
                p => Level.Current.InBounds(p, WhirlPoolGadget.Size, !combatManager.AmIFriendly));

            if (area == null) return false;

            combatManager.SpawnGadget(prefab, area.Value);

            return true;
        }
    }
}