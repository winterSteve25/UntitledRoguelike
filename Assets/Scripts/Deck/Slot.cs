using UnityEngine;
using UnityEngine.EventSystems;

namespace Deck
{
    public class Slot : MonoBehaviour
    {
        public Vector2Int Pos { get; private set; }
        private Inventory _inventory;

        public void Init(Inventory inventory, int x, int y)
        {
            _inventory = inventory;
            Pos = new Vector2Int(x, y);
        }
    }
}