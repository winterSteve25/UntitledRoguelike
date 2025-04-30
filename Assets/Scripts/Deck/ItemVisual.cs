using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace Deck
{
    public class ItemVisual : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private FollowMouse followBehaviour;

        private Inventory.ItemInstance _thisInstance;
        private bool _followMouse;
        private Slot _slot;
        private Inventory _inventory;

        private void Start()
        {
            followBehaviour.enabled = false;
        }

        private void Update()
        {
            if (_followMouse) return;
            transform.position = _slot.transform.position;
        }

        public void Init(Inventory.ItemInstance item, GridLayoutGroup grid, Slot position, Inventory inventory,
            Canvas canvas)
        {
            followBehaviour.Init(canvas);
            image.sprite = item.Item.Sprite;

            RectTransform rectTransform = (RectTransform)transform;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                item.Item.Size.x * grid.cellSize.x +
                grid.spacing.x * Mathf.Max(0, item.Item.Size.x - 1));
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                item.Item.Size.y * grid.cellSize.y +
                grid.spacing.y * Mathf.Max(0, item.Item.Size.y - 1));

            _thisInstance = item;
            _slot = position;
            _inventory = inventory;

            transform.position = _slot.transform.position;
        }

        private void ToggleFollowMouse()
        {
            RectTransform rectTransform = (RectTransform)transform;

            if (_followMouse)
            {
                _followMouse = false;
                rectTransform.pivot = new Vector2(0, 1);
                followBehaviour.enabled = false;
                transform.position = _slot.transform.position;
                image.raycastTarget = true;
                return;
            }

            _followMouse = true;
            rectTransform.pivot = new Vector2((1f / _thisInstance.Item.Size.x) * 0.5f,
                1 - (1f / _thisInstance.Item.Size.y) * 0.5f);
            followBehaviour.enabled = true;
            followBehaviour.UpdatePosition();
            image.raycastTarget = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ToggleFollowMouse();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            var list = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, list);

            foreach (var item in list)
            {
                if (!item.gameObject.TryGetComponent(out Slot slot)) continue;
                TryPlaceIn(slot.Pos);
                return;
            }

            TryUseOrCancel();
        }

        private void TryPlaceIn(Vector2Int pos)
        {
            if (_inventory.CanMoveTo(_thisInstance, pos))
            {
                _inventory.RemoveItem(_slot.Pos);
                _inventory.AddItem(_thisInstance.Item, pos);
            }
            else
            {
                ToggleFollowMouse();
            }
        }

        private void TryUseOrCancel()
        {
            if (_thisInstance.Item.OnlyUseOnGrid)
            {
                
            }

            ToggleFollowMouse();
        }
    }
}