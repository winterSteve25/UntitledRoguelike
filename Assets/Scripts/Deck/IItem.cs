using UnityEngine;

namespace Deck
{
    public interface IItem
    {
        public Vector2Int Size { get; }
        public Sprite Sprite { get; }
        public int Cost { get; }
        
        bool CanUse(Vector2 worldPosition);
        void Use(Vector2 worldPosition);
    }
}