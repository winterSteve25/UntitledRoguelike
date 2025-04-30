using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Deck
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup gridLayout;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Vector2Int size;
        [SerializeField] private Slot slotPrefab;
        [SerializeField] private ItemVisual visualPrefab;

        private List<ItemInstance> _items;
        private Slot[,] _slots;
        private RectTransform _rectTransform;

        public Slot this[int i, int j] => _slots[i, j];

        private void Awake()
        {
            _items = new List<ItemInstance>();
            _slots = new Slot [size.x, size.y];
            _rectTransform = (RectTransform)transform;
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                size.x * gridLayout.cellSize.x + gridLayout.padding.left + gridLayout.padding.right +
                gridLayout.spacing.x * Mathf.Max(0, size.x - 1));
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                size.y * gridLayout.cellSize.y + gridLayout.padding.top + gridLayout.padding.bottom +
                gridLayout.spacing.y * Mathf.Max(0, size.y - 1));

            // y loop first b/c grid layout works like that
            for (var j = 0; j < size.y; j++)
            {
                for (var i = 0; i < size.x; i++)
                {
                    var slot = Instantiate(slotPrefab, gridLayout.transform);
                    slot.Init(this, i, j);
                    _slots[i, j] = slot;
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
        }

        public bool AddItem(IItem item, Vector2Int position)
        {
            if (position.x < 0 || position.y < 0 || position.x > size.x || position.y > size.y) return false;
            if (_items.Any(x => RectangleTester.InBound(x.Item.Size, x.Position, position.x, position.y))) return false;

            var visuals = Instantiate(visualPrefab, transform.parent);
            var instance = new ItemInstance(item, position, visuals);

            visuals.Init(instance, gridLayout, _slots[position.x, position.y], this, canvas);
            _items.Add(instance);

            return true;
        }

        public void RemoveItem(Vector2Int position)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (!RectangleTester.InBound(_items[i].Item.Size, _items[i].Position, position.x, position.y)) continue;
                Destroy(_items[i].Visual.gameObject);
                _items.RemoveAt(i);
                break;
            }
        }

        public ItemInstance GetItem(Vector2Int position)
        {
            return _items.FirstOrDefault(t => RectangleTester.InBound(t.Item.Size, t.Position, position.x, position.y));
        }

        public bool IsOccupied(Vector2Int position)
        {
            return GetItem(position) != null;
        }

        public bool CanMoveTo(ItemInstance item, Vector2Int position)
        {
            if (position.x < 0 || position.y < 0 || position.x + item.Item.Size.x > size.x ||
                position.y + item.Item.Size.y > size.y)
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
            public readonly ItemVisual Visual;

            public ItemInstance(IItem item, Vector2Int position, ItemVisual visual)
            {
                Item = item;
                Position = position;
                Visual = visual;
            }
        }
    }
}