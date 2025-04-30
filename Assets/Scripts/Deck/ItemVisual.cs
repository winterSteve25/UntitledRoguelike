using System.Collections.Generic;
using Combat;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;

namespace Deck
{
    public class ItemVisual : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private FollowMouse followBehaviour;

        private Camera _cam;
        private InventoryUI.ItemInstance _thisInstance;
        private bool _followMouse;
        private Slot _slot;
        private InventoryUI _inventoryUI;

        private void Update()
        {
            if (_followMouse) return;
            transform.position = _slot.transform.position;
        }

        public void Init(InventoryUI.ItemInstance item, GridLayoutGroup grid, Slot position, InventoryUI inventoryUI,
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

            _cam = canvas.worldCamera;
            _thisInstance = item;
            _slot = position;
            _inventoryUI = inventoryUI;

            transform.position = _slot.transform.position;
        }

        private void FollowMouse()
        {
            RectTransform rectTransform = (RectTransform)transform;
            _followMouse = true;
            rectTransform.pivot = new Vector2(1f / _thisInstance.Item.Size.x * 0.5f,
                (1f / _thisInstance.Item.Size.y) * 0.5f);
            followBehaviour.enabled = true;
            followBehaviour.UpdatePosition();
            image.raycastTarget = false;
        }

        private void ResetPosition()
        {
            RectTransform rectTransform = (RectTransform)transform;
            _followMouse = false;
            rectTransform.pivot = new Vector2(0, 0);
            followBehaviour.enabled = false;
            transform.position = _slot.transform.position;
            image.raycastTarget = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            FollowMouse();
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
            if (_inventoryUI.CanMoveTo(_thisInstance, pos))
            {
                _inventoryUI.RemoveItem(_slot.Pos);
                _inventoryUI.AddItem(_thisInstance.Item, pos);
            }
            else
            {
                ResetPosition();
            }
        }

        private void TryUseOrCancel()
        {
            var pos = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var combatManager = CombatManager.Current;
            
            if (combatManager.PlayerEnergy >= _thisInstance.Item.Cost &&
                combatManager.FriendlyTurn &&
                _thisInstance.Item.CanUse(pos))
            {
                combatManager.PlayerEnergy -= _thisInstance.Item.Cost;
                _thisInstance.Item.Use(pos);
                _inventoryUI.RemoveItem(_slot.Pos);
                return;
            }

            ResetPosition();
        }
    }
}