using Combat;
using Cysharp.Threading.Tasks;
using Levels;
using UnityEngine;

namespace Content.WhirlPool
{
    public class SpawnWhirlPoolAbility : MonoBehaviour, IAbility
    {
        public string Name => "Spawn Whirl Pool";
        public int Cost => 3;
        public bool Blocking => true;

        [SerializeField] private WhirlPoolGadget prefab;
        [SerializeField] private int spawnRadius;
        
        public async UniTask<bool> Perform(CombatManager combatManager, Unit unit, IAreaSelector areaSelector)
        {
            var area = await areaSelector.SelectArea(unit.GridPosition, unit.Type.Size, spawnRadius,
                p => Level.Current.InBounds(p, WhirlPoolGadget.Size));

            if (area == null) return false;
            var gadget = combatManager.SpawnGadget(prefab);
            gadget.Init(area.Value);

            return true;
        }
    }
}