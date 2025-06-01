using UnityEngine;

namespace UI
{
    public class CursorChanger : MonoBehaviour
    {
        [SerializeField] private Texture2D cursorTexture;
    
        private void Start()
        {
            Cursor.SetCursor(cursorTexture, new Vector2(291, 103), CursorMode.Auto);
        }
    }
}