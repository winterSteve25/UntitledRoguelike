using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Combat
{
    public class AreaSelector : MonoBehaviour
    {
        public static AreaSelector Current { get; private set; }

        [SerializeField] private Camera cam;

        private bool _ready = false;
        private bool _selecting = false;
        private Vector2Int _selected;

        private Vector2Int _center;
        private Vector2Int _destSize;
        private int _radius;
        private SpotSelectionMode _mode;

        private void Awake()
        {
            Current = this;
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
            var clickedTile = new Vector2Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y));

            if (IsValid(clickedTile))
            {
                Debug.Log(clickedTile);
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
            SpotSelectionMode mode = SpotSelectionMode.Omnidirectional)
        {
            _center = center;
            _destSize = destinationSize;
            _radius = radius;
            _mode = mode;

            _ready = false;
            _selecting = true;

            await UniTask.WaitUntil(() => _ready);
            return _selected;
        }

        private bool IsValid(Vector2Int target)
        {
            Vector2Int anchor;

            bool hasCenter = _destSize.x % 2 == 1 && _destSize.y % 2 == 1;
            if (hasCenter)
            {
                // Odd-sized area: target tile is the center
                anchor = target;
            }
            else
            {
                // Even-sized or asymmetric area: target tile is top-left corner
                // Compute center from top-left
                anchor = target + new Vector2Int(_destSize.x / 2, _destSize.y / 2);
            }
            
            Debug.Log(hasCenter);

            Vector2Int offset = anchor - _center;
            int dx = Mathf.Abs(offset.x);
            int dy = Mathf.Abs(offset.y);
            
            Debug.Log(offset);

            switch (_mode)
            {
                case SpotSelectionMode.Straight:
                    if (dx != 0 && dy != 0) return false;
                    if (dx + dy > _radius) return false;
                    break;
                case SpotSelectionMode.Diagonal:
                    if (dx != dy) return false;
                    if (dx > _radius) return false;
                    break;
                case SpotSelectionMode.Omnidirectional:
                    if (dx + dy > _radius) return false;
                    break;
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