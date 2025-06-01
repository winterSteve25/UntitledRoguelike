using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;

namespace Combat.Deck
{
    public class ItemVisual : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private FollowMouse followBehaviour;

        private Camera _cam;
        private InventoryUI.ItemInstanceWithVisuals _thisInstance;
        private bool _followMouse;
        private RectTransform _deckParent;
        private bool _interactable;
        protected Slot Slot;
        protected Inventory Inventory;

        private void Update()
        {
            if (_followMouse) return;
            transform.position = Slot.transform.position;
        }

        public void Init(InventoryUI.ItemInstanceWithVisuals item, GridLayoutGroup grid, Slot position,
            Inventory inventory, Canvas canvas, RectTransform deckParent, bool interactable)
        {
            followBehaviour.Init(canvas, item.ItemType.Size);
            
            _deckParent = deckParent;
            _interactable = interactable;
            _cam = canvas.worldCamera;
            _thisInstance = item;
            Slot = position;
            Inventory = inventory;

            if (interactable)
            {
                image.sprite = item.ItemType.Sprite;
            }
            else
            {
                image.gameObject.SetActive(false);
            }

            RectTransform rectTransform = (RectTransform)transform;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                item.ItemType.Size.x * grid.cellSize.x +
                grid.spacing.x * Mathf.Max(0, item.ItemType.Size.x - 1));
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                item.ItemType.Size.y * grid.cellSize.y +
                grid.spacing.y * Mathf.Max(0, item.ItemType.Size.y - 1));
            
            transform.position = Slot.transform.position;
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
            transform.position = Slot.transform.position;
            image.raycastTarget = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_interactable) return;
            FollowMouse();

            // TODO
            // if (EventSystem.current.IsPointerOverGameObject(eventData.pointerId)) return;
            // Tween.UIPivotY(_deckParent, 1, 0.1f);
            // Tween.UIAnchoredPositionY(_deckParent, -20, 0.1f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_interactable) return;
            var list = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, list);

            // if (EventSystem.current.IsPointerOverGameObject(eventData.pointerId))
            // {
            //     Tween.UIPivotY(_deckParent, 0, 0.1f);
            //     Tween.UIAnchoredPositionY(_deckParent, 20, 0.1f);
            // }

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
            if (Inventory.CanMoveTo(_thisInstance.Instance, pos))
            {
                Inventory.RemoveItem(Slot.Pos);
                Inventory.AddItem(_thisInstance.ItemType, pos);
            }
            else
            {
                ResetPosition();
            }
        }

        protected virtual void TryUseOrCancel()
        {
            var pos = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var combatManager = CombatManager.Current;

            if (combatManager.Me.Energy >= _thisInstance.ItemType.Cost &&
                combatManager.MyTurn &&
                _thisInstance.ItemType.CanUse(pos))
            {
                combatManager.Me.Energy -= _thisInstance.ItemType.Cost;
                _thisInstance.ItemType.Use(pos);
                Inventory.RemoveItem(Slot.Pos);
                return;
            }

            ResetPosition();
        }
    }
}