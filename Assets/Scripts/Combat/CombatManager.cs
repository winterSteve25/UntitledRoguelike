using System;
using System.Collections.Generic;
using System.Linq;
using Levels;
using UnityEngine;

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
            }
        }

        [SerializeField] private int maxEnergy;
        [SerializeField] private Level level;
        [SerializeField] private CombatInfoUI infoUI;

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
            unit.Init(unitType, position, level.CellToWorld(position), friendly);
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
                       !AreRectanglesOverlapping(
                           u.GridPosition.x, u.GridPosition.y,
                           u.Type.Size.x, u.Type.Size.y,
                           position.x, position.y,
                           unitType.Size.x, unitType.Size.y));
        }

        private static bool AreRectanglesOverlapping(int x1, int y1, int width1, int height1,
            int x2, int y2, int width2, int height2)
        {
            // Rectangle 1 bounds
            int left1 = x1;
            int right1 = x1 + width1;
            int bottom1 = y1;
            int top1 = y1 + height1;

            // Rectangle 2 bounds
            int left2 = x2;
            int right2 = x2 + width2;
            int bottom2 = y2;
            int top2 = y2 + height2;

            // If one rectangle is to the left of the other
            if (right1 <= left2 || right2 <= left1)
                return false;

            // If one rectangle is below the other
            if (top1 <= bottom2 || top2 <= bottom1)
                return false;

            // Rectangles overlap
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