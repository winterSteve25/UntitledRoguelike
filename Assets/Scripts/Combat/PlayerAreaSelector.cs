using System;
using System.Collections.Generic;
using System.Threading;
using Combat.UI;
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
        private bool _canceled;
        private Vector2Int? _selected;

        private Vector2Int _center;
        private Vector2Int _centerSize;
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
        public async UniTask<Vector2Int?> SelectArea(Vector2Int center, Vector2Int centerSize, int radius,
            Predicate<Vector2Int> isValid, SpotSelectionMode mode = SpotSelectionMode.Omnidirectional)
        {
            var flipboard = !CombatManager.Current.AmIFriendly;

            _center = center;
            _centerSize = centerSize;
            _radius = radius;
            _mode = mode;
            _isValid = isValid;
            _canceled = false;

            _ready = false;
            _selecting = true;

            selectedUnitUI.Show(null);
            selectedUnitUI.CanOpenMenu(false);

            for (int i = -radius; i <= radius + Mathf.FloorToInt(centerSize.x * 0.5f); i++)
            {
                for (int j = -radius; j <= radius + Mathf.FloorToInt(centerSize.y * 0.5f); j++)
                {
                    var pos = new Vector2Int(center.x + i, center.y + j);

                    var wp = Unit.GetWorldPosition(pos, Vector2Int.one);
                    var visual = _moveablePool.Get();
                    visual.transform.position = wp;
                    _activeObjects.Add(visual);
                    visual.GetComponentInChildren<SpriteRenderer>().color = Color.gray;

                    if (!_isValid(pos))
                    {
                        visual.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                    }

                    if (!IsValid(pos, flipboard))
                    {
                        visual.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
                    }

                    if (i == 0 && j == 0)
                    {
                        visual.GetComponentInChildren<SpriteRenderer>().color = Color.green;
                    }
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

        private bool IsValid(Vector2Int pos, bool flipBoard)
        {
            return IAreaSelector.IsValid(_center, _centerSize, pos, _radius, _mode, flipBoard);
        }
    }

    public enum SpotSelectionMode
    {
        Straight, // like rook
        Diagonal, // like bishop
        Omnidirectional // like queen
    }
}