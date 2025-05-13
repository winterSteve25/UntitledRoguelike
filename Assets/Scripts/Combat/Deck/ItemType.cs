using System;
using UnityEngine;

namespace Combat.Deck
{
    public abstract class ItemType : ScriptableObject
    {
        public abstract Vector2Int Size { get; }
        public abstract Sprite Sprite { get; }
        public abstract int Cost { get; }
        public abstract Category Category { get; }
        
        public abstract bool CanUse(Vector2 worldPosition);
        public abstract void Use(Vector2 worldPosition);
    }

    public enum Category
    {
        Unit,
    }
    
    public static class CategoryExt {
        public static ItemType GetItemType(this Category category, string itemType)
        {
            return category switch
            {
                Category.Unit => Resources.Load<UnitType>($"UnitTypes/{itemType}"),
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            };
        }
    }
}