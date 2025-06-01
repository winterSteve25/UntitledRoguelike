using System;
using Combat.Deck;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class AddCardButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private InventoryUI inventory;
        [SerializeField] private ItemType itemType;
        [SerializeField] private Image border;

        private void Start()
        {
            var color = border.color;
            color.a = 0;
            border.color = color;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            inventory.AddItemAnywhere(itemType);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Tween.Alpha(border, 1, 0.2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Tween.Alpha(border, 0, 0.1f);
        }
    }
}