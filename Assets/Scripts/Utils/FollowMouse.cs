using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utils
{
    public class FollowMouse : MonoBehaviour
    {
        [SerializeField] private Vector2 offset;
        [SerializeField] private bool changePivot;

        private RectTransform _rectTransform;
        private Transform _transform;
        private Canvas _parentCanvas;

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
    }
}