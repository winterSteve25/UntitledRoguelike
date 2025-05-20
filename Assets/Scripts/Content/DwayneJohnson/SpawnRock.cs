using Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Content.DwayneJohnson
{
    public class SpawnRock : MonoBehaviour, IAbility
    {
        public string Name => "Spawn Nut";
        public int Cost => 2;
        public bool Blocking => true;
        public string Description => "Bust a massive nut in front of you :)";
        public Sprite Icon => Resources.Load<Sprite>("Sprites/AbilityIcons/Rock");

        [SerializeField] private UnitType rockType;
        [SerializeField] private int radius;

        public async UniTask<bool> Perform(CombatManager combatManager, Unit unit, IAreaSelector areaSelector)
        {
            var area = await areaSelector.SelectArea(
                unit.GridPositionSync,
                unit.Type.Size,
                rockType.Size,
                radius,
                x => !combatManager.TryGetUnit(x.x, x.y, out _),
                SpotSelectionMode.Straight
            );

            if (area == null) return false;

            combatManager.SpawnUnit(rockType, area.Value, unit.Friendly);
            
            return true;
        }
    }
}