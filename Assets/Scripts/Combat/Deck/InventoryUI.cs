using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Combat.Deck
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup gridLayout;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Slot slotPrefab;
        [SerializeField] private ItemVisual visualPrefab;
        [SerializeField] private RectTransform everythingParent;
        [SerializeField] private bool interactable;

        private Inventory _inventory;
        private List<ItemInstanceWithVisuals> _items;
        private Slot[,] _slots;
        private RectTransform _rectTransform;

        public void Init(Vector2Int size, Inventory inventory)
        {
            _inventory = inventory;
            _inventory.OnItemAdded += AddItem;
            _inventory.OnItemRemoved += RemoveItem;
            _items = new List<ItemInstanceWithVisuals>();
            
            _slots = new Slot [size.x, size.y];
            _rectTransform = (RectTransform)transform;
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                size.x * gridLayout.cellSize.x + gridLayout.padding.left + gridLayout.padding.right +
                gridLayout.spacing.x * Mathf.Max(0, size.x - 1));
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                size.y * gridLayout.cellSize.y + gridLayout.padding.top + gridLayout.padding.bottom +
                gridLayout.spacing.y * Mathf.Max(0, size.y - 1));

            // y loop first b/c grid layout works like that
            for (var j = size.y - 1; j >= 0; j--)
            {
                for (var i = 0; i < size.x; i++)
                {
                    var slot = Instantiate(slotPrefab, gridLayout.transform);
                    slot.Init(i, j);
                    _slots[i, j] = slot;
                }
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
        }

        private void AddItem(Inventory.ItemInstance item)
        {
            var visuals = Instantiate(visualPrefab, transform.parent);
            var instance = new ItemInstanceWithVisuals(item, visuals);

            visuals.Init(instance, gridLayout, _slots[item.Position.x, item.Position.y], _inventory, canvas, everythingParent, interactable);
            _items.Add(instance);
        }
        
        private void RemoveItem(int index)
        {
            Destroy(_items[index].Visual.gameObject);
            _items.RemoveAt(index);
        }

        public void AddItem(ItemType itemType, Vector2Int position)
        {
            _inventory.AddItem(itemType, position);
        }

        public void AddItemAnywhere(ItemType itemType)
        {
            _inventory.AddAnywhere(itemType);
        }
        
        public void AddItemsAnywhere<T>(List<T> itemType) where T : ItemType
        {
            _inventory.AddAnywhere(itemType);
        }

        public void RemoveItem(Vector2Int pos)
        {
            _inventory.RemoveItem(pos);
        }
        
        public bool CanMoveTo(ItemInstanceWithVisuals item, Vector2Int position)
        {
            return _inventory.CanMoveTo(item.Instance, position);
        }

        public class ItemInstanceWithVisuals
        {
            public readonly Inventory.ItemInstance Instance;
            public readonly ItemVisual Visual;
            
            public ItemType ItemType => Instance.ItemType;

            public ItemInstanceWithVisuals(Inventory.ItemInstance instance, ItemVisual visual)
            {
                Instance = instance;
                Visual = visual;
            }
        }
    }
}