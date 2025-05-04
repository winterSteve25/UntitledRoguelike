using Combat;
using UnityEngine;

namespace Content.General
{
    public class SpawnGadgetAfterDeathPassive : MonoBehaviour, IPassive
    {
        [SerializeField] private Gadget prefab;
        
        public void OnSpawned(Unit unit)
        {
            unit.OnDeath += UnitOnOnDeath;
        }
        
        private void UnitOnOnDeath(Unit unit)
        {
            var gadget = CombatManager.Current.SpawnGadget(prefab);
            gadget.Init(unit.GridPosition);
        }
    }
}