using Combat;
using Deck;
using UnityEngine;

namespace DefaultNamespace
{
    public class test : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private UnitType a;
        [SerializeField] private UnitType b;
        
        private void Start()
        {
            inventory.AddItem(a, new Vector2Int(0, 0));
            inventory.AddItem(b, new Vector2Int(2, 0));
        }
    }
}