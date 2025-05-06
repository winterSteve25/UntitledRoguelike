using Deck;
using Unity.Netcode;
using UnityEngine;

namespace Combat
{
    public class Player : NetworkBehaviour
    {
        public Inventory Inventory { get; private set; }
        public bool Friendly { get; private set; }

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

        public void Init(Vector2Int inventorySize, int energy, bool friendly)
        {
            Inventory = new Inventory(inventorySize);
            _energy = energy;
            Friendly = friendly;
        }

        public override string ToString()
        {
            return $"Player {Friendly}, Energy: {Energy}, Inventory: {Inventory}";
        }
    }
}