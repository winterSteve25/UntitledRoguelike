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
        private InventoryUI.ItemInstanceWithVisuals _thisInstance;
        private bool _followMouse;
        private Slot _slot;
        private InventoryUI _inventoryUI;

        private void Update()
        {
            if (_followMouse) return;
            transform.position = _slot.transform.position;
        }

        public void Init(InventoryUI.ItemInstanceWithVisuals item, GridLayoutGroup grid, Slot position, InventoryUI inventoryUI,
            Canvas canvas)
        {
            followBehaviour.Init(canvas, item.ItemType.Size);
            image.sprite = item.ItemType.Sprite;

            RectTransform rectTransform = (RectTransform)transform;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                item.ItemType.Size.x * grid.cellSize.x +
                grid.spacing.x * Mathf.Max(0, item.ItemType.Size.x - 1));
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                item.ItemType.Size.y * grid.cellSize.y +
                grid.spacing.y * Mathf.Max(0, item.ItemType.Size.y - 1));

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
            rectTransform.pivot = new Vector2(1f / _thisInstance.ItemType.Size.x * 0.5f,
                (1f / _thisInstance.ItemType.Size.y) * 0.5f);
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
                _inventoryUI.AddItem(_thisInstance.ItemType, pos);
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
            
            if (combatManager.PlayerEnergy >= _thisInstance.ItemType.Cost &&
                combatManager.FriendlyTurn &&
                _thisInstance.ItemType.CanUse(pos))
            {
                combatManager.PlayerEnergy -= _thisInstance.ItemType.Cost;
                _thisInstance.ItemType.Use(pos);
                _inventoryUI.RemoveItem(_slot.Pos);
                return;
            }

            ResetPosition();
        }
    }
}