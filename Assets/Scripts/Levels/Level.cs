using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace Levels
{
    public class Level : MonoBehaviour
    {
        public static Level Current { get; private set; }
        public Tilemap Tilemap => tilemap;
        public Vector2Int LevelSize => levelSize;
        
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Vector2Int levelSize;
        [SerializeField] private TileBase filler;

        private void Awake()
        {
            Current = this;
        }

        private void OnDestroy()
        {
            Current = null;
        }

        private void OnValidate()
        {
            // tilemap.ClearAllTiles();
            //
            // for (int i = 0; i < levelSize.x; i++)
            // {
            //     for (int j = 0; j < levelSize.y; j++)
            //     {
            //         tilemap.SetTile(new Vector3Int(i, j, 0), filler);
            //     }
            // }
        }

        public Vector3 CellToWorld(Vector2Int pos)
        {
            return tilemap.CellToWorld(new Vector3Int(pos.x, pos.y, 0));
        }

        public Vector2Int WorldToCell(Vector3 pos)
        {
            var gp = tilemap.WorldToCell(pos);
            return new Vector2Int(gp.x, gp.y);
        }

        public bool InBounds(Vector2Int point, Vector2Int size, bool upsideDown)
        {
            return RectangleTester.InBound(levelSize, Vector2Int.zero, point.x, point.y, size.x, size.y, true, upsideDown);
        }
    }
}