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
            combatManager.DespawnUnit(unit);
            for (int i = -radius; i < radius; i++)
            {
                for (int j = -radius; j < radius; j++)
                {
                    if (!combatManager.TryGetUnit(unit.GridPositionSync.x + i, unit.GridPositionSync.y + j, out var u)) continue;
                    if (u == unit) continue;
                    u.AddHpRpc(-damage, DamageSource.Explosion);
                }
            }
        }
    }
}