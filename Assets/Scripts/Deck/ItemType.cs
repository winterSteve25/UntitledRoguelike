using UnityEngine;

namespace Deck
{
    public abstract class ItemType : ScriptableObject
    {
        public abstract Vector2Int Size { get; }
        public abstract Sprite Sprite { get; }
        public abstract int Cost { get; }
        
        public abstract bool CanUse(Vector2 worldPosition);
        public abstract void Use(Vector2 worldPosition);
    }
}