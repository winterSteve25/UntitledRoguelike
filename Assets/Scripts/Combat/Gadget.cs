using Levels;
using UnityEngine;
using UnityEngine.Serialization;

namespace Combat
{
    public abstract class Gadget : MonoBehaviour
    {
        public Vector2Int GridPosition { get; private set; }
        public abstract Vector2Int GridSize { get; }

        [SerializeField] private int duration;

        public void Init(Vector2Int position)
        {
            GridPosition = position;
            transform.position = Level.Current.CellToWorld(position);
        }
        
        public void NextTurn(bool friendlyTurn)
        {
            OnTurn(friendlyTurn);
            duration--;
            if (duration <= 0)
            {
                CombatManager.Current.RemoveGadget(this);
            }
        }

        protected virtual void OnTurn(bool friendlyTurn)
        {
        }
    }
}