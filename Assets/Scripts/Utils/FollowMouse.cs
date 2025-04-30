using System;
using Levels;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utils
{
    public class FollowMouse : MonoBehaviour
    {
        [SerializeField] private Vector2 offset;
        [SerializeField] private bool changePivot;
        [SerializeField] private bool snapToGridIfPossible;

        private RectTransform _rectTransform;
        private Transform _transform;
        private Canvas _parentCanvas;
        private Vector2 _originPivot;
        private bool _wasOnGrid;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _transform = transform;
        }

        private void Update()
        {
            UpdatePosition();
        }

        public void Init(Canvas parentCanvas)
        {
            _parentCanvas = parentCanvas;
        }

        public void UpdatePosition()
        {
            var mousePosition = Mouse.current.position.ReadValue();

            var wp = _parentCanvas.worldCamera.ScreenToWorldPoint(mousePosition);
            wp.z = 0;
            var gp = Level.Current.WorldToCell(wp);
            var useSnappedPosition = snapToGridIfPossible && CanSnapToGrid(gp);

            if (useSnappedPosition)
            {
                if (!_wasOnGrid)
                {
                    _originPivot = _rectTransform.pivot;
                }

                mousePosition = _parentCanvas.worldCamera.WorldToScreenPoint(GetMousePositionOnGrid(gp));
                _rectTransform.pivot = new Vector2(0.5f, 0);
                _wasOnGrid = true;
            }
            else if (_wasOnGrid)
            {
                _wasOnGrid = false;
                _rectTransform.pivot = _originPivot;
            }

            switch (_parentCanvas.renderMode)
            {
                case RenderMode.ScreenSpaceCamera:
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        _parentCanvas.transform as RectTransform, mousePosition,
                        _parentCanvas.worldCamera, out var pos);

                    _transform.position = _parentCanvas.transform.TransformPoint(pos + offset);
                    break;
                case RenderMode.ScreenSpaceOverlay:
                {
                    _transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);

                    if (!changePivot) break;

                    var pivotX = mousePosition.x / Screen.width;
                    var pivotY = mousePosition.y / Screen.height;

                    _rectTransform.pivot = new Vector2(Mathf.RoundToInt(pivotX), Mathf.RoundToInt(pivotY));
                    break;
                }
                case RenderMode.WorldSpace:
                    Debug.LogWarning("World space canvas not supported!");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool CanSnapToGrid(Vector2Int gp)
        {
            return Level.Current.InBounds(gp, Vector2Int.one);
        }

        private Vector2 GetMousePositionOnGrid(Vector2Int gp)
        {
            var wp = Level.Current.CellToWorld(gp);
            wp.z = 0;
            return wp;
        }
    }
}