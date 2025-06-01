using PrimeTween;
using UnityEngine;

namespace VFX
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite;
        
        public void BeginHide()
        {
            Tween.Alpha(sprite, 0, 0.05f);
        }
        
        public void Delete()
        {
            Destroy(gameObject);
        }
    }
}