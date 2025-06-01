using Cysharp.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class WinScreen : MonoBehaviour
    {
        private static readonly int ProgressPropertyId = Shader.PropertyToID("_Progress");
        private static readonly int Texture1 = Shader.PropertyToID("_Texture_1");
        private static readonly int Texture2 = Shader.PropertyToID("_Texture_2");

        [SerializeField] private CanvasGroup title;
        [SerializeField] private Image background;
        [SerializeField] private RectTransform panel;
        [SerializeField] private RectTransform panelInvisible;
        [SerializeField] private RectTransform panelVisible;
        [SerializeField] private RectTransform gainedElo;
        [SerializeField] private Slider eloSlider;
        [SerializeField] private TMP_Text lowerBoundElo;
        [SerializeField] private TMP_Text upperBoundElo;
        [SerializeField] private Image rank;
        [SerializeField] private RectTransform playersPanel;
        [SerializeField] private RectTransform rankPanel;
        [SerializeField] private CanvasGroup buttons;

        private void Update()
        {
            if (!Keyboard.current.spaceKey.wasPressedThisFrame) return;
            Open().Forget();
        }

        private async UniTaskVoid Open()
        {
            title.transform.localScale = new Vector3(3, 3, 1);
            title.alpha = 0;
            buttons.alpha = 0;
            
            playersPanel.pivot = new Vector2(0.5f, 0);
            playersPanel.anchoredPosition = Vector2.zero;
            rankPanel.anchorMin = new Vector2(0.5f, 1);
            rankPanel.anchorMax = new Vector2(0.5f, 1);
            
            Tween.Alpha(background, 0.3f, 0.5f);
            await Tween.Position(panel, panelVisible.position, 0.2f, Ease.OutCubic);
            await UniTask.WaitForSeconds(0.2f);
            await Tween.Scale(title.transform, new Vector3(1, 1, 1), 0.2f, Ease.InElastic)
                .Group(Tween.Alpha(title, 1, 0.15f));

            await UniTask.WaitForSeconds(0.1f);
            await Tween.UIPivotY(playersPanel, 1, 0.5f)
                .Group(Tween.UIAnchoredPositionY(playersPanel, -45, 0.5f));
            
            await UniTask.WaitForSeconds(0.5f);
            await Tween.UIAnchorMin(rankPanel, new Vector2(0.5f, 0), 0.5f)
                .Group(Tween.UIAnchorMax(rankPanel, new Vector2(0.5f, 0), 0.5f));
            
            await UniTask.WaitForSeconds(1f);

            await Tween.UIPivotX(gainedElo, 0, 0.2f, endDelay: 0.75f);
            await Tween.UIAnchorMin(gainedElo, Vector2.zero, 0.2f, Ease.InQuad)
                .Group(Tween.UIAnchorMax(gainedElo, Vector2.zero, 0.2f, Ease.InQuad));

            await Tween.UISliderValue(eloSlider, 1, 1.5f, ease: Ease.OutQuad);

            eloSlider.value = 0;
            lowerBoundElo.text = "2000";
            upperBoundElo.text = "2100";
            await Tween.MaterialProperty(rank.materialForRendering, ProgressPropertyId, 1f, 1f);

            rank.materialForRendering.SetFloat(ProgressPropertyId, 0);
            rank.materialForRendering.SetTexture(Texture2, rank.materialForRendering.GetTexture(Texture1));

            Tween.PunchScale(rank.transform, new Vector3(0.3f, 0.3f), 0.5f);
            Tween.MaterialProperty(rank.materialForRendering, ProgressPropertyId, 1f, 1f);
            Tween.UISliderValue(eloSlider, 0.075f, 1f, Ease.InOutCubic);

            Tween.Alpha(buttons, 1, 0.5f);
        }
    }
}