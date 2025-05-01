using System;
using System.Collections.Generic;
using Combat;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Levels
{
    public class Level : MonoBehaviour
    {
        public static Level Current { get; private set; }
        public Tilemap Tilemap => tilemap;

        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Vector2Int levelSize;
        [SerializeField] private TileBase filler;
        [SerializeField] private List<Unit> enemies;

        private void Awake()
        {
            Current = this;
        }

        private void Start()
        {
            var cm = CombatManager.Current;

            foreach (var en in enemies)
            {
                cm.SpawnUnit(en.Type, WorldToCell(en.transform.position), false);
                Destroy(en.gameObject);
            }
            
            enemies.Clear();
        }

        private void OnDestroy()
        {
            Current = null;
        }

        private void OnValidate()
        {
            tilemap.ClearAllTiles();
            
            for (int i = 0; i < levelSize.x; i++)
            {
                for (int j = 0; j < levelSize.y; j++)
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), filler);
                }
            }
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

        public bool InBounds(Vector2Int point, Vector2Int size)
        {
            Debug.Log($"Checking {point} of {size}");
            var result = point.x >= 0 && point.x + size.x <= levelSize.x && point.y >= 0 &&
                         point.y + size.y <= levelSize.y;
            Debug.Log(result);
            return result;
        }
    }
}