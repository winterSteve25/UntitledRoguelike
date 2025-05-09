using System.Text;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Combat.UI
{
    public class ScrollUI : MonoBehaviour
    {
        private const string IconText = "<sprite name=\"Energy\">";

        [SerializeField] private Sprite unscrollImage;
        [SerializeField] private Sprite scrollImage;

        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text cost;
        [SerializeField] private TMP_Text description;
        [SerializeField] private RectTransform panel;

        public void Show(IAbility ability)
        {
            title.text = ability.Name;
            cost.text = "Cost: " + new StringBuilder(IconText.Length * ability.Cost)
                .Insert(0, IconText, ability.Cost);
            description.text = ability.Description;

            LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
            Tween.UIPivotX(panel, 0, 0.1f);
        }

        public void Hide()
        {
            Tween.UIPivotX(panel, 1, 0.1f);
        }
    }
}