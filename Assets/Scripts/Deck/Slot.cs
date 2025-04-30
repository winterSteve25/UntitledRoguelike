using UnityEngine;
using UnityEngine.EventSystems;

namespace Deck
{
    public class Slot : MonoBehaviour
    {
        public Vector2Int Pos { get; private set; }
        private InventoryUI _inventoryUI;

        public void Init(InventoryUI inventoryUI, int x, int y)
        {
            _inventoryUI = inventoryUI;
            Pos = new Vector2Int(x, y);
        }
    }
}