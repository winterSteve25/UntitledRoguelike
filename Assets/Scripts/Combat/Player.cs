using Combat.Deck;
using Combat.UI;
using Unity.Netcode;
using UnityEngine;

namespace Combat
{
    public class Player : MonoBehaviour
    {
        [field: SerializeField]
        public Inventory Inventory { get; private set; }

        [SerializeField] private CombatInfoUI infoUI;
        [SerializeField] private SelectedUnitUI selectedUnitUI;

        // not synchronized
        private int _energy;
        public int Energy
        {
            get => _energy;
            set
            {
                _energy = value;
                infoUI.UpdateEnergy(_energy);
                selectedUnitUI.UpdateEnergy(_energy);
            }
        }

        public void Init(Vector2Int inventorySize, int energy)
        {
            Inventory.Init(inventorySize);
            Energy = energy;
        }

        public override string ToString()
        {
            return $"Energy: {Energy}, Inventory: {Inventory}";
        }
    }
}