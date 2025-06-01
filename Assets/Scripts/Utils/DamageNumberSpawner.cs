using Combat;
using Cysharp.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;

namespace Utils
{
    public class DamageNumberSpawner : MonoBehaviour
    {
        public static DamageNumberSpawner Current { get; private set; }

        [SerializeField] private TMP_Text prefab;
        [SerializeField] private Camera cam;
        
        private void Awake()
        {
            Current = this;
        }

        public void SpawnNumber(Vector2 position, int amount, bool heal)
        {
            // TODO: use pools
            var txt = Instantiate(prefab, transform);
            txt.text = amount.ToString();
            txt.color = heal ? Unit.FriendlyColor : Unit.UnfriendlyColor;
            txt.transform.position = position;
            DelayDespawn(txt).Forget();
        }

        private async UniTaskVoid DelayDespawn(TMP_Text text)
        {
            Tween.PositionY(text.transform, text.transform.position.y + 1, 1, ease: Ease.InQuad);
            Tween.Custom(text.color.a, 0, 0.95f, f =>
            {
                var color = text.color;
                color.a = f;
                text.color = color;
            }, ease: Ease.InQuint);
            
            await UniTask.WaitForSeconds(1f);
            Destroy(text.gameObject);
        }
    }
}