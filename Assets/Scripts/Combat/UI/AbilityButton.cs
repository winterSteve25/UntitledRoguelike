using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Combat.UI
{
    public class AbilityButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private RectTransform visual;
        [SerializeField] private ScrollUI scrollUI;

        private IAbility _ability;
        private Action _onClick;
        private bool _interactable = true;

        public bool Interactable
        {
            get => _interactable;
            set
            {
                if (_interactable == value) return;
                _interactable = value;
                InteractChanged();
            }
        }

        public void Init(Action onClick, IAbility ability, ScrollUI scrollUI)
        {
            _onClick = onClick;
            _ability = ability;
            this.scrollUI = scrollUI;
            // icon.sprite = ability.;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_interactable) return;
            _onClick.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_interactable)
            {
                Tween.UIAnchoredPosition(visual, new Vector2(0, 10), 0.1f);
            }
            
            scrollUI.Show(_ability);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_interactable)
            {
                Tween.UIAnchoredPosition(visual, new Vector2(0, 0), 0.1f);
            }
            
            scrollUI.Hide();
        }

        private void InteractChanged()
        {
            Tween.UIAnchoredPosition(visual, new Vector2(0, _interactable ? 0 : -35f), 0.1f);
        }
    }
}