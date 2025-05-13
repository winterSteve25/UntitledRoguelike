using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Utils;

namespace Combat.Deck
{
    public class Inventory : NetworkBehaviour
    {
        public event Action<ItemInstance> OnItemAdded;
        public event Action<int> OnItemRemoved;

        // synchronized manually via rpc calls
        private List<ItemInstance> _itemsSync;
        private Vector2Int _size;

        public void Init(Vector2Int inventorySize)
        {
            _itemsSync = new List<ItemInstance>();
            _size = inventorySize;
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
                    return;
                }
            }
        }

        public void AddItem(ItemType itemType, Vector2Int position, bool skipChecks = false)
        {
            AddItemRpc(itemType.name, itemType.Category, position, skipChecks);
        }

        [Rpc(SendTo.Server)]
        private void AddItemRpc(string itemType, Category category, Vector2Int position, bool skipChecks)
        {
            var it = category.GetItemType(itemType);
            if (!skipChecks && !CanPlaceAt(it, position)) return;
            AddItemToListRpc(itemType, category, position);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void AddItemToListRpc(string itemType, Category category, Vector2Int position)
        {
            var it = category.GetItemType(itemType);
            var instance = new ItemInstance(it, position);
            OnItemAdded?.Invoke(instance);
            _itemsSync.Add(instance);
        }

        public void RemoveItem(Vector2Int position)
        {
            RemoveItemRpc(position);
        }

        [Rpc(SendTo.Server)]
        private void RemoveItemRpc(Vector2Int position)
        {
            for (var i = 0; i < _itemsSync.Count; i++)
            {
                if (!RectangleTester.InBound(_itemsSync[i].ItemType.Size, _itemsSync[i].Position, position.x,
                        position.y, false, false))
                    continue;
                RemoveItemFromListRpc(i);
                break;
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void RemoveItemFromListRpc(int i)
        {
            OnItemRemoved?.Invoke(i);
            _itemsSync.RemoveAt(i);
        }

        public bool CanPlaceAt(ItemType itemType, Vector2Int position)
        {
            var inBound = RectangleTester.InBound(_size, Vector2Int.zero, position.x, position.y, itemType.Size.x,
                itemType.Size.y, true, false);

            return inBound && !_itemsSync.Any(x => RectangleTester.AreRectanglesOverlapping(
                x.Position.x, x.Position.y, x.ItemType.Size.x, x.ItemType.Size.y,
                position.x, position.y, itemType.Size.x, itemType.Size.y));
        }

        public ItemInstance GetItem(Vector2Int position)
        {
            return _itemsSync.FirstOrDefault(t =>
                RectangleTester.InBound(t.ItemType.Size, t.Position, position.x, position.y, false, false));
        }

        public bool CanMoveTo(ItemInstance item, Vector2Int position)
        {
            if (position.x < 0 || position.y < 0 || position.x + item.ItemType.Size.x > _size.x ||
                position.y + item.ItemType.Size.y > _size.y)
                return false;

            foreach (var i in _itemsSync)
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