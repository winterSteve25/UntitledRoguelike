using Combat;
using UnityEngine;

namespace Content.General
{
    public class ImmuneToDamageType : MonoBehaviour, IPassive
    {
        [SerializeField] private DamageSource source;
        
        public void OnSpawned(Unit unit)
        {
            unit.OnHpChange += UnitOnOnHpChange;
        }

        private void UnitOnOnHpChange(Unit unit, int original, int current, DamageSource source, CancelToken cancel)
        {
            if (this.source == source) cancel.Canceled = true;
        }
    }
}