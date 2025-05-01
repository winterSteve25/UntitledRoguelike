using Combat;
using Deck;
using UnityEngine;

namespace Content.General
{
    public class GiveCardPassive : MonoBehaviour, IPassive
    {
        [SerializeField] private int turnCount;
        [SerializeField] private bool friendlyOnly;
        [SerializeField] private ItemType itemType;

        private int _counter;

        public void OnSpawned(Unit unit)
        {
            unit.OnNewTurn += UnitOnOnNewTurn;
        }

        public void OnDespawned(Unit unit)
        {
            unit.OnNewTurn -= UnitOnOnNewTurn;
        }

        private void UnitOnOnNewTurn(Unit unit, bool friendlyturn)
        {
            if (!friendlyturn && friendlyOnly) return;
            _counter++;
            if (_counter != turnCount) return;
            
            _counter = 0;
            var inventory = unit.Friendly 
                ? CombatManager.Current.PlayerInventory
                : EnemyManager.Current.Inventory;

            inventory.AddAnywhere(itemType);
        }
    }
}