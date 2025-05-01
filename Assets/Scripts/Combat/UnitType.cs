using Deck;
using Levels;
using UnityEngine;

namespace Combat
{
    public class UnitType : ScriptableObject, IItem
    {
        [field: SerializeField] public Unit Prefab { get; set; }
        
        [field: Header("Stats")]
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Vector2Int Size { get; private set; }
        [field: SerializeField] public int MaxHp { get; private set; }
        [field: SerializeField] public int Cost { get; private set; }

        [Header("Visual Configuration")]
        [SerializeField] private bool useCustomOffset;
        [SerializeField] private Vector2Int placementOffset;
        
        public Sprite Sprite => Prefab.GetComponentInChildren<SpriteRenderer>().sprite;
        
        public bool CanUse(Vector2 worldPosition)
        {
            var gridPosition = GetRelativePosition(worldPosition);
            return CombatManager.Current.CanPlaceUnitAt(this, new Vector2Int(gridPosition.x, gridPosition.y));
        }

        public void Use(Vector2 worldPosition)
        {
            var gridPosition = GetRelativePosition(worldPosition);
            CombatManager.Current.SpawnUnit(this, new Vector2Int(gridPosition.x, gridPosition.y), true);
        }

        private Vector2Int GetRelativePosition(Vector2 worldPosition)
        {
            var gridPosition = Level.Current.Tilemap.WorldToCell(worldPosition);
            var offset = placementOffset;
            
            if (!useCustomOffset)
            {
                offset = new Vector2Int(Mathf.Max(0, Size.x - 1), Mathf.Max(0, Size.y - 1));
            }
            
            return new Vector2Int(gridPosition.x, gridPosition.y);
        }
    }
}