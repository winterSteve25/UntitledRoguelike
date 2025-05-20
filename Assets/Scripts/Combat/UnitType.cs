using Combat.Deck;
using Levels;
using Unity.Netcode;
using UnityEngine;
using Utils;

namespace Combat
{
    public class UnitType : ItemType
    {
        [field: SerializeField] public NetworkObject Prefab { get; set; }

        [field: Header("Stats")]
        [field: SerializeField]
        public string Name { get; private set; }

        [field: SerializeField] public float MaxHp { get; private set; }
        [SerializeField] private Vector2Int size;
        [SerializeField] private int cost;

        public override Vector2Int Size => size;
        public override int Cost => cost;
        public override Category Category => Category.Unit;
        public override Sprite Sprite => Prefab.GetComponentInChildren<SpriteRenderer>().sprite;

        public override bool CanUse(Vector2 worldPosition)
        {
            var gridPosition = GetRelativePosition(worldPosition);
            var dy = Mathf.FloorToInt(size.y / 2f);
            
            // TODO: maybe dont hard code values?
            // restrict placement
            if (CombatManager.Current.AmIFriendly)
            {
                if (gridPosition.y + dy >= 3) return false;
            }
            else
            {
                if (gridPosition.y - dy <= 5) return false;
            }

            return CombatManager.Current.CanPlaceUnitAt(this, new Vector2Int(gridPosition.x, gridPosition.y));
        }

        public override void Use(Vector2 worldPosition)
        {
            var gridPosition = GetRelativePosition(worldPosition);
            var combatManager = CombatManager.Current;
            combatManager.SpawnUnit(this, new Vector2Int(gridPosition.x, gridPosition.y), combatManager.AmIFriendly);
        }

        private Vector2Int GetRelativePosition(Vector2 worldPosition)
        {
            var gridPosition = Level.Current.Tilemap.WorldToCell(worldPosition);
            return new Vector2Int(gridPosition.x, gridPosition.y);
        }
    }
}