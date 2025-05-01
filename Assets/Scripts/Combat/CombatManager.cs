using System;
using System.Collections.Generic;
using System.Linq;
using Levels;
using UnityEngine;
using Utils;

namespace Combat
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Current { get; private set; }

        public List<Unit> ActiveUnits { get; private set; }
        public bool FriendlyTurn { get; private set; }
        public int TurnNumber { get; private set; }

        public event Action<int, bool> OnTurnChanged;

        private int _playerEnergy;

        public int PlayerEnergy
        {
            get => _playerEnergy;
            set
            {
                _playerEnergy = value;
                infoUI.UpdateEnergy(_playerEnergy, maxEnergy);
                selectedUnitUI.UpdateEnergy(_playerEnergy);
            }
        }

        [SerializeField] private int maxEnergy;
        [SerializeField] private Level level;
        [SerializeField] private CombatInfoUI infoUI;
        [SerializeField] private SelectedUnitUI selectedUnitUI;

        private void Awake()
        {
            Current = this;
        }

        private void Start()
        {
            ActiveUnits = new List<Unit>();
        }

        private void OnDestroy()
        {
            OnTurnChanged = null;
            Current = null;
        }

        public void NextTurn()
        {
            TurnNumber++;
            FriendlyTurn = TurnNumber % 2 == 0;
            OnTurnChanged?.Invoke(TurnNumber, FriendlyTurn);
            selectedUnitUI.UnitInteractableChange(FriendlyTurn);

            if (FriendlyTurn)
            {
                PlayerEnergy = maxEnergy;
            }

            foreach (var unit in ActiveUnits)
            {
                unit.NextTurn(FriendlyTurn);
            }
        }

        public Unit SpawnUnit(UnitType unitType, Vector2Int position, bool friendly)
        {
            var unit = Instantiate(unitType.Prefab);
            unit.Init(position, level.CellToWorld(position), friendly);
            ActiveUnits.Add(unit);
            return unit;
        }

        public Unit GetUnit(int x, int y)
        {
            return ActiveUnits.FirstOrDefault(u =>
                u.GridPosition.x <= x && u.GridPosition.y <= y &&
                x < u.GridPosition.x + u.Type.Size.x && y < u.GridPosition.y + u.Type.Size.y);
        }

        public bool CanPlaceUnitAt(UnitType unitType, Vector2Int position)
        {
            return level.InBounds(position, unitType.Size) &&
                   ActiveUnits.TrueForAll(u =>
                       !RectangleTester.AreRectanglesOverlapping(
                           u.GridPosition.x, u.GridPosition.y,
                           u.Type.Size.x, u.Type.Size.y,
                           position.x, position.y,
                           unitType.Size.x, unitType.Size.y));
        }

        public bool CanMoveTo(Unit unit, Vector2Int position)
        {
            if (!Level.Current.InBounds(position, unit.Type.Size)) return false;
            
            foreach (var u in ActiveUnits)
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

        public void RemoveUnit(Unit unit)
        {
            ActiveUnits.Remove(unit);
            Destroy(unit.gameObject);
        }
    }
}