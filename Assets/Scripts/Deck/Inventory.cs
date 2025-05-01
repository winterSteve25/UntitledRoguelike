using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Deck
{
    public class Inventory
    {
        private readonly List<ItemInstance> _items = new();
        private readonly Vector2Int _size;

        public event Action<ItemInstance> OnItemAdded;
        public event Action<int> OnItemRemoved;

        public Inventory(Vector2Int size)
        {
            _size = size;
        }

        public bool AddItem(IItem item, Vector2Int position)
        {
            if (position.x < 0 || position.y < 0 || position.x > _size.x || position.y > _size.y) return false;
            if (_items.Any(x => RectangleTester.InBound(x.Item.Size, x.Position, position.x, position.y))) return false;
            
            var instance = new ItemInstance(item, position);
            OnItemAdded?.Invoke(instance);
            _items.Add(instance);

            return true;
        }

        public void RemoveItem(Vector2Int position)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (!RectangleTester.InBound(_items[i].Item.Size, _items[i].Position, position.x, position.y)) continue;
                OnItemRemoved?.Invoke(i);
                _items.RemoveAt(i);
                break;
            }
        }

        public ItemInstance GetItem(Vector2Int position)
        {
            return _items.FirstOrDefault(t => RectangleTester.InBound(t.Item.Size, t.Position, position.x, position.y));
        }

        public bool CanMoveTo(ItemInstance item, Vector2Int position)
        {
            if (position.x < 0 || position.y < 0 || position.x + item.Item.Size.x > _size.x ||
                position.y + item.Item.Size.y > _size.y)
                return false;

            foreach (var i in _items)
            {
                var overlap = RectangleTester.AreRectanglesOverlapping(position.x, position.y, item.Item.Size.x,
                    item.Item.Size.y,
                    i.Position.x, i.Position.y, i.Item.Size.x, i.Item.Size.y);

                if (!overlap) continue;
                if (ReferenceEquals(item, i)) continue;

                return false;
            }

            return true;
        }

        public class ItemInstance
        {
            public readonly IItem Item;
            public readonly Vector2Int Position;

            public ItemInstance(IItem item, Vector2Int position)
            {
                Item = item;
                Position = position;
            }
        }
    }
}