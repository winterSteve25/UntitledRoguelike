using Deck;
using Levels;
using UnityEngine;

namespace Combat
{
    public class UnitType : ItemType
    {
        [field: SerializeField] public Unit Prefab { get; set; }
        
        [field: Header("Stats")]
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public float MaxHp { get; private set; }
        [SerializeField] private Vector2Int size;
        [SerializeField] private int cost;
        
        public override Vector2Int Size => size;
        public override int Cost => cost;
        public override Sprite Sprite => Prefab.GetComponentInChildren<SpriteRenderer>().sprite;
        
        public override bool CanUse(Vector2 worldPosition)
        {
            var gridPosition = GetRelativePosition(worldPosition);
            // restrict placement
            if (gridPosition.y > 2) return false;
            return CombatManager.Current.CanPlaceUnitAt(this, new Vector2Int(gridPosition.x, gridPosition.y));
        }

        public override void Use(Vector2 worldPosition)
        {
            var gridPosition = GetRelativePosition(worldPosition);
            CombatManager.Current.SpawnUnit(this, new Vector2Int(gridPosition.x, gridPosition.y), true);
        }

        private Vector2Int GetRelativePosition(Vector2 worldPosition)
        {
            var gridPosition = Level.Current.Tilemap.WorldToCell(worldPosition);
            return new Vector2Int(gridPosition.x, gridPosition.y);
        }
    }
}