using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Combat
{
    public interface IAreaSelector
    {
        UniTask<Vector2Int?> SelectArea(Vector2Int center, Vector2Int centerSize, Vector2Int targetSize, int radius,
            Predicate<Vector2Int> isValid, SpotSelectionMode mode = SpotSelectionMode.Omnidirectional);

        public static bool IsValid(Vector2Int center, Vector2Int centerSize, Vector2Int target, int radius, SpotSelectionMode mode, bool flipboard)
        {
            if (RectangleTester.InBound(centerSize, center, target.x, target.y, false, flipboard)) return false;
            if (!RectangleTester.InBound(new Vector2Int(radius, radius) * 2 + centerSize,
                    center - new Vector2Int(radius, radius), target.x, target.y, false, flipboard)) return false;

            if (mode == SpotSelectionMode.Straight)
            {
                if ((target.x != center.x) == (target.y != center.y)) return false;
            }
            else if (mode == SpotSelectionMode.Diagonal)
            {
                var rel = target - center;
                if (Mathf.Abs(rel.x) != Mathf.Abs(rel.y)) return false;
            }

            return true;
        }
    }
}