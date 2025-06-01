using Combat;
using UnityEngine;

namespace Content.Explode
{
    public class ExplodeOnDeathPassive : MonoBehaviour, IPassive
    {
        [SerializeField] private ExplodeAbility ability;
        
        public void OnSpawned(Unit unit)
        {
            unit.OnDeath += OnUnitDeath;
        }

        private void OnUnitDeath(Unit unit, CancelToken cancel)
        {
            ability.Explode(CombatManager.Current, unit);
        }
    }
}