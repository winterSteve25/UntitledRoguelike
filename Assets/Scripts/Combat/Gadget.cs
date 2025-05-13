using Levels;
using Unity.Netcode;
using UnityEngine;

namespace Combat
{
    public abstract class Gadget : NetworkBehaviour
    {
        public Vector2Int GridPositionSync { get; private set; }
        public abstract Vector2Int GridSize { get; }

        [SerializeField] private int duration;

        [Rpc(SendTo.ClientsAndHost)]
        public void InitRpc(Vector2Int position)
        {
            GridPositionSync = position;
            transform.position = Unit.GetWorldPosition(position, GridSize);
        }
        
        public bool NextTurn(bool friendlyTurn)
        {
            OnTurn(friendlyTurn);
            duration--;
            if (duration <= 0)
            {
                return true;
            }

            return false;
        }

        protected virtual void OnTurn(bool friendlyTurn)
        {
        }
    }
}