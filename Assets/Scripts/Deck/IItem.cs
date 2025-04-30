using UnityEngine;

namespace Deck
{
    public interface IItem
    {
        public Vector2Int Size { get; }
        public bool OnlyUseOnGrid { get; }
        public Sprite Sprite { get; } 
    }
}