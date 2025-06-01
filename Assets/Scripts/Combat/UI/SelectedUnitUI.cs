using System.Globalization;
using Levels;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Combat.UI
{
    public class SelectedUnitUI : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private CanvasGroup panel;

        [Header("UI Elements")] 
        [SerializeField] private TMP_Text unitName;
        [SerializeField] private TMP_Text hp;
        [SerializeField] private Image icon;
        [SerializeField] private AbilitiesButtonGroup abilityBtns;
        [SerializeField] private ScrollUI scrollUI;

        public Unit Showing => _showing;
        private Unit _showing;
        private bool _canOpenMenu;

        private void Awake()
        {
            _canOpenMenu = true;
        }

        private void OnEnable()
        {
            CombatManager.Current.OnTurnChanged += TurnChanged;
        }

        private void OnDisable()
        {
            if (CombatManager.Current == null) return;
            CombatManager.Current.OnTurnChanged -= TurnChanged;
        }

        private void Update()
        {
            if (!_canOpenMenu) return;
            if (!Mouse.current.leftButton.wasPressedThisFrame) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            var wp = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            wp.z = 0;
            var gp = Level.Current.Tilemap.WorldToCell(wp);
            if (!CombatManager.Current.TryGetUnit(gp.x, gp.y, out var unit))
            {
                Show(null);
                return;
            }

            Show(unit);
        }

        public void Show(Unit unit)
        {
            Unsubscribe(_showing);
            var panelTransform = (RectTransform)panel.transform;

            if (unit == null)
            {
                Tween.UIPivotY(panelTransform, 1, 0.1f);
                Tween.UIAnchoredPositionY(panelTransform, -80, 0.2f);
                _showing = null;
                abilityBtns.Clear();
                scrollUI.Hide();
                return;
            }

            if (_showing == null)
            {
                Tween.UIPivotY(panelTransform, 0, 0.1f);
                Tween.UIAnchoredPositionY(panelTransform, 20, 0.2f);
            }

            abilityBtns.Clear();
            _showing = unit;
            
            var size = icon.rectTransform.sizeDelta;
            var ratio = unit.Type.Sprite.rect.width / unit.Type.Sprite.rect.height;
            size.x = size.y * ratio;
            
            unitName.text = unit.Type.Name;
            icon.sprite = unit.Type.Sprite;
            icon.rectTransform.sizeDelta = size;
            
            ChangeHp(unit, 0, unit.Hp, DamageSource.Healing, null);

            var combatManager = CombatManager.Current;
            var areaSelector = PlayerAreaSelector.Current;

            foreach (var ability in unit.Abilities)
            {
                abilityBtns.AddAbility(combatManager, unit, ability, scrollUI, areaSelector);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(panelTransform);
            Subscribe(_showing);
        }

        public void CanOpenMenu(bool value)
        {
            _canOpenMenu = value;
        }

        private void Subscribe(Unit unit)
        {
            if (unit == null) return;
            unit.OnHpChange += ChangeHp;
            unit.OnInteractabilityChanged += UnitInteractableChange;
        }

        private void Unsubscribe(Unit unit)
        {
            if (unit == null) return;
            unit.OnHpChange -= ChangeHp;
            unit.OnInteractabilityChanged -= UnitInteractableChange;
        }

        private void ChangeHp(Unit unit, int original, int current, DamageSource source, CancelToken cancelToken)
        {
            hp.text = current.ToString();
        }

        public void UnitInteractableChange(bool interactable)
        {
            if (abilityBtns.List == null) return;
            foreach (var btn in abilityBtns.List)
            {
                btn.Item1.Interactable = interactable && CombatManager.Current.AmIFriendly == _showing.Friendly && CombatManager.Current.MyTurn;
            }
        }

        private void TurnChanged(int turnNumber, bool friendly)
        {
            if (_showing == null) return;
            if (abilityBtns.List == null) return;
            foreach (var btn in abilityBtns.List)
            {
                btn.Item1.Interactable = btn.Item1.Interactable && friendly == _showing.Friendly;
            }
        }

        public void UpdateEnergy(int playerEnergy)
        {
            if (abilityBtns.List == null) return;
            foreach (var btn in abilityBtns.List)
            {
                btn.Item1.Interactable =
                    btn.Item1.Interactable
                    && CombatManager.Current.AmIFriendly == _showing.Friendly
                    && btn.Item2 <= playerEnergy
                    && CombatManager.Current.MyTurn;
            }
        }
    }
}