using System;
using Levels;
using PrimeTween;
using Unity.Netcode;
using UnityEngine;

namespace Combat
{
    public delegate void UnitHpChangeEvent(Unit unit, float original, float newHp, DamageSource source, CancelToken cancel);
    public delegate void NewTurnEvent(Unit unit, bool friendlyTurn);

    public class CancelToken
    {
        public bool Canceled = false;
    }

    public class Unit : NetworkBehaviour
    {
        [SerializeField] private GameObject abilitiesParent;
        [SerializeField] private GameObject passivesParent;
        [SerializeField] private SpriteRenderer border;

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

        public void Init(Vector2Int position, bool friendly)
        {
            InitRpc(position, friendly);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void InitRpc(Vector2Int position, bool friendly)
        {
            var combatManager = CombatManager.Current;
            Abilities = abilitiesParent.GetComponents<IAbility>();
            Passives = passivesParent.GetComponents<IPassive>();
            border.color = combatManager.AmIFriendly == friendly ? Color.aquamarine : Color.crimson;

            _interactable = true;
            GridPosition = position;
            Friendly = friendly;
            Hp = Type.MaxHp;
            transform.position = GetWorldPosition(position);

            foreach (var passive in Passives)
            {
                passive.OnSpawned(this);
            }
            
            combatManager.RegisterUnit(this);
        }

        public void RemoveSelf()
        {
            RemoveSelfRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void RemoveSelfRpc()
        {
            CombatManager.Current.RemoveUnit(this);
        }
        
        public override void OnDestroy()
        {
            base.OnDestroy();
            OnDeath = null;
            OnNewTurn = null;
            OnHpChange = null;
            OnInteractabilityChanged = null;
        }

        public void NextTurn(bool friendlyTurn)
        {
            OnNewTurn?.Invoke(this, friendlyTurn);
        }

        public void MoveTo(Vector2Int position)
        {
            GridPosition = position;
            Tween.Position(transform, GetWorldPosition(position), 0.2f);
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
            CombatManager.Current.DespawnUnit(this);
        }

        private Vector2 GetWorldPosition(Vector2Int position)
        {
            if (!CombatManager.Current.AmIFriendly)
            {
                return Level.Current.CellToWorld(position + new Vector2Int(1 + Mathf.FloorToInt(Type.Size.x / 2f), 1 + Mathf.FloorToInt(Type.Size.y / 2f)));
            }
            
            return Level.Current.CellToWorld(position);
        }
    }
}