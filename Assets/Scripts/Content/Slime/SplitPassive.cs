using Combat;
using UnityEngine;

namespace Content.Slime
{
    public class SplitPassive : MonoBehaviour, IPassive
    {
        [SerializeField] private UnitType smallSlime;
        
        public void OnSpawned(Unit unit)
        {
            unit.OnDeath += OnDeath;
        }
        
        private void OnDeath(Unit unit, CancelToken cancel)
        {
            var combatManager = CombatManager.Current;
            cancel.Canceled = true;
            combatManager.DespawnUnit(unit, false);
            
            SplitAbility.SpawnSmallSlimes(combatManager, unit, smallSlime);
        }
    }
}