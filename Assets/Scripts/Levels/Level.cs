using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Levels
{
    public class Level : MonoBehaviour
    {
        public static Level Current { get; private set; }
        
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Vector2Int levelSize;
        [SerializeField] private TileBase filler;

        private void Awake()
        {
            Current = this;
        }

        private void Start()
        {
            for (int i = 0; i < levelSize.x; i++)
            {
                for (int j = 0; j < levelSize.y; j++)
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), filler);
                }
            }
        }

        private void OnDestroy()
        {
            Current = null;
        }

        public Vector3 CellToWorld(Vector2Int pos)
        {
            return tilemap.CellToWorld(new Vector3Int(pos.x, pos.y, 0));
        }

        public bool InBounds(Vector2Int point, Vector2Int size)
        {
            return point.x >= 0 && point.x + size.x < levelSize.x && point.y >= 0 && point.y + size.y < levelSize.y;
        }
    }
}