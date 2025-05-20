using System;
using Levels;
using PrimeTween;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

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
        private static readonly Color FriendlyColor = new Color(74 / 255f, 194 / 255f, 112 / 255f);
        private static readonly Color UnfriendlyColor = new Color(202 / 255f, 70 / 255f, 92 / 255f);
        
        [SerializeField] private GameObject abilitiesParent;
        [SerializeField] private GameObject passivesParent;
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private SpriteRenderer border;

        public event Action<Unit, CancelToken> OnDeath;
        public event UnitHpChangeEvent OnHpChange;
        public event NewTurnEvent OnNewTurn;
        public event Action<bool> OnInteractabilityChanged;

        [field: SerializeField] public UnitType Type { get; set; }
        public Vector2Int GridPositionSync { get; private set; }
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
                TriggerInteractabilityChangedEventRpc(value);
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void InitRpc(Vector2Int position, bool friendly)
        {
            var combatManager = CombatManager.Current;
            Abilities = abilitiesParent.GetComponents<IAbility>();
            Passives = passivesParent.GetComponents<IPassive>();
            border.color = combatManager.AmIFriendly == friendly ? FriendlyColor : UnfriendlyColor;

            _interactable = true;
            GridPositionSync = position;
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
            GridPositionSync = position;
            Tween.Position(transform, GetWorldPosition(position), 0.2f);
        }

        [Rpc(SendTo.Server)]
        public void AddHpRpc(float amount, DamageSource source)
        {
            var cancel = new CancelToken();
            OnHpChange?.Invoke(this, Hp, Hp + amount, source, cancel);
            if (cancel.Canceled) return;
            Hp += amount;
        }

        private void Die()
        {
            TriggerDeathEventRpc();
        }
        
        private Vector2 GetWorldPosition(Vector2Int position)
        {
            return GetWorldPosition(position, Type.Size);
        }

        public static Vector2 GetWorldPosition(Vector2Int position, Vector2Int size)
        {
            if (!CombatManager.Current.AmIFriendly)
            {
                return Level.Current.CellToWorld(position + size);
            }

            return Level.Current.CellToWorld(position);
        }
        
        [Rpc(SendTo.Server)]
        private void TriggerDeathEventRpc()
        {
            var cancelToken = new CancelToken();
            OnDeath?.Invoke(this, cancelToken);
            if (cancelToken.Canceled) return;
            CombatManager.Current.DespawnUnit(this, true);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void TriggerInteractabilityChangedEventRpc(bool value)
        {
            OnInteractabilityChanged?.Invoke(value);
        }
    }
}