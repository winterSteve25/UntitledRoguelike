using Combat;
using Deck;
using UnityEngine;

namespace Content.General
{
    public class GiveCardPassive : MonoBehaviour, IPassive
    {
        [SerializeField] private int turnCount;
        [SerializeField] private bool friendlyOnly;
        [SerializeField] private bool atStart;
        [SerializeField] private ItemType itemType;

        private int _counter;

        public void OnSpawned(Unit unit)
        {
            unit.OnNewTurn += UnitOnOnNewTurn;
            _counter = 0;
        }

        private void UnitOnOnNewTurn(Unit unit, bool friendlyturn)
        {
            if (atStart && (friendlyturn != CombatManager.Current.AmIFriendly && friendlyOnly)) return;
            if (!atStart && (friendlyturn == CombatManager.Current.AmIFriendly || !friendlyOnly)) return;

            _counter++;
            if (_counter != turnCount) return;

            _counter = 0;
            var inventory = CombatManager.Current.Me.Inventory;
            inventory.AddAnywhere(itemType);
        }
    }
}