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

        public void AddAnywhere(ItemType itemType)
        {
            for (int i = 0; i < _size.x; i++)
            {
                for (int j = 0; j < _size.y; j++)
                {
                    var pos = new Vector2Int(i, j);
                    if (!CanPlaceAt(itemType, pos)) continue;
                    AddItem(itemType, pos, true);
                    break;
                }
            }
        }

        public void AddItem(ItemType itemType, Vector2Int position, bool skipChecks = false)
        {
            if (!skipChecks && !CanPlaceAt(itemType, position)) return;
            var instance = new ItemInstance(itemType, position);
            OnItemAdded?.Invoke(instance);
            _items.Add(instance);
        }

        public void RemoveItem(Vector2Int position)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (!RectangleTester.InBound(_items[i].ItemType.Size, _items[i].Position, position.x, position.y, false))
                    continue;
                OnItemRemoved?.Invoke(i);
                _items.RemoveAt(i);
                break;
            }
        }

        public bool CanPlaceAt(ItemType itemType, Vector2Int position)
        {
            var inBound = RectangleTester.InBound(_size, Vector2Int.zero, position.x, position.y, itemType.Size.x,
                itemType.Size.y, true);
            
            return inBound && !_items.Any(x => RectangleTester.AreRectanglesOverlapping(
                       x.Position.x, x.Position.y, x.ItemType.Size.x, x.ItemType.Size.y,
                       position.x, position.y, itemType.Size.x, itemType.Size.y));
        }

        public ItemInstance GetItem(Vector2Int position)
        {
            return _items.FirstOrDefault(t =>
                RectangleTester.InBound(t.ItemType.Size, t.Position, position.x, position.y, false));
        }

        public bool CanMoveTo(ItemInstance item, Vector2Int position)
        {
            if (position.x < 0 || position.y < 0 || position.x + item.ItemType.Size.x > _size.x ||
                position.y + item.ItemType.Size.y > _size.y)
                return false;

            foreach (var i in _items)
            {
                var overlap = RectangleTester.AreRectanglesOverlapping(position.x, position.y, item.ItemType.Size.x,
                    item.ItemType.Size.y,
                    i.Position.x, i.Position.y, i.ItemType.Size.x, i.ItemType.Size.y);

                if (!overlap) continue;
                if (ReferenceEquals(item, i)) continue;

                return false;
            }

            return true;
        }

        public void DebugPrint()
        {
            foreach (var i in _items)
            {
                Debug.Log($"{i.Position} - {i.ItemType}");
            }
        }

        public class ItemInstance
        {
            public readonly ItemType ItemType;
            public readonly Vector2Int Position;

            public ItemInstance(ItemType itemType, Vector2Int position)
            {
                ItemType = itemType;
                Position = position;
            }
        }
    }
}