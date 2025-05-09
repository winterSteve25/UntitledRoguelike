using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Combat.UI
{
    public class AbilityButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private ScrollUI scrollUI;

        private IAbility _ability;
        private Action _onClick;
        private bool _interactable = true;
        public bool Interactable
        {
            get => _interactable;
            set
            {
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
            _onClick.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            scrollUI.Show(_ability);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            scrollUI.Hide();
        }

        private void InteractChanged()
        {
        }
    }
}