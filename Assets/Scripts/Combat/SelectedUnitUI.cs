using System.Collections.Generic;
using Levels;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Combat
{
    public class SelectedUnitUI : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private CanvasGroup panel;

        [Header("UI Elements")] [SerializeField]
        private TMP_Text unitName;

        [SerializeField] private TMP_Text hp;
        [SerializeField] private RectTransform btnsGroup;
        [SerializeField] private Button abilityBtnPrefab;

        public Unit Showing => _showing;
        private Unit _showing;
        private List<(Button, int)> _abilityBtns;
        private bool _canOpenMenu;

        private void Awake()
        {
            _abilityBtns = new List<(Button, int)>();
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
                Tween.UIAnchoredPositionY(panelTransform, -50, 0.2f);
                _showing = null;

                foreach (var btn in _abilityBtns)
                {
                    Destroy(btn.Item1.gameObject);
                }

                _abilityBtns.Clear();
                return;
            }

            if (_showing == null)
            {
                Tween.UIPivotY(panelTransform, 0, 0.1f);
                Tween.UIAnchoredPositionY(panelTransform, 50, 0.2f);
            }

            foreach (var btn in _abilityBtns)
            {
                Destroy(btn.Item1.gameObject);
            }

            _abilityBtns.Clear();

            unitName.text = unit.Type.Name;
            _showing = unit;
            ChangeHp(unit, 0, unit.Hp);
            
            var combatManager = CombatManager.Current;
            var areaSelector = PlayerAreaSelector.Current;

            foreach (var ability in unit.Abilities)
            {
                // TODO: Use pools
                var btn = Instantiate(abilityBtnPrefab, btnsGroup);
                btn.interactable = unit.Interactable
                                   && CombatManager.Current.FriendlyTurn == unit.Friendly
                                   && CombatManager.Current.PlayerEnergy >= ability.Cost;
                btn.GetComponentInChildren<TMP_Text>().text = $"{ability.Name} ({ability.Cost})";
                btn.onClick.AddListener(() =>
                {
                    if (combatManager.PlayerEnergy < ability.Cost || !combatManager.FriendlyTurn)
                    {
                        return;
                    }

                    combatManager.PlayerEnergy -= ability.Cost;
                    ability.Perform(combatManager, unit, areaSelector);
                });

                _abilityBtns.Add((btn, ability.Cost));
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
            unit.OnHpChanged += ChangeHp;
            unit.OnInteractabilityChanged += UnitInteractableChange;
        }

        private void Unsubscribe(Unit unit)
        {
            if (unit == null) return;
            unit.OnHpChanged -= ChangeHp;
            unit.OnInteractabilityChanged -= UnitInteractableChange;
        }

        private void ChangeHp(Unit unit, int original, int current)
        {
            hp.text = $"Hp: {current}/{unit.Type.MaxHp}";
        }

        public void UnitInteractableChange(bool interactable)
        {
            foreach (var btn in _abilityBtns)
            {
                btn.Item1.interactable = interactable && CombatManager.Current.FriendlyTurn == _showing.Friendly;
            }
        }

        private void TurnChanged(int turnNumber, bool friendly)
        {
            if (_showing == null) return;

            foreach (var btn in _abilityBtns)
            {
                btn.Item1.interactable = btn.Item1.interactable && friendly == _showing.Friendly;
            }
        }

        public void UpdateEnergy(int playerEnergy)
        {
            foreach (var btn in _abilityBtns)
            {
                btn.Item1.interactable =
                    btn.Item1.interactable
                    && CombatManager.Current.FriendlyTurn == _showing.Friendly
                    && btn.Item2 <= playerEnergy;
            }
        }
    }
}