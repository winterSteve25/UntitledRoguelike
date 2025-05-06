using System;
using System.Collections.Generic;
using System.Linq;
using Deck;
using Levels;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Combat
{
    public class CombatManager : NetworkBehaviour
    {
        public static CombatManager Current { get; private set; }

        public int TurnNumberSynchronized { get; private set; } // synchronized manually via NextTurnRpc
        public bool AmIFriendly { get; private set; }
        public bool IsFriendlyTurn => TurnNumberSynchronized % 2 == 0;
        public Player Me => AmIFriendly ? playerFriendly : playerUnfriendly;
        public bool MyTurn => AmIFriendly == IsFriendlyTurn;
        public List<Unit> ActiveUnits => _activeUnitsSynchronized;

        public event Action<int, bool> OnTurnChanged;

        [Header("Configurations")] [SerializeField]
        private int maxEnergy;

        [SerializeField] private Vector2Int inventorySize;

        [Header("References")] [SerializeField]
        private Level level;

        [SerializeField] private CombatInfoUI infoUI;
        [SerializeField] private SelectedUnitUI selectedUnitUI;
        [SerializeField] private InventoryUI myInventoryUI;
        [SerializeField] private InventoryUI opponentInventoryUI;
        
        [SerializeField] private Player playerFriendly;
        [SerializeField] private Player playerUnfriendly;

        private List<Unit> _activeUnitsSynchronized; // manually synchronized via Rpc calls
        private List<Gadget> _activeGadgets;

        private void Awake()
        {
            Current = this;

            playerFriendly.Init(inventorySize, maxEnergy, true);
            playerUnfriendly.Init(inventorySize, maxEnergy, false);
            myInventoryUI.Init(inventorySize, playerFriendly.Inventory);
            opponentInventoryUI.Init(inventorySize, playerUnfriendly.Inventory);
            
            AmIFriendly = NetworkManager.Singleton.IsHost;

            _activeUnitsSynchronized = new List<Unit>();
            _activeGadgets = new List<Gadget>();

            TurnNumberSynchronized = 0;
        }

        private void Start()
        {
            if (AmIFriendly) return;
            var levelTransform = Level.Current.transform;
            levelTransform.position *= -1;
            levelTransform.rotation = Quaternion.Euler(0, 0, 180);
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

            if (IsServer)
            {
                for (var i = 0; i < _activeGadgets.Count; i++)
                {
                    var gadget = _activeGadgets[i];
                    if (gadget.NextTurn(IsFriendlyTurn))
                    {
                        RemoveGadgetFromListForOthersRpc(gadget);
                        DespawnNetworkObjectRpc(gadget.NetworkObject);
                        _activeGadgets.RemoveAt(i);
                        i--;
                    }
                }
            }
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

        [Rpc(SendTo.Server)]
        private void SpawnUnitRpc(string unitType, Vector2Int position, bool friendly, RpcParams rpcParams = default)
        {
            var obj = NetworkManager.SpawnManager.InstantiateAndSpawn(
                Resources.Load<NetworkObject>($"UnitTypes/{unitType}"), rpcParams.Receive.SenderClientId, true);
            var unit = obj.GetComponent<Unit>();
            unit.InitRpc(position, friendly);
            AddUnitToListRpc(unit);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void AddUnitToListRpc(NetworkBehaviourReference unit)
        {
            if (!unit.TryGet(out Unit u, NetworkManager)) return;
            _activeUnitsSynchronized.Add(u);
        }

        public void DespawnUnit(Unit unit)
        {
            RemoveUnitFromListRpc(unit);
            DespawnNetworkObjectRpc(unit.NetworkObject);
        }

        [Rpc(SendTo.Server)]
        private void DespawnNetworkObjectRpc(NetworkObjectReference unit)
        {
            if (!unit.TryGet(out var obj, NetworkManager)) return;
            obj.Despawn();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void RemoveUnitFromListRpc(NetworkBehaviourReference unit)
        {
            if (!unit.TryGet(out Unit u, NetworkManager)) return;
            _activeUnitsSynchronized.Remove(u);
        }

        public Unit GetUnit(int x, int y)
        {
            return _activeUnitsSynchronized.FirstOrDefault(u =>
                u.GridPositionSynchronized.x <= x && u.GridPositionSynchronized.y <= y &&
                x < u.GridPositionSynchronized.x + u.Type.Size.x && y < u.GridPositionSynchronized.y + u.Type.Size.y);
        }

        public bool CanPlaceUnitAt(UnitType unitType, Vector2Int position)
        {
            return level.InBounds(position, unitType.Size, !AmIFriendly) &&
                   _activeUnitsSynchronized.TrueForAll(u =>
                       !RectangleTester.AreRectanglesOverlapping(
                           u.GridPositionSynchronized.x, u.GridPositionSynchronized.y,
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
                    unit.Type.Size.y, u.GridPositionSynchronized.x, u.GridPositionSynchronized.y, u.Type.Size.x,
                    u.Type.Size.y);

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

        public void SpawnGadget(Gadget prefab, Vector2Int position)
        {
            SpawnGadgetRpc(prefab.name, position);
        }

        [Rpc(SendTo.Server)]
        private void SpawnGadgetRpc(string gadgetPrefabName, Vector2Int position, RpcParams rpcParams = default)
        {
            var obj = NetworkManager.SpawnManager.InstantiateAndSpawn(
                Resources.Load<NetworkObject>($"Gadgets/{gadgetPrefabName}"), rpcParams.Receive.SenderClientId, true);

            var gadget = obj.GetComponent<Gadget>();
            gadget.InitRpc(position);
            AddGadgetToListRpc(gadget);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void AddGadgetToListRpc(NetworkBehaviourReference gadget)
        {
            if (!gadget.TryGet(out Gadget g, NetworkManager)) return;
            _activeGadgets.Add(g);
        }

        [Rpc(SendTo.NotMe)]
        private void RemoveGadgetFromListForOthersRpc(NetworkBehaviourReference unit)
        {
            if (!unit.TryGet(out Gadget g, NetworkManager)) return;
            _activeGadgets.Remove(g);
        }
    }
}