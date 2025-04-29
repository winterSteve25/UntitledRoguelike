using System.Collections.Generic;
using System.Linq;
using Levels;
using UnityEngine;

namespace Combat
{
    public class CombatManager : MonoBehaviour
    {
        [SerializeField] private Level level;

        private List<Unit> _activeUnits;

        private void Start()
        {
            _activeUnits = new List<Unit>();
        }

        public Unit Spawn(UnitType unitType, Vector2Int position, bool friendly)
        {
            var unit = Instantiate(unitType.Prefab);
            unit.Init(unitType, position, level.CellToWorld(position), friendly);
            _activeUnits.Add(unit);
            return unit;
        }

        public Unit GetUnit(int x, int y)
        {
            return _activeUnits.FirstOrDefault(u =>
                u.GridPosition.x <= x && u.GridPosition.y <= y &&
                x < u.GridPosition.x + u.Type.Size.x && y < u.GridPosition.y + u.Type.Size.y);
        }

        public bool CanPlaceUnitAt(UnitType unitType, Vector2Int position)
        {
            return level.InBounds(position, unitType.Size) &&
                   _activeUnits.TrueForAll(u => !AreRectanglesOverlapping(u.GridPosition.x, u.GridPosition.y, u.Type.Size.x,
                       u.Type.Size.y,
                       position.x, position.y, unitType.Size.x, unitType.Size.y));
        }

        private static bool AreRectanglesOverlapping(int x1, int y1, int width1, int height1,
            int x2, int y2, int width2, int height2)
        {
            // Calculate the sides of rectangle 1
            int left1 = x1;
            int right1 = x1 + width1;
            int top1 = y1;
            int bottom1 = y1 + height1;

            // Calculate the sides of rectangle 2
            int left2 = x2;
            int right2 = x2 + width2;
            int top2 = y2;
            int bottom2 = y2 + height2;

            // Check if one rectangle is to the left of the other
            if (right1 <= left2 || right2 <= left1)
                return false;

            // Check if one rectangle is above the other
            if (bottom1 <= top2 || bottom2 <= top1)
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
            _activeUnits.Remove(unit);
        }
    }
}