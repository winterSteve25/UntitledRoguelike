using System;
using Levels;
using PrimeTween;
using UnityEngine;

namespace Combat
{
    public delegate void UnitHpChangedEvent(Unit unit, int original, int current);
    public delegate void NewTurnEvent(Unit unit, bool friendlyTurn);

    public class Unit : MonoBehaviour
    {
        [SerializeField] private GameObject abilitiesParent;
        [SerializeField] private GameObject passivesParent;

        public event Action OnDeath;
        public event Action OnAttack;
        public event UnitHpChangedEvent OnHpChanged;
        public event NewTurnEvent OnNewTurn;
        public event Action<bool> OnInteractabilityChanged;

        public Vector2Int GridPosition { get; private set; }
        public UnitType Type { get; private set; }
        public bool Friendly { get; private set; }
        public int Hp { get; private set; }

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

        public void Init(UnitType unitType, Vector2Int position, Vector2 worldPosition, bool friendly)
        {
            Abilities = abilitiesParent.GetComponents<IAbility>();
            Passives = passivesParent.GetComponents<IPassive>();

            _interactable = true;
            Type = unitType;
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
    }
}