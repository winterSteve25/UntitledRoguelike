using Deck;
using Unity.Netcode;
using UnityEngine;

namespace Combat
{
    public class Player : MonoBehaviour
    {
        [field: SerializeField]
        public Inventory Inventory { get; private set; }

        private int _energy;
        public int Energy
        {
            get => _energy;
            set
            {
                _energy = value;
                // TODO
                // infoUI.UpdateEnergy(_energy, maxEnergy);
                // selectedUnitUI.UpdateEnergy(_energy);
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