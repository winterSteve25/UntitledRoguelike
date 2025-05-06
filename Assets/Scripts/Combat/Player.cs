using Deck;
using UnityEngine;

namespace Combat
{
    public class Player
    {
        public readonly Inventory Inventory;
        public readonly bool Friendly;
        
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

        public Player(Vector2Int inventorySize, int energy, bool friendly)
        {
            Inventory = new Inventory(inventorySize);
            _energy = energy;
            Friendly = friendly;
        }
    }
}