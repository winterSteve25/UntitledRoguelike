using System;
using System.Collections.Generic;
using Combat.UI;
using Cysharp.Threading.Tasks;
using Levels;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using Utils;

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
        private bool _canceled;
        private Vector2Int? _selected;

        private Vector2Int _center;
        private Vector2Int _centerSize;
        private Vector2Int _targetSize;
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

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CancelSelection();
                return;
            }

            if (!Mouse.current.leftButton.wasPressedThisFrame) return;

            var screenPos = Mouse.current.position.ReadValue();
            var worldPos = cam.ScreenToWorldPoint(screenPos);
            worldPos.z = 0;
            var clickedTile = Level.Current.WorldToCell(worldPos);
            var flipboard = !CombatManager.Current.AmIFriendly;

            var dx = Mathf.FloorToInt(_targetSize.x * 0.5f);
            var dy = Mathf.FloorToInt(_targetSize.y * 0.5f);

            if (flipboard)
            {
                clickedTile -= new Vector2Int(dx, dy);
            }

            if (_isValid(clickedTile) && IsValid(clickedTile, flipboard))
            {
                _selected = clickedTile;
                _ready = true;
                _selecting = false;
            }
        }

        public void CancelSelection()
        {
            _selected = null;
            _ready = false;
            _selecting = false;
            _canceled = true;
        }

        // selects the an area of destinationSize size in a r*r radius from the center
        // if the destination size is > 1x1, the mouse will select the center.
        // only the center has to within the radius
        // if the destination size is non symmetric => destSize.x != destSize.y 
        // then the mouse will select from the bottom left corner => only bottom left has to be within radius.
        public async UniTask<Vector2Int?> SelectArea(
            Vector2Int center,
            Vector2Int centerSize,
            Vector2Int targetSize,
            int radius,
            Predicate<Vector2Int> isValid,
            SpotSelectionMode mode = SpotSelectionMode.Omnidirectional
        )
        {
            var flipboard = !CombatManager.Current.AmIFriendly;

            _center = center;
            _centerSize = centerSize;
            _targetSize = targetSize;
            _radius = radius;
            _mode = mode;
            _isValid = isValid;
            _canceled = false;

            _ready = false;
            _selecting = true;

            selectedUnitUI.Show(null);
            selectedUnitUI.CanOpenMenu(false);

            var dx = Mathf.FloorToInt(_targetSize.x * 0.5f);
            var dy = Mathf.FloorToInt(_targetSize.y * 0.5f);

            for (int i = -radius; i <= radius + dx; i++)
            {
                for (int j = -radius; j <= radius + dy; j++)
                {
                    var pos = new Vector2Int(center.x + i, center.y + j);
                    var wp = Unit.GetWorldPosition(pos, Vector2Int.one);
                    
                    if (flipboard)
                    {
                        pos -= new Vector2Int(dx, dy);
                    }

                    if (!_isValid(pos))
                    {
                        continue;
                    }

                    if (!IsValid(pos, flipboard))
                    {
                        continue;
                    }

                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    
                    var visual = _moveablePool.Get();
                    visual.transform.position = wp;
                    _activeObjects.Add(visual);
                    visual.GetComponentInChildren<SpriteRenderer>().color = Color.gray;
                }
            }

            await UniTask.WaitUntil(() => _ready || _canceled);

            selectedUnitUI.CanOpenMenu(true);
            foreach (var o in _activeObjects)
            {
                _moveablePool.Release(o);
            }

            _activeObjects.Clear();

            return _selected;
        }

        private bool IsValid(Vector2Int pos, bool flipboard)
        {
            return IsValid(_center, _centerSize, pos, _targetSize, _radius, _mode, flipboard);
        }
        
       private static bool IsValid(Vector2Int center, Vector2Int centerSize, Vector2Int target, Vector2Int targetSize,
            int radius, SpotSelectionMode mode, bool flipboard)
        {
            // TODO:
            // always false on upside down because the selector already re-anchors the position selected to orientate
            // lowk no idea why the other ones need to flip but it seems to work
            // at one point in the future probably should investigate and clean it up
            if (!Level.Current.InBounds(target, targetSize, false)) return false;
            if (RectangleTester.InBound(centerSize, center, target.x, target.y, false, flipboard)) return false;
            if (!RectangleTester.InBound(
                    new Vector2Int(radius, radius) * 2 + centerSize,
                    center - new Vector2Int(radius, radius), 
                    target.x, target.y, true, flipboard)) 
                return false;

            if (mode == SpotSelectionMode.Straight)
            {
                if (target.x != center.x && target.y != center.y) return false;
            }
            else if (mode == SpotSelectionMode.Diagonal)
            {
                var rel = target - center;
                if (Mathf.Abs(rel.x) != Mathf.Abs(rel.y)) return false;
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