using System;
using Levels;
using PrimeTween;
using Unity.Netcode;
using UnityEngine;

namespace Combat
{
    public delegate void UnitHpChangeEvent(Unit unit, float original, float newHp, DamageSource source,
        CancelToken cancel);

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

        [field: SerializeField] public UnitType Type { get; set; }
        public Vector2Int GridPositionSynchronized { get; private set; }
        public bool Friendly { get; private set; }

        public IAbility[] Abilities { get; private set; }
        public IPassive[] Passives { get; private set; }

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private NetworkVariable<float> _hp = new();
        public float Hp
        {
            get => _hp.Value;
            private set
            {
                _hp.Value = value;
                
                if (value <= 0)
                {
                    Die();
                }
            }
        }

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

        [Rpc(SendTo.ClientsAndHost)]
        public void InitRpc(Vector2Int position, bool friendly)
        {
            var combatManager = CombatManager.Current;
            Abilities = abilitiesParent.GetComponents<IAbility>();
            Passives = passivesParent.GetComponents<IPassive>();
            border.color = combatManager.AmIFriendly == friendly ? Color.aquamarine : Color.crimson;

            _interactable = true;
            GridPositionSynchronized = position;
            Friendly = friendly;
            transform.position = GetWorldPosition(position);

            if (IsHost)
            {
                Hp = Type.MaxHp;
            }

            foreach (var passive in Passives)
            {
                passive.OnSpawned(this);
            }
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

        [Rpc(SendTo.ClientsAndHost)]
        public void MoveToRpc(Vector2Int position)
        {
            GridPositionSynchronized = position;
            Tween.Position(transform, GetWorldPosition(position), 0.2f);
        }
        
        public void AddHp(float amount, DamageSource source)
        {
            var cancel = new CancelToken();
            OnHpChange?.Invoke(this, Hp, Hp + amount, source, cancel);
            if (cancel.Canceled) return;
            ModifyHpRpc(amount);
        }

        [Rpc(SendTo.Server)]
        private void ModifyHpRpc(float amount)
        {
            Hp += amount;
        }

        private void Die()
        {
            OnDeath?.Invoke(this);
            CombatManager.Current.DespawnUnit(this);
        }

        private Vector2 GetWorldPosition(Vector2Int position)
        {
            return GetWorldPosition(position, Type.Size);
        }

        public static Vector2 GetWorldPosition(Vector2Int position, Vector2Int size)
        {
            if (!CombatManager.Current.AmIFriendly)
            {
                return Level.Current.CellToWorld(position + new Vector2Int(1 + Mathf.FloorToInt(size.x / 2f),
                    1 + Mathf.FloorToInt(size.y / 2f)));
            }

            return Level.Current.CellToWorld(position);
        }
    }
}