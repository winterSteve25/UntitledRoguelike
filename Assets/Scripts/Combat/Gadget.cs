using Levels;
using UnityEngine;

namespace Combat
{
    public abstract class Gadget : MonoBehaviour
    {
        public Vector2Int GridPosition { get; private set; }
        public abstract Vector2Int GridSize { get; }

        private int _duration;

        public void Init(int duration, Vector2Int position)
        {
            _duration = duration;
            GridPosition = position;
            transform.position = Level.Current.CellToWorld(position);
        }
        
        public void NextTurn(bool friendlyTurn)
        {
            OnTurn(friendlyTurn);
            _duration--;
            if (_duration <= 0)
            {
                CombatManager.Current.RemoveGadget(this);
            }
        }

        protected virtual void OnTurn(bool friendlyTurn)
        {
        }
    }
}