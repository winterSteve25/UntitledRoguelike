using System;
using System.Collections.Generic;
using System.Linq;
using Deck;
using Levels;
using Unity.Netcode;
using UnityEngine;
using Utils;

namespace Combat
{
    public class CombatManager : NetworkBehaviour
    {
        public static CombatManager Current { get; private set; }

        public int TurnNumberSynchronized { get; private set; } // synchronized manually via NextTurnRpc
        public bool AmIFriendly { get; private set; }
        public bool IsFriendlyTurn => TurnNumberSynchronized % 2 == 0;
        public Player Me => AmIFriendly ? _playerFriendly : _playerUnfriendly;
        public bool MyTurn => AmIFriendly == IsFriendlyTurn;

        public event Action<int, bool> OnTurnChanged;

        [Header("Configurations")] 
        [SerializeField] private int maxEnergy;
        [SerializeField] private Vector2Int inventorySize;

        [Header("References")] 
        [SerializeField] private Level level;
        [SerializeField] private CombatInfoUI infoUI;
        [SerializeField] private SelectedUnitUI selectedUnitUI;
        [SerializeField] private InventoryUI inventoryUI;

        private List<Unit> _activeUnitsSynchronized; // manually synchronized via RegisterUnit
        private DeferredRemovalList<Gadget> _activeGadgets;
        private Player _playerFriendly;
        private Player _playerUnfriendly;

        private void Awake()
        {
            Current = this;

            _playerFriendly = new Player(inventorySize, maxEnergy, true);
            _playerUnfriendly = new Player(inventorySize, maxEnergy, false);

            inventoryUI.Init(inventorySize, _playerFriendly.Inventory);
            AmIFriendly = NetworkManager.Singleton.IsHost;
        }

        private void Start()
        {
            if (!AmIFriendly)
            {
                var levelTransform = Level.Current.transform;
                levelTransform.position *= -1;
                levelTransform.rotation = Quaternion.Euler(0, 0, 180);
            }

            _activeUnitsSynchronized = new List<Unit>();
            _activeGadgets = new DeferredRemovalList<Gadget>();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnTurnChanged = null;
            Current = null;
        }

        public void NextTurn()
        {
            NextTurnRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void NextTurnRpc()
        {
            TurnNumberSynchronized++;
            OnTurnChanged?.Invoke(TurnNumberSynchronized, IsFriendlyTurn);
            selectedUnitUI.UnitInteractableChange(IsFriendlyTurn);
            Me.Energy = maxEnergy;

            foreach (var unit in _activeUnitsSynchronized)
            {
                unit.NextTurn(IsFriendlyTurn);
            }

            foreach (var gadget in _activeGadgets)
            {
                gadget.NextTurn(IsFriendlyTurn);
            }

            _activeGadgets.Flush();
        }

        public void SpawnUnit(UnitType unitType, Vector2Int position, bool friendly)
        {
            SpawnUnitRpc(unitType.name,
                AmIFriendly
                    ? position
                    : position - new Vector2Int(Mathf.FloorToInt(unitType.Size.x / 2f),
                        Mathf.FloorToInt(unitType.Size.y / 2f)),
                friendly);
        }

        // WARNING: THIS SHOULD ONLY BE CALLED FROM Unit.Init
        public void RegisterUnit(Unit unit)
        {
            _activeUnitsSynchronized.Add(unit);
        }

        [Rpc(SendTo.Server)]
        private void SpawnUnitRpc(string unitType, Vector2Int position, bool friendly)
        {
            var obj = NetworkManager.SpawnManager.InstantiateAndSpawn(
                Resources.Load<NetworkObject>($"UnitTypes/{unitType}"), NetworkManager.LocalClientId, true);
            var unit = obj.GetComponent<Unit>();
            unit.Init(position, friendly);
        }
        
        public void DespawnUnit(Unit unit)
        {
            unit.RemoveSelf();
        }

        // WARNING: THIS SHOULD ONLY BE CALLED FROM Unit.RemoveSelf
        public void RemoveUnit(Unit unit)
        {
            _activeUnitsSynchronized.Remove(unit);
        }
        
        public Unit GetUnit(int x, int y)
        {
            return _activeUnitsSynchronized.FirstOrDefault(u =>
                u.GridPosition.x <= x && u.GridPosition.y <= y &&
                x < u.GridPosition.x + u.Type.Size.x && y < u.GridPosition.y + u.Type.Size.y);
        }

        public bool CanPlaceUnitAt(UnitType unitType, Vector2Int position)
        {
            return level.InBounds(position, unitType.Size, !AmIFriendly) &&
                   _activeUnitsSynchronized.TrueForAll(u =>
                       !RectangleTester.AreRectanglesOverlapping(
                           u.GridPosition.x, u.GridPosition.y,
                           u.Type.Size.x, u.Type.Size.y,
                           position.x, position.y,
                           unitType.Size.x, unitType.Size.y));
        }

        public bool CanMoveTo(Unit unit, Vector2Int position)
        {
            if (!Level.Current.InBounds(position, unit.Type.Size, !AmIFriendly)) return false;

            foreach (var u in _activeUnitsSynchronized)
            {
                var overlap = RectangleTester.AreRectanglesOverlapping(position.x, position.y, unit.Type.Size.x,
                    unit.Type.Size.y, u.GridPosition.x, u.GridPosition.y, u.Type.Size.x, u.Type.Size.y);

                if (!overlap) continue;
                if (ReferenceEquals(unit, u)) continue;

                return false;
            }

            return true;
        }

        public bool TryGetUnit(int x, int y, out Unit unit)
        {
            unit = GetUnit(x, y);
            return unit != null;
        }

        public Gadget SpawnGadget(Gadget prefab)
        {
            var gadget = Instantiate(prefab);
            _activeGadgets.Add(gadget);
            return gadget;
        }

        public void RemoveGadget(Gadget gadget)
        {
            _activeGadgets.Remove(gadget);
            Destroy(gadget.gameObject);
        }
    }
}