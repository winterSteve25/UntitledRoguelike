using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Deck;
using Combat.UI;
using Levels;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using Utils;

namespace Combat
{
    public class CombatManager : NetworkBehaviour
    {
        public static CombatManager Current { get; private set; }

        public int TurnNumberSync { get; private set; } // synchronized manually via NextTurnRpc
        public bool AmIFriendly { get; private set; }
        public bool IsFriendlyTurn => TurnNumberSync % 2 == 0;
        public int MaxEnergy => maxEnergy;
        public Player Me => AmIFriendly ? playerFriendly : playerUnfriendly;
        public bool MyTurn => AmIFriendly == IsFriendlyTurn;
        public List<Unit> ActiveUnits => _activeUnitsSync;
        public List<Gadget> ActiveGadgets => _activeGadgetsSync;

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

        // manually synchronized via Rpc calls
        private List<Unit> _activeUnitsSync;
        private List<Gadget> _activeGadgetsSync;

        private void Awake()
        {
            Current = this;
            AmIFriendly = NetworkManager.Singleton.IsHost;

            playerFriendly.Init(inventorySize, maxEnergy);
            playerUnfriendly.Init(inventorySize, maxEnergy);

            if (AmIFriendly)
            {
                myInventoryUI.Init(inventorySize, playerFriendly.Inventory);
                opponentInventoryUI.Init(inventorySize, playerUnfriendly.Inventory);
            }
            else
            {
                myInventoryUI.Init(inventorySize, playerUnfriendly.Inventory);
                opponentInventoryUI.Init(inventorySize, playerFriendly.Inventory);
            }

            _activeUnitsSync = new List<Unit>();
            _activeGadgetsSync = new List<Gadget>();

            TurnNumberSync = 0;
        }

        private void Start()
        {
            if (IsServer)
            {
                NextTurnRpc();
            }

            if (AmIFriendly) return;

            var levelTransform = Level.Current.transform;
            levelTransform.position *= -1;
            levelTransform.rotation = Quaternion.Euler(0, 0, 180);
            // flip the tiles again to counter the clip of the object
            var x4 = Level.Current.Tilemap.orientationMatrix;
            x4 = Matrix4x4.TRS(x4.GetPosition(), Quaternion.Euler(0, 0, 180), x4.lossyScale);
            Level.Current.Tilemap.orientationMatrix = x4;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnTurnChanged = null;
            Current = null;
        }

        public Player GetPlayer(bool friendly)
        {
            return friendly ? playerFriendly : playerUnfriendly;
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void NextTurnRpc()
        {
            TurnNumberSync++;
            OnTurnChanged?.Invoke(TurnNumberSync, IsFriendlyTurn);
            selectedUnitUI.UnitInteractableChange(IsFriendlyTurn);
            Me.Energy = maxEnergy;

            if (IsServer)
            {
                foreach (var unit in _activeUnitsSync)
                {
                    unit.NextTurn(IsFriendlyTurn);
                }

                for (var i = 0; i < _activeGadgetsSync.Count; i++)
                {
                    var gadget = _activeGadgetsSync[i];
                    if (gadget.NextTurn(IsFriendlyTurn))
                    {
                        RemoveGadgetFromListForOthersRpc(gadget);
                        DespawnNetworkObjectRpc(gadget.NetworkObject);
                        _activeGadgetsSync.RemoveAt(i);
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
            _activeUnitsSync.Add(u);
        }

        public void DespawnUnit(Unit unit, bool isDeath)
        {
            RemoveUnitFromListRpc(unit);
            DespawnNetworkObjectRpc(unit.NetworkObject);

            // TODO
            // WHEN SLIME DIES IT SPLITS BUT IT WILL FIRST DESPAWN AND TRIGGER DEATH..
            // if (!isDeath)
            // {
            //     return;
            // }
            //
            // bool? f = null;
            // foreach (var u in _activeUnitsSync)
            // {
            //     if (u == unit) continue;
            //     if (f == null)
            //     {
            //         f = u.Friendly;
            //         continue;
            //     }
            //
            //     // checks if every remaining unit is on 1 team
            //     if (u.Friendly == f) continue;
            //
            //     // if one is not the same team then stop right here
            //     return;
            // }
            //
            // // if the code reaches here all units are on the same team
            // // then the opposing team loses
            // NetworkManager.Shutdown();
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
            if (selectedUnitUI.Showing == u)
            {
                selectedUnitUI.Show(null);
            }
            
            _activeUnitsSync.Remove(u);
        }

        public Unit GetUnit(int x, int y)
        {
            return _activeUnitsSync.FirstOrDefault(u =>
                u.GridPositionSync.x <= x && u.GridPositionSync.y <= y &&
                x < u.GridPositionSync.x + u.Type.Size.x && y < u.GridPositionSync.y + u.Type.Size.y);
        }

        public bool CanPlaceUnitAt(UnitType unitType, Vector2Int position)
        {
            return level.InBounds(position, unitType.Size, !AmIFriendly) &&
                   _activeUnitsSync.TrueForAll(u =>
                       !RectangleTester.AreRectanglesOverlapping(
                           u.GridPositionSync.x, u.GridPositionSync.y,
                           u.Type.Size.x, u.Type.Size.y,
                           position.x, position.y,
                           unitType.Size.x, unitType.Size.y));
        }

        public bool CanMoveTo(Unit unit, Vector2Int position)
        {
            if (!Level.Current.InBounds(position, unit.Type.Size, !AmIFriendly)) return false;

            foreach (var u in _activeUnitsSync)
            {
                var overlap = RectangleTester.AreRectanglesOverlapping(position.x, position.y, unit.Type.Size.x,
                    unit.Type.Size.y, u.GridPositionSync.x, u.GridPositionSync.y, u.Type.Size.x,
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

        public List<Unit> GetUnitsInArea(Vector2Int position, Vector2Int size)
        {
            var lst = new List<Unit>();

            foreach (var unit in _activeUnitsSync)
            {
                if (!RectangleTester.AreRectanglesOverlapping(
                        position.x, position.y,
                        size.x, size.y,
                        unit.GridPositionSync.x, unit.GridPositionSync.y,
                        unit.Type.Size.x, unit.Type.Size.y)) continue;
                
                lst.Add(unit);
            }

            return lst;
        }

        public void SpawnGadget(Gadget prefab, Vector2Int position)
        {
            var pos = AmIFriendly
                ? position
                : position - new Vector2Int(Mathf.FloorToInt(prefab.GridSize.x / 2f),
                    Mathf.FloorToInt(prefab.GridSize.y / 2f));

            SpawnGadgetRpc(prefab.name, pos);
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
            _activeGadgetsSync.Add(g);
        }

        [Rpc(SendTo.NotMe)]
        private void RemoveGadgetFromListForOthersRpc(NetworkBehaviourReference unit)
        {
            if (!unit.TryGet(out Gadget g, NetworkManager)) return;
            _activeGadgetsSync.Remove(g);
        }
    }
}