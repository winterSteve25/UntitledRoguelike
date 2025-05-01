using System;
using Levels;
using PrimeTween;
using UnityEngine;

namespace Combat
{
    public delegate void UnitHpChangeEvent(Unit unit, float original, float newHp, DamageSource source, CancelToken cancel);
    public delegate void NewTurnEvent(Unit unit, bool friendlyTurn);

    public class CancelToken
    {
        public bool Canceled = false;
    }

    public class Unit : MonoBehaviour
    {
        [SerializeField] private GameObject abilitiesParent;
        [SerializeField] private GameObject passivesParent;

        public event Action<Unit> OnDeath;
        public event UnitHpChangeEvent OnHpChange;
        public event NewTurnEvent OnNewTurn;
        public event Action<bool> OnInteractabilityChanged;

        [field: SerializeField]
        public UnitType Type { get; set; }
        public Vector2Int GridPosition { get; private set; }
        public bool Friendly { get; private set; }
        public float Hp { get; private set; }

        public IAbility[] Abilities { get; private set; }
        public IPassive[] Passives { get; private set; }

        private bool _interactable;
        public bool Interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
                OnInteractabilityChanged?.Invoke(value);
            }
        }

        public void Init(Vector2Int position, Vector2 worldPosition, bool friendly)
        {
            Abilities = abilitiesParent.GetComponents<IAbility>();
            Passives = passivesParent.GetComponents<IPassive>();

            _interactable = true;
            GridPosition = position;
            Friendly = friendly;
            Hp = Type.MaxHp;

            transform.position = worldPosition;
        }

        public void NextTurn(bool friendlyTurn)
        {
            OnNewTurn?.Invoke(this, friendlyTurn);
        }

        public void MoveTo(Vector2Int position)
        {
            GridPosition = position;
            Tween.Position(transform, Level.Current.CellToWorld(position), 0.2f);
        }

        public void AddHp(float amount, DamageSource source)
        {
            var cancel = new CancelToken();
            OnHpChange?.Invoke(this, Hp, Hp + amount, source, cancel);
            if (cancel.Canceled) return;
            Hp += amount;

            if (Hp <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            OnDeath?.Invoke(this);
            CombatManager.Current.RemoveUnit(this);
        }
    }
}