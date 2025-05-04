using UnityEngine;
using UnityEngine.EventSystems;

namespace Deck
{
    public class Slot : MonoBehaviour
    {
        public Vector2Int Pos { get; private set; }

        public void Init(int x, int y)
        {
            Pos = new Vector2Int(x, y);
        }
    }
}