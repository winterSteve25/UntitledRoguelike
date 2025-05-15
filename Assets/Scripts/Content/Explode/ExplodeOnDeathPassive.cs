using Combat;
using UnityEngine;

namespace Content.Explode
{
    public class ExplodeOnDeathPassive : MonoBehaviour, IPassive
    {
        [SerializeField] private int radius;
        [SerializeField] private int damage;
        
        public void OnSpawned(Unit unit)
        {
            unit.OnDeath += OnUnitDeath;
        }

        private void OnUnitDeath(Unit obj)
        {
            ExplodeAbility.Explode(CombatManager.Current, obj, radius, damage);
        }
    }
}