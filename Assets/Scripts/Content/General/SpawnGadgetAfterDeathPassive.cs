using Combat;
using UnityEngine;

namespace Content.Slime
{
    public class LeavePoisonPondPassive : MonoBehaviour, IPassive
    {
        [SerializeField] private Gadget prefab;
        [SerializeField] private int duration;
        
        public void OnSpawned(Unit unit)
        {
            unit.OnDeath += UnitOnOnDeath;
        }
        
        private void UnitOnOnDeath(Unit unit)
        {
            var gadget = CombatManager.Current.SpawnGadget(prefab);
            gadget.Init(duration, unit.GridPosition);
        }
    }
}