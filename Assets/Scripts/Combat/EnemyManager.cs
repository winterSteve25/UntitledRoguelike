using Deck;
using UnityEngine;

namespace Combat
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Current { get; private set; }
        
        public Inventory Inventory { get; private set; }
        
        [SerializeField] private Vector2Int invSize;

        private void Awake()
        {
            Current = this;
        }

        private void Start()
        {
            Inventory = new Inventory(invSize);
        }

        private void OnDestroy()
        {
            Current = null;
        }
    }
}