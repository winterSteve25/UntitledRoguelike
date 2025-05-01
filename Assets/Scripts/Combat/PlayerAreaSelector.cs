using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Levels;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

namespace Combat
{
    public class PlayerAreaSelector : MonoBehaviour, IAreaSelector
    {
        public static PlayerAreaSelector Current { get; private set; }

        [SerializeField] private Camera cam;
        [SerializeField] private SelectedUnitUI selectedUnitUI;
        [SerializeField] private GameObject movablePrefab;

        private bool _ready;
        private bool _selecting;
        private Vector2Int _selected;

        private Vector2Int _center;
        private Vector2Int _destSize;
        private SpotSelectionMode _mode;
        private int _radius;
        private Predicate<Vector2Int> _isValid;

        private IObjectPool<GameObject> _moveablePool;
        private List<GameObject> _activeObjects;

        private void Awake()
        {
            Current = this;
        }

        private void Start()
        {
            _moveablePool = new ObjectPool<GameObject>(
                () => Instantiate(movablePrefab, transform),
                obj => obj.SetActive(true),
                obj => obj.SetActive(false),
                Destroy,
                false
            );
            _activeObjects = new List<GameObject>();
        }

        private void OnDestroy()
        {
            Current = null;
        }

        private void Update()
        {
            if (!_selecting || EventSystem.current.IsPointerOverGameObject())
                return;

            if (!Mouse.current.leftButton.wasPressedThisFrame) return;

            var screenPos = Mouse.current.position.ReadValue();
            var worldPos = cam.ScreenToWorldPoint(screenPos);
            worldPos.z = 0;
            var clickedTile = Level.Current.WorldToCell(worldPos);

            if (_isValid(clickedTile) && IsValid(clickedTile))
            {
                _selected = clickedTile;
                _ready = true;
                _selecting = false;
            }
        }

        // selects the an area of destinationSize size in a r*r radius from the center
        // if the destination size is > 1x1, the mouse will select the center.
        // only the center has to within the radius
        // if the destination size is non symmetric => destSize.x != destSize.y 
        // then the mouse will select from the top left corner => only top left has to be within radius.
        public async UniTask<Vector2Int> SelectArea(Vector2Int center, Vector2Int destinationSize, int radius,
            Predicate<Vector2Int> isValid, SpotSelectionMode mode = SpotSelectionMode.Omnidirectional)
        {
            _center = center;
            _destSize = destinationSize;
            _radius = radius;
            _mode = mode;
            _isValid = isValid;

            _ready = false;
            _selecting = true;

            var wasShowing = selectedUnitUI.Showing;
            selectedUnitUI.Show(null);
            selectedUnitUI.CanOpenMenu(false);

            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    var pos = new Vector2Int(center.x + i, center.y + j);
                    
                    var wp = Level.Current.CellToWorld(pos);
                    var visual = _moveablePool.Get();
                    visual.transform.position = wp;
                    _activeObjects.Add(visual);
                    visual.GetComponentInChildren<SpriteRenderer>().color = Color.white;
                    
                    if (!_isValid(pos))
                    {
                        visual.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                    }
                    
                    if (!IsValid(pos))
                    {
                        visual.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
                    }
                }
            }

            await UniTask.WaitUntil(() => _ready);

            selectedUnitUI.CanOpenMenu(true);
            foreach (var o in _activeObjects)
            {
                _moveablePool.Release(o);
            }

            if (wasShowing != null) selectedUnitUI.Show(wasShowing);
            _activeObjects.Clear();

            return _selected;
        }

        private bool IsValid(Vector2Int target)
        {
            if (target == _center) return false;
            if (target.x - _center.x > _radius || target.y - _center.y > _radius)
                return false;

            if (_mode == SpotSelectionMode.Straight)
            {
                if ((target.x != _center.x) == (target.y != _center.y)) return false;
            }
            else if (_mode == SpotSelectionMode.Diagonal)
            {
                var rel = target - _center;
                if (rel.x != rel.y) return false;
            }

            return true;
        }
    }

    public enum SpotSelectionMode
    {
        Straight, // like rook
        Diagonal, // like bishop
        Omnidirectional // like queen
    }
}