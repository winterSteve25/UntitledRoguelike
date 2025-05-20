using Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Content.Explode
{
    public class ExplodeAbility : MonoBehaviour, IAbility
    {
        public string Name => "Explode";
        public int Cost => 1;
        public bool Blocking => false;
        public string Description => "Explode this unit at the start of the next friendly turn dealing 2 damage";
        public Sprite Icon => Resources.Load<Sprite>("Sprites/AbilityIcons/Explode");

        [SerializeField] private int radius;
        [SerializeField] private int damage;

        public async UniTask<bool> Perform(CombatManager combatManager, Unit unit, IAreaSelector areaSelector)
        {
            unit.Interactable = false;
            await IAbility.UntilNextFriendlyTurn(combatManager);
            Explode(combatManager, unit, radius, damage);
            return true;
        }

        public static void Explode(CombatManager combatManager, Unit unit, int radius, int damage)
        {
            var pos = unit.GridPositionSync;
            pos.x -= radius;
            pos.y -= radius;
            
            foreach (Unit u in combatManager.GetUnitsInArea(pos, new Vector2Int(radius * 2 + unit.Type.Size.x, radius * 2 + unit.Type.Size.y)))
            {
                if (u == unit) continue;
                u.AddHpRpc(-damage, DamageSource.Explosion);
            }
            
            combatManager.DespawnUnit(unit, true);
        }
    }
}